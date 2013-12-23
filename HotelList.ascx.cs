// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using ProductList;
using Telerik.Web.UI;

namespace Cowrie.Modules.ProductList
{
    public partial class HotelList : PortalModuleBase
    {
        public int DetailsTabId { get; set; }

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

            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
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
                        int? hotelTypeId = null;
                        if (Settings["hoteltype"] != null)
                        {
                            hotelTypeId = Convert.ToInt32(Settings["hoteltype"]);
                        }
                        Utils.PopulateLocationTree(RadTreeViewLocations, db, locationId, selectedLocationId, true, hotelTypeId);
                        BindData(db);

                        SavePersistentSetting();
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
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
            if (RadTreeViewLocations.SelectedValue != null)
            {
                Session["locationId"] = Convert.ToInt32(RadTreeViewLocations.SelectedValue);
            }
            else
            {
                Session.Remove("locationId");
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

        private void BindData(SelectedHotelsEntities db)
        {
            int locationId = Convert.ToInt32(RadTreeViewLocations.SelectedValue);
            int? hotelTypeId = null;
            if (Settings["hoteltype"] != null)
            {
                hotelTypeId = Convert.ToInt32(Settings["hoteltype"]);
            }
            var hotels = Utils.HotelsInLocation(db, locationId, hotelTypeId);
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

            var selectedLocation = db.Locations.SingleOrDefault(l => l.Id == locationId);
            LabelSelectedLocation.Text = selectedLocation.Name;

            LabelCount.Text = hotels.Count().ToString();
        }

        #region IActionable Members

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
                BindData(db);
            }
        }

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            Session["sortCriteria"] = DropDownListSortCriterias.SelectedValue;

            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db);
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
                BindData(db);
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
                BindData(db);
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
                BindData(db);
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
    }
}