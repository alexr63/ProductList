﻿// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web.UI.WebControls;
using Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using ProductList;
using SelectedHotelsModel;
using Subgurim.Controles;
using Telerik.Web.UI;
using Utils = ProductList.Utils;

namespace Cowrie.Modules.ProductList
{
    public partial class HotelList : PortalModuleBase, IActionable
    {
        protected double X_Map = 54.0;   // Latitude
        protected double Y_Map = -5.0;   // Longitude

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
                        BindData(db, X_Map, Y_Map, 10.0);

                        SavePersistentSetting();
                    }

                    Session["HiddenFieldX"] = X_Map.ToString();
                    Session["HiddenFieldY"] = Y_Map.ToString();
                    GLatLng _point = new GLatLng(X_Map, Y_Map);
                    ResetMap();
                    CreateMarker(_point);
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void ResetMap()
        {
            GMap1.reset();
            GMap1.addControl(new GControl(GControl.preBuilt.GOverviewMapControl));
            GMap1.addControl(new GControl(GControl.preBuilt.LargeMapControl));

            GMap1.addGMapUI(new GMapUI());

            KeyDragZoom keyDragZoom = new KeyDragZoom();
            keyDragZoom.key = KeyDragZoom.HotKeyEnum.ctrl;
            keyDragZoom.boxStyle = "{border: '4px solid #FFFF00'}";
            keyDragZoom.paneStyle = "{backgroundColor: 'black', opacity: 0.2, cursor: 'crosshair'}";

            GMap1.addKeyDragZoom(keyDragZoom);

            //GMap1.resetMarkerClusterer();
            GMap1.resetMarkers();
            GMap1.resetMarkerManager();

            GLatLng _point = new GLatLng(X_Map, Y_Map);
            GMap1.setCenter(_point, 5); // UK
        }
        protected GMarker CreateMarker(GLatLng FromPoint)
        {
            GMarker _Marker = new GMarker(FromPoint);
            GMarkerOptions _options = new GMarkerOptions();
            _options.draggable = true;
            _Marker.options = _options;

            GMap1.Add(_Marker);

            GMap1.addListener(new GListener(_Marker.ID, GListener.Event.dragend,
                 string.Format(@"
               function(overlay, point)
               {{
                  var ev = new serverEvent('myDragEnd', {0});
                  ev.addArg({0}.getZoom());
                  ev.addArg({1}.position.lng());
                  ev.addArg({1}.position.lat());
                  ev.send();
               }}
               ", GMap1.GMap_Id, _Marker.ID)));

            GMap1.addListener(new GListener(GMap1.GMap_Id, GListener.Event.click,
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
               ", GMap1.GMap_Id, _Marker.ID)));

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
                double distance = 10.0;
                BindData(db, Convert.ToDouble(Session["HiddenFieldX"]), Convert.ToDouble(Session["HiddenFieldY"]), distance);
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
            Enums.SortCriteriaEnum sortCriteria = (Enums.SortCriteriaEnum)Enum.Parse(typeof(Enums.SortCriteriaEnum), DropDownListSortCriterias.SelectedValue);
            switch (sortCriteria)
            {
                case Enums.SortCriteriaEnum.Name:
                    ListViewContent.DataSource = hotels.OrderBy(h => h.Name).ToList();
                    break;
                case Enums.SortCriteriaEnum.PriceAsc:
                    ListViewContent.DataSource = hotels.OrderBy(h => h.UnitCost).ToList();
                    break;
                case Enums.SortCriteriaEnum.PriceDesc:
                    ListViewContent.DataSource = hotels.OrderByDescending(h => h.UnitCost).ToList();
                    break;
                case Enums.SortCriteriaEnum.RatingAsc:
                    ListViewContent.DataSource = hotels.OrderBy(h => h.Star).ToList();
                    break;
                case Enums.SortCriteriaEnum.RatingDesc:
                    ListViewContent.DataSource = hotels.OrderByDescending(h => h.Star).ToList();
                    break;
            }
            ListViewContent.DataBind();

            //var selectedLocation = db.Locations.SingleOrDefault(l => l.Id == locationId);
            //LabelSelectedLocation.Text = selectedLocation.Name;

            LabelCount.Text = hotels.Count().ToString();
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
        protected string GMap1_ServerEvent(object s, GAjaxServerEventOtherArgs e)
        {
            switch (e.eventName)
            {
                case "myDragEnd":
                    string zoomLevel = e.eventArgs[0];

                    Session["HiddenFieldX"] = e.eventArgs[2];    // Coordinates swapped on client request
                    Session["HiddenFieldY"] = e.eventArgs[1];    //

                    GLatLng _point = new GLatLng(Convert.ToDouble(e.eventArgs[2], new System.Globalization.CultureInfo("en-US", false)),
                        Convert.ToDouble(e.eventArgs[1], new System.Globalization.CultureInfo("en-US", false)));

                    return GetInverseGeoCode(_point);
            }

            return string.Empty;
        }
        protected string GMap1_MarkerClick(object s, GAjaxServerEventArgs e)
        {
            return GetInverseGeoCode(e.point);
        }
        protected string GetInverseGeoCode(GLatLng FromPoint)
        {
            string streetNumber;
            string streetName;
            string locality;
            string postalCode;
            string _addr = Geocoder.NearestAddressGoogle(FromPoint.ToString(), out streetNumber, out streetName, out locality, out postalCode);
            if (_addr != String.Empty)
            {
                GInfoWindow window = new GInfoWindow(FromPoint, _addr, true);

                return window.ToString(GMap1.GMap_Id);
            }
            else
                return string.Empty;
        }
    }
}