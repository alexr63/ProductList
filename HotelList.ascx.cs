// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins;
using DotNetNuke.UI.Skins.Controls;
using DotNetNuke.UI.WebControls;
using ProductList;
using SelectedHotelsModel;
using Subgurim.Controles;
using Subgurim.Controles.GoogleChartIconMaker;

namespace Cowrie.Modules.ProductList
{
    public partial class HotelList : PortalModuleBase, IActionable
    {
        public int DetailsTabId { get; set; }
        public int EditTabId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var childTabs = TabController.GetTabsByParent(TabId, PortalId);
            if (childTabs.Count() > 0)
            {
                DetailsTabId = childTabs[0].TabID;
            }
            if (childTabs.Count() > 1)
            {
                EditTabId = childTabs[1].TabID;
            }

            try
            {
                if (!IsPostBack)
                {
                    LoadPersistentSettings();

                    StringBuilder sb = new StringBuilder();

                    sb.Append("var markersArray=[];");
                    sb.Append("function clearOverlays() {");
                    sb.Append("   for (var i = 0; i < markersArray.length; i++ ) {");
                    sb.Append("     markersArray[i].setMap(null);");
                    sb.Append("   }");
                    sb.Append("   markersArray = [];");
                    sb.Append("}");

                    locationGMap.Add(sb.ToString());

                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
#if DEBUGDB
                        db.Database.Log = logInfo => MyLogger.Log(logInfo, PortalSettings);
#endif
                        PanelCategories.Visible = false;
                        PanelProducts.Width = Unit.Pixel(870);

                        GeoName geoName = db.GeoNames.SingleOrDefault(gn => gn.Name == "London" && gn.CountryCode == "GB");
                        GLatLng point = new GLatLng(geoName.Latitude.Value, geoName.Longitude.Value);
                        if (Session["HiddenFieldX"] == null && Session["Location"] == null)
                        {
                            if (Settings["location"] != null)
                            {
                                var location = Settings["location"].ToString();
                                var geoNames = db.GeoNames.Where(gn => gn.Name.ToLower() == location.ToLower())
                                    .OrderByDescending(gn => gn.Population)
                                    .ThenByDescending(gn => gn.ModificationDate);
                                if (geoNames.Any())
                                {
                                    geoName = geoNames.FirstOrDefault();
                                    Session["HiddenFieldX"] = geoName.Latitude.Value;
                                    Session["HiddenFieldY"] = geoName.Longitude.Value;
                                }
                            }
                        }
                        else if (Session["Location"] != null)
                        {
                            var location = Session["Location"].ToString();
                            var geoNames = db.GeoNames.Where(gn => gn.Name.ToLower() == location.ToLower())
                                .OrderByDescending(gn => gn.Population)
                                .ThenByDescending(gn => gn.ModificationDate);
                            if (geoNames.Any())
                            {
                                geoName = geoNames.FirstOrDefault();
                                if (geoName.Latitude != null && geoName.Longitude != null)
                                {
                                    Session["HiddenFieldX"] = geoName.Latitude.Value;
                                    Session["HiddenFieldY"] = geoName.Longitude.Value;
                                }
                            }
                        }
                        else if (Session["HiddenFieldX"] != null)
                        {
                            point = new GLatLng(Convert.ToDouble(Session["HiddenFieldX"]), Convert.ToDouble(Session["HiddenFieldY"]));
                            var location = DbGeography.FromText(String.Format("POINT({0} {1})", point.lng, point.lat));
                            geoName = db.GeoNames.Where(gn => gn.FeatureClass == "P").OrderBy(gn => gn.Location.Distance(location)).First();
                        }

                        point = new GLatLng(Convert.ToDouble(Session["HiddenFieldX"]), Convert.ToDouble(Session["HiddenFieldY"]));
                        LabelSelectedLocation.Text = geoName.Name;

                        double distance = double.Parse(DropDownListDistance.SelectedValue);
                        ResetMap(point, distance);
                        CreateMarker(point);

                        BindData(db, point, distance);

                        SavePersistentSetting();
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void PopulateLocationsOnDemand(object source, DNNTextSuggestEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                var query = db.GeoNames.Where(gn => gn.Name.StartsWith(e.Text) && gn.FeatureClass == "P").Select(gn => gn.Name).Distinct().OrderBy(gn => gn).Take(10);
                foreach (string geoName in query)
                {
                    var objNode = new DNNNode(geoName) {ID = e.Nodes.Count.ToString()};
                    e.Nodes.Add(objNode);
                }
            }
        }

        private void ResetMap(GLatLng point, double radius)
        {
            //locationGMap.reset();
            //locationGMap.resetCustomOverlays();
            //locationGMap.markerManager = null;
            //locationGMap.resetGroundOverlays();
            //locationGMap.resetScreenOverlays();
            //locationGMap.addControl(new GControl(GControl.preBuilt.GOverviewMapControl));
            locationGMap.Add(new GControl(GControl.preBuilt.LargeMapControl));
            locationGMap.Add(new GControl(GControl.preBuilt.MapTypeControl));

            locationGMap.addGMapUI(new GMapUI());

            KeyDragZoom keyDragZoom = new KeyDragZoom();
            keyDragZoom.key = KeyDragZoom.HotKeyEnum.ctrl;
            keyDragZoom.boxStyle = "{border: '4px solid #FFFF00'}";
            keyDragZoom.paneStyle = "{backgroundColor: 'black', opacity: 0.2, cursor: 'crosshair'}";

            locationGMap.addKeyDragZoom(keyDragZoom);

            //locationGMap.resetMarkerClusterer();
            locationGMap.resetMarkers();
            locationGMap.resetMarkerManager();

            locationGMap.setCenter(point, GetZoomLevel(radius) - 1);
        }
        public int GetZoomLevel(double radius)
        {
            return (int)(14 - Math.Log(radius) / Math.Log(2));
        }
        protected GMarker CreateMarker(GLatLng FromPoint)
        {
            GMarker _Marker = new GMarker(FromPoint);
            GMarkerOptions _options = new GMarkerOptions();
            _options.draggable = true;
            _Marker.options = _options;

            locationGMap.Add(_Marker);

            locationGMap.addListener(new GListener(_Marker.ID, GListener.Event.dragend,
                 string.Format(@"
               function(overlay, point)
               {{
                  var ev = new serverEvent('myDragEnd', {0});
                  ev.addArg({0}.getZoom());
                  ev.addArg({1}.position.lng());
                  ev.addArg({1}.position.lat());
                  ev.send();
               }}
               ", locationGMap.GMap_Id, _Marker.ID)));

            locationGMap.addListener(new GListener(locationGMap.GMap_Id, GListener.Event.click,
                 string.Format(@"
               function(overlay, marker)
               {{
                  {1}.setPosition({0}.getCenter());
                  var ev = new serverEvent('myDragEnd', {0});
                  ev.addArg({0}.getZoom());
                  ev.addArg({1}.position.lng());
                  ev.addArg({1}.position.lat());
                  ev.send();
               }}
               ", locationGMap.GMap_Id, _Marker.ID)));

            return _Marker;
        }

        private void LoadPersistentSettings()
        {
            if (Session["distance"] != null)
            {
                DropDownListDistance.SelectedValue = Session["distance"].ToString();
            }
            else if (Settings["distance"] != null && Settings["distance"].ToString() != String.Empty)
            {
                DropDownListDistance.SelectedValue = Settings["distance"].ToString();
                Session["distance"] = Settings["distance"];
            }
            if (Session["filter"] != null)
            {
                TextBoxFilter.Text = Session["filter"].ToString();
            }
            else if (Settings["filter"] != null && Settings["filter"].ToString() != String.Empty)
            {
                TextBoxFilter.Text = Settings["filter"].ToString();
                Session["filter"] = Settings["filter"];
            }
            if (Session["sortCriteria"] != null)
            {
                DropDownListSortCriterias.SelectedValue = Session["sortCriteria"].ToString();
            }
            int startRowIndex = 0;
            if (Session["startRowIndex"] != null)
            {
                startRowIndex = Convert.ToInt32(Session["startRowIndex"]);
                DataPagerContent.SetPageProperties(startRowIndex, 10, false);
                DataPagerContent2.SetPageProperties(startRowIndex, 10, false);
            }
            if (Session["pageSize"] != null)
            {
                int pageSize = Convert.ToInt32(Session["pageSize"]);
                DropDownListPageSizes.SelectedValue = pageSize.ToString();
                DataPagerContent.SetPageProperties(startRowIndex, pageSize, false);
                DataPagerContent2.SetPageProperties(startRowIndex, pageSize, false);
            }
        }

        private void SavePersistentSetting()
        {
            Session["distance"] = DropDownListDistance.SelectedValue;
            if (TextBoxFilter.Text != String.Empty)
            {
                Session["filter"] = TextBoxFilter.Text;
            }
            else
            {
                Session.Remove("filter");
            }
            Session["sortCriteria"] = DropDownListSortCriterias.SelectedValue;
            Session["startRowIndex"] = DataPagerContent.StartRowIndex;
            Session["pageSize"] = Convert.ToInt32(DropDownListPageSizes.SelectedValue);
        }

        protected void ButtonLocate_Click(object sender, EventArgs e)
        {
            if (DNNTxtLocation.Text != String.Empty)
            {
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    var query = db.GeoNames.FirstOrDefault(gn => gn.Name.ToLower() == DNNTxtLocation.Text.ToLower());
                    if (query == null)
                    {
                        Skin.AddModuleMessage(this, "", "No location found - please type a valid location", ModuleMessage.ModuleMessageType.RedError);
                        return;
                    }
                }
                Session["Location"] = DNNTxtLocation.Text;
            }

            SavePersistentSetting();
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        public class HotelView : Hotel
        {
            public double? Distance { get; set; }
        }
        private void BindData(SelectedHotelsEntities db, GLatLng point, double distance)
        {
            int? hotelTypeId = null;
            if (Settings["hoteltype"] != null)
            {
                hotelTypeId = Convert.ToInt32(Settings["hoteltype"]);
            }
            var location = DbGeography.FromText(String.Format("POINT({0} {1})", point.lng, point.lat));
            var hotels = from hotel in db.Products.Where(p => !p.IsDeleted).OfType<Hotel>()
                let dist = hotel.Location.Distance(location)*.00062
                where
                    !hotel.IsDeleted && hotel.Location != null && hotel.Location.Latitude != null &&
                    hotel.Location.Longitude != null &&
                    dist <= distance &&
                    (hotelTypeId == null || hotel.HotelTypeId == hotelTypeId)
                select new HotelView
                {
                    Id = hotel.Id,
                    Name = hotel.Name,
                    Description = hotel.Description,
                    Distance = dist,
                    Star = hotel.Star,
                    Rooms = hotel.Rooms,
                    CustomerRating = hotel.CustomerRating,
                    Address = hotel.Address,
                    Location = hotel.Location,
                    Image = hotel.Image,
                    UnitCost = hotel.UnitCost,
                    CurrencyCode = hotel.CurrencyCode
                };

            if (Session["filter"] != null)
            {
                hotels =
                    hotels.Where(
                        p =>
                        p.Name.ToLower().Contains(TextBoxFilter.Text.ToLower()) ||
                        p.Description.ToLower().Contains(TextBoxFilter.Text.ToLower()));
                LabelFilteredBy.Text = String.Format("and filtered by \"{0}\"", TextBoxFilter.Text);
                LabelFilteredBy.Visible = true;
                ButtonClearFilter.Visible = true;
            }
            else
            {
                LabelFilteredBy.Visible = false;
                ButtonClearFilter.Visible = false;
            }
            List<HotelView> hotelList = null;
            Enums.SortCriteriaEnum sortCriteria = (Enums.SortCriteriaEnum)Enum.Parse(typeof(Enums.SortCriteriaEnum), DropDownListSortCriterias.SelectedValue);
            switch (sortCriteria)
            {
                case Enums.SortCriteriaEnum.Name:
                    hotelList = hotels.OrderBy(h => h.Name).ToList();
                    break;
                case Enums.SortCriteriaEnum.PriceAsc:
                    hotelList = hotels.OrderBy(h => h.UnitCost).ToList();
                    break;
                case Enums.SortCriteriaEnum.PriceDesc:
                    hotelList = hotels.OrderByDescending(h => h.UnitCost).ToList();
                    break;
                case Enums.SortCriteriaEnum.RatingAsc:
                    hotelList = hotels.OrderBy(h => h.Star).ToList();
                    break;
                case Enums.SortCriteriaEnum.RatingDesc:
                    hotelList = hotels.OrderByDescending(h => h.Star).ToList();
                    break;
                case Enums.SortCriteriaEnum.DistanceAsc:
                    hotelList = hotels.OrderBy(h => h.Distance).ToList();
                    break;
                case Enums.SortCriteriaEnum.DistanceDesc:
                    hotelList = hotels.OrderByDescending(h => h.Distance).ToList();
                    break;
            }
            ListViewContent.DataSource = hotelList;
            ListViewContent.DataBind();

            var count = hotels.Count();
            DataPagerContent.Visible = count > DataPagerContent.PageSize;
            DataPagerContent2.Visible = count > DataPagerContent2.PageSize;
            LabelCount.Text = count.ToString();

            int i = DataPagerContent.StartRowIndex + 1;
            foreach (var hotel in hotelList.Skip(DataPagerContent.StartRowIndex).Take(DataPagerContent.PageSize))
            {
                GLatLng gLatLng = new GLatLng(hotel.Location.Latitude.Value, hotel.Location.Longitude.Value);
                PinLetter pinLetter = new PinLetter(i.ToString(), Color.FromArgb(0xB4, 0x97, 0x59), Color.Black);
                var markerOptions = new GMarkerOptions(new GIcon(pinLetter.ToString(), pinLetter.Shadow()), hotel.Name.Replace("'", "´"));
                GMarker marker = new GMarker(gLatLng, markerOptions);
                StringBuilder sb = new StringBuilder();
                sb.Append(hotel.Name);
                sb.AppendFormat("<br />Address:&nbsp;{0}", hotel.Address);
                GInfoWindow window = new GInfoWindow(marker, sb.ToString(), false);
                locationGMap.addInfoWindow(window);
                i++;
            }

        }

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                var actions = new ModuleActionCollection
                    {
                        {
                            GetNextActionID(), "Add Product", "", "", "",
                            EditUrl(), false, SecurityAccessLevel.Edit, true, false
                        }
                    };
                return actions;
            }
        }

        #endregion

        protected void DataListContent_DataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
            }
        }

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["sortCriteria"] = DropDownListSortCriterias.SelectedValue;
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected void DropDownListPageSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = Convert.ToInt32(DropDownListPageSizes.SelectedValue);
            Session["pageSize"] = pageSize;
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected void ListViewContent_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            Session["startRowIndex"] = e.StartRowIndex;
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected void ButtonFilter_Click(object sender, EventArgs e)
        {
            SavePersistentSetting();
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected void ButtonClearFilter_Click(object sender, EventArgs e)
        {
            TextBoxFilter.Text = String.Empty;
            Session.Remove("filter");

            SavePersistentSetting();
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected void ListViewContent_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            if (e.CommandName == "BookNow")
            {
                Response.Redirect(e.CommandArgument.ToString());
            }
            else if (e.CommandName == "MoreHotelInfo")
            {
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + e.CommandArgument.ToString()));
            }
        }
        protected string locationGMap_ServerEvent(object s, GAjaxServerEventOtherArgs e)
        {
            switch (e.eventName)
            {
                case "myDragEnd":
                    string zoomLevel = e.eventArgs[0];

                    Session["HiddenFieldX"] = e.eventArgs[2];    // Coordinates swapped on client request
                    Session["HiddenFieldY"] = e.eventArgs[1];    //
                    Session.Remove("Location");

                    GLatLng point = new GLatLng(Convert.ToDouble(e.eventArgs[2], new System.Globalization.CultureInfo("en-US", false)),
                        Convert.ToDouble(e.eventArgs[1], new System.Globalization.CultureInfo("en-US", false)));

                    return "clearOverlays();";
            }

            return string.Empty;
        }
        protected string locationGMap_MarkerClick(object s, GAjaxServerEventArgs e)
        {
            string addr;
            return GetInverseGeoCode(e.point, out addr);
        }
        protected string GetInverseGeoCode(GLatLng FromPoint, out string addr)
        {
            string streetNumber;
            string streetName;
            string locality;
            string postalCode;
            string _addr = Geocoder.NearestAddressGoogle(FromPoint.ToString(), out streetNumber, out streetName, out locality, out postalCode);
            if (_addr != String.Empty)
            {
                GInfoWindow window = new GInfoWindow(FromPoint, _addr, true);
                addr = _addr;
                return window.ToString(locationGMap.GMap_Id);
            }
            else
                addr = String.Empty;
                return string.Empty;
        }
        protected string locationGMap_Click(object s, GAjaxServerEventArgs e)
        {
            return string.Format("{0}.panTo({1})", e.who, e.point.ToString("new"));
        }
    }
}