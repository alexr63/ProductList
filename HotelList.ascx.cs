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
using ProductList;
using SelectedHotelsModel;
using Subgurim.Controles;
using Subgurim.Controles.GoogleChartIconMaker;
using Telerik.Web.UI;
using Utils = ProductList.Utils;

namespace Cowrie.Modules.ProductList
{
    public partial class HotelList : PortalModuleBase, IActionable
    {
        protected double X_Map = 54.0;   // Latitude
        protected double Y_Map = -5.0;   // Longitude
        protected double distance = 10.0;

        public int DetailsTabId { get; set; }
        public int EditTabId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Settings["location"] == null)
            {
                return;
            }

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
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
#if DEBUG
                        db.Database.Log = logInfo => MyLogger.Log(logInfo, PortalSettings);
#endif
                        int locationId = 1069;
                        try
                        {
                            locationId = Convert.ToInt32(Settings["location"]);
                        }
                        catch (Exception)
                        {
                        }
                        int selectedLocationId = locationId;
                        if (Session["ReturnFromDetails"] != null)
                        {
                            Session["ReturnFromDetails"] = null;
                            LoadPersistentSettings(ref selectedLocationId);
                        }
                        else
                        {
                            try
                            {
                                if (Settings["preselectedlocation"] != null && Settings["preselectedlocation"].ToString() != String.Empty)
                                {
                                    selectedLocationId = Convert.ToInt32(Settings["preselectedlocation"]);
                                }
                            }
                            catch (Exception)
                            {
                            }
                            if (Settings["search"] != null &&
                                Settings["search"].ToString() != String.Empty)
                            {
                                TextBoxSearch.Text = Settings["search"].ToString();
                            }
                        }

#if MULTIPLELOCATIONS
                        List<Location> selectedLocations = new List<Location>();
                        object setting = Settings["locations"];
                        if (setting != null)
                        {
                            try
                            {
                                string[] selectedLocationIds = setting.ToString().Split(';');
                                foreach (string s in selectedLocationIds)
                                {
                                    int locationId = int.Parse(s);
                                    var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                                    if (location != null)
                                    {
                                        selectedLocations.Add(location);
                                    }
                                }
                                LabelCurrentLocation.Text = String.Join(", ", selectedLocations);
                            }
                            catch (Exception)
                            {
                            }
                        }
#endif
                        var selectedLocation = db.Locations.SingleOrDefault(l => l.Id == selectedLocationId);
                        LabelSelectedLocation.Text = selectedLocation.Name;
                        if (Settings["hidetree"] == null || !Convert.ToBoolean(Settings["hidetree"]))
                        {
                            int? hotelTypeId = null;
                            if (Settings["hoteltype"] != null)
                            {
                                hotelTypeId = Convert.ToInt32(Settings["hoteltype"]);
                            }
                            //Utils.PopulateLocationTree(RadTreeViewLocations, db, locationId, selectedLocationId, false, hotelTypeId);
                            const int englandGeoNameId = 6269131;
                            Utils.PopulateGeoLocationTree(RadTreeViewLocations, db, englandGeoNameId, englandGeoNameId, false, hotelTypeId);
                        }
                        else
                        {
                            PanelCategories.Visible = false;
                            PanelProducts.Width = Unit.Pixel(870);
                        }
                        PanelCategories.Visible = false;
                        PanelProducts.Width = Unit.Pixel(870);

                        var london = db.GeoNames.SingleOrDefault(gn => gn.Name == "London" && gn.CountryCode == "GB");
                        X_Map = london.Latitude.Value;
                        Y_Map = london.Longitude.Value;
                        BindData(db, X_Map, Y_Map, distance);

                        SavePersistentSetting();
                    }

                    Session["HiddenFieldX"] = X_Map.ToString();
                    Session["HiddenFieldY"] = Y_Map.ToString();

                    GLatLng _point = new GLatLng(Convert.ToDouble(Session["HiddenFieldX"]), Convert.ToDouble(Session["HiddenFieldY"]));
                    double _radius = double.Parse(DropDownListDistance.SelectedValue);
                    ResetMap(_point, _radius);
                    CreateMarker(_point);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void ResetMap(GLatLng point, double radius)
        {
            //locationGMap.reset();
            //locationGMap.addControl(new GControl(GControl.preBuilt.GOverviewMapControl));
            locationGMap.Add(new GControl(GControl.preBuilt.LargeMapControl));
            locationGMap.Add(new GControl(GControl.preBuilt.MapTypeControl));

            //locationGMap.addGMapUI(new GMapUI());

            //KeyDragZoom keyDragZoom = new KeyDragZoom();
            //keyDragZoom.key = KeyDragZoom.HotKeyEnum.ctrl;
            //keyDragZoom.boxStyle = "{border: '4px solid #FFFF00'}";
            //keyDragZoom.paneStyle = "{backgroundColor: 'black', opacity: 0.2, cursor: 'crosshair'}";

            //locationGMap.addKeyDragZoom(keyDragZoom);

            ////locationGMap.resetMarkerClusterer();
            locationGMap.resetMarkers();
            locationGMap.resetMarkerManager();

            locationGMap.setCenter(point, GetZoomLevel(radius) - 1); // UK
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

        private void LoadPersistentSettings(ref int selectedLocationId)
        {
            if (Session["locationId"] != null)
            {
                selectedLocationId = Convert.ToInt32(Session["locationId"]);
            }
            if (Session["search"] != null)
            {
                TextBoxSearch.Text = Session["search"].ToString();
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
            if (PanelCategories.Visible)
            {
                if (!String.IsNullOrEmpty(RadTreeViewLocations.SelectedValue))
                {
                    Session["locationId"] = Convert.ToInt32(RadTreeViewLocations.SelectedValue);
                }
                else
                {
                    Session.Remove("locationId");
                }
            }
            if (TextBoxSearch.Text != String.Empty)
            {
                Session["search"] = TextBoxSearch.Text;
            }
            else
            {
                Session.Remove("search");
            }
            Session["sortCriteria"] = DropDownListSortCriterias.SelectedValue;
            Session["startRowIndex"] = DataPagerContent.StartRowIndex;
            Session["pageSize"] = Convert.ToInt32(DropDownListPageSizes.SelectedValue);
        }

        protected void ButtonLocate_Click(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                GLatLng _point = new GLatLng(Convert.ToDouble(Session["HiddenFieldX"]), Convert.ToDouble(Session["HiddenFieldY"]));
                double _radius = double.Parse(DropDownListDistance.SelectedValue);
                ResetMap(_point, _radius);
                CreateMarker(_point);
                BindData(db, Convert.ToDouble(Session["HiddenFieldX"]), Convert.ToDouble(Session["HiddenFieldY"]), _radius);
            }
        }

        private void BindData(SelectedHotelsEntities db, double lat, double lon, double distance)
        {
            var location = DbGeography.FromText(String.Format("POINT({0} {1})", lon, lat));
            var hotels = from hotel in db.Products.Where(p => !p.IsDeleted).OfType<Hotel>()
                where !hotel.IsDeleted && hotel.Location != null &&
                hotel.Location.Distance(location) * .00062 <= distance
                select hotel;

            int locationId = Convert.ToInt32(Settings["location"]);
            if (PanelCategories.Visible && Session["locationId"] != null)
            {
                locationId = Convert.ToInt32(Session["locationId"]);
            }
#if DEBUG
            //locationId = 6269131;
#endif
            int? hotelTypeId = null;
            if (Settings["hoteltype"] != null)
            {
                hotelTypeId = Convert.ToInt32(Settings["hoteltype"]);
            }
            //var hotels = db.HotelsInLocation(locationId, hotelTypeId);
            //var hotels = db.HotelsInGeoLocation(locationId, hotelTypeId);
            if (TextBoxSearch.Text != String.Empty)
            {
                hotels =
                    hotels.Where(
                        p =>
                        p.Name.ToLower().Contains(TextBoxSearch.Text.ToLower()) ||
                        p.Description.ToLower().Contains(TextBoxSearch.Text.ToLower()));
                LabelFilteredBy.Text = String.Format("and filtered by \"{0}\"", TextBoxSearch.Text);
                LabelFilteredBy.Visible = true;
                ButtonClear.Visible = true;
            }
            else
            {
                LabelFilteredBy.Visible = false;
                ButtonClear.Visible = false;
            }
            List<Hotel> hotelList = null;
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
            }
            ListViewContent.DataSource = hotelList;
            ListViewContent.DataBind();

            //var selectedLocation = db.Locations.SingleOrDefault(l => l.Id == locationId);
            if (Session["Location"] != null)
            {
                LabelSelectedLocation.Text = Session["Location"].ToString();
            }

            LabelCount.Text = hotels.Count().ToString();

            int i = 1;
            foreach (var hotel in hotelList)
            {
                GLatLng gLatLng = new GLatLng(hotel.Lat.Value, hotel.Lon.Value);
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

        protected void RadTreeViewLocations_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            int locationId = Convert.ToInt32(RadTreeViewLocations.SelectedValue);
            Session["locationId"] = locationId;

            DataPagerContent.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false);
            DataPagerContent2.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false); 

            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, 54.0, -5.0, 10.0);
            }
        }

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["sortCriteria"] = DropDownListSortCriterias.SelectedValue;

            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, 54.0, -5.0, 10.0);
            }
        }

        protected void DropDownListPageSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = Convert.ToInt32(DropDownListPageSizes.SelectedValue);
            Session["pageSize"] = pageSize;

            DataPagerContent.PageSize = pageSize;
            DataPagerContent2.PageSize = pageSize;
        }

        protected void ListViewContent_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            Session["startRowIndex"] = e.StartRowIndex;

            //set current page startindex, max rows and rebind to false
            DataPagerContent.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);
            DataPagerContent2.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, 54.0, -5.0, 10.0);
            }
        }

        protected void RadTreeViewLocations_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? locationId = Convert.ToInt32(e.Node.Value);
                var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                int selectedLocationId = Convert.ToInt32(RadTreeViewLocations.SelectedValue);
                int? hotelTypeId = null;
                if (Settings["hoteltype"] != null)
                {
                    hotelTypeId = Convert.ToInt32(Settings["hoteltype"]);
                }
                Utils.CreateSubLocationNodes(db, location, e.Node, selectedLocationId, hotelTypeId);
            }
            e.Node.Expanded = true;
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            Session["search"] = TextBoxSearch.Text;

            DataPagerContent.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false);
            DataPagerContent2.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false); 

            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, 54.0, -5.0, 10.0);
            }
        }

        protected void ButtonClear_Click(object sender, EventArgs e)
        {
            TextBoxSearch.Text = String.Empty;
            Session.Remove("search");

            DataPagerContent.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false);
            DataPagerContent2.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false);

            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, 54.0, -5.0, 10.0);
            }
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

                    GLatLng _point = new GLatLng(Convert.ToDouble(e.eventArgs[2], new System.Globalization.CultureInfo("en-US", false)),
                        Convert.ToDouble(e.eventArgs[1], new System.Globalization.CultureInfo("en-US", false)));
                    string addr;
                    var inverseGeoCode = GetInverseGeoCode(_point, out addr);
                    Session["Location"] = addr;

                    //ResetMap();
                    //CreateMarker(_point);

                    //return inverseGeoCode;
                    return string.Empty;
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

        //protected void DNNTextSuggestLocation_PopulateOnDemand(object source, DotNetNuke.UI.WebControls.DNNTextSuggestEventArgs e)
        //{
        //    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
        //    {
        //        var query = from gn in db.GeoNames
        //                    where gn.Name.StartsWith(e.Text)
        //                    orderby gn.Name
        //                    select gn;
        //        foreach (var geoName in query.Take(DNNTextSuggestLocation.MaxSuggestRows))
        //        {
        //            e.Nodes.Add(new DNNNode(geoName.Name));
        //        }
        //    }
        //}
        //protected void DNNTextSuggestLocation_NodeClick(object source, DNNTextSuggestEventArgs e)
        //{
        //    DNNTextSuggestLocation.Text = e.Text;
        //}
        //protected void DNNTxtBannerGroup_PopulateOnDemand(object source, DNNTextSuggestEventArgs e)
        //{
        //    DNNNode objNode = new DNNNode("q1");
        //    objNode.ID = e.Nodes.Count.ToString();
        //    e.Nodes.Add(objNode);
        //    objNode = new DNNNode("q2");
        //    objNode.ID = e.Nodes.Count.ToString();
        //    e.Nodes.Add(objNode);
        //}
    }
}