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
                        int preSelectedLocationId = locationId;
                        try
                        {
                            if (Settings["preselectedlocation"] != null && Settings["preselectedlocation"].ToString() != String.Empty)
                            {
                                preSelectedLocationId = Convert.ToInt32(Settings["preselectedlocation"]);
                            }
                            if (Session["locationId"] != null)
                            {
                                preSelectedLocationId = Convert.ToInt32(Session["locationId"]);
                            }
                        }
                        catch (Exception)
                        {
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

                        var selectedLocation = db.Locations.SingleOrDefault(l => l.Id == preSelectedLocationId);
                        LabelSelectedLocation.Text = selectedLocation.Name;
                        Utils.PopulateLocationTree(RadTreeViewLocations, db, locationId, preSelectedLocationId, true);
                        Session["locationId"] = preSelectedLocationId;
                        if (Settings["search"] != null &&
                            Settings["search"].ToString() != String.Empty)
                        {
                            TextBoxSearch.Text = Settings["search"].ToString();
                        }
                        BindData(db, preSelectedLocationId);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void BindData(SelectedHotelsEntities db, int locationId)
        {
            var hotels = Utils.HotelsInLocation(db, locationId);
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
            if (DropDownListSortCriterias.SelectedValue == "Name")
            {
                ListViewContent.DataSource = hotels.OrderBy(h => h.Name).ToList();
            }
            else if (DropDownListSortCriterias.SelectedValue == "Price")
            {
                ListViewContent.DataSource = hotels.OrderBy(h => h.UnitCost).ToList();
            }
            else
            {
                ListViewContent.DataSource = hotels.OrderBy(h => h.Star).ToList();
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
            DataPagerContent.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false); 

            int locationId = int.Parse(e.Node.Value);
            Session["locationId"] = locationId;
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, locationId);
            }
        }

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Session["locationId"] != null)
            {
                int locationId = Convert.ToInt32(Session["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindData(db, locationId);
                }
            }
        }

        protected void DropDownListPageSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataPagerContent.PageSize = Convert.ToInt32(DropDownListPageSizes.SelectedValue);
        }

        protected void ListViewContent_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            //set current page startindex, max rows and rebind to false
            DataPagerContent.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            //rebind List View
            if (Session["locationId"] != null)
            {
                int locationId = Convert.ToInt32(Session["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindData(db, locationId);
                }
            }
        }

        protected void RadTreeViewLocations_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? locationId = Convert.ToInt32(e.Node.Value);
                var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                CreateSubLocationNodes(location, e.Node);
            }
            e.Node.Expanded = true;
        }

        public static int? PopulateTree(RadTreeView radTreeView, SelectedHotelsEntities db, int locationId, int? selectedLocationId = null)
        {
            radTreeView.Nodes.Clear();
            IOrderedQueryable<Location> topLocations = from l in db.Locations
                                                       where !l.IsDeleted &&
                                                             l.Id == locationId
                                                       orderby l.Name
                                                       select l;
            foreach (Location location in topLocations)
            {
                RadTreeNode node = new RadTreeNode();
                node.Text = location.Name;
                node.ToolTip = location.Name;
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                node.Value = location.Id.ToString();
                if (selectedLocationId != null && location.Id == selectedLocationId)
                {
                    node.Selected = true;
                }
                radTreeView.Nodes.Add(node);
                //CreateSubLocationNodes(location, objNode, selectedLocationId);
            }
            if (topLocations.Any())
            {
                return topLocations.First().Id;
            }
            return null;
        }

        public static void CreateSubLocationNodes(Location location, RadTreeNode objNode, int? selectedLocationId = null)
        {
            if (location.SubLocations.Any(l => !l.IsDeleted))
            {
                var subLocations = from l in location.SubLocations
                                   where !l.IsDeleted
                                   orderby l.Name
                                   select l;
                foreach (Location subLocation in subLocations)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Text = subLocation.Name;
                    node.ToolTip = subLocation.Name;
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                    node.Value = subLocation.Id.ToString();
                    if (selectedLocationId != null && subLocation.Id == selectedLocationId)
                    {
                        node.Selected = true;
                    }
                    if (subLocation.SubLocations.Any())
                    {
                                            node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                    }
                    objNode.Nodes.Add(node);
                    //CreateSubLocationNodes(subLocation, objSubNode, selectedLocationId);
                }
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            DataPagerContent.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false); 

            if (Session["locationId"] != null)
            {
                int locationId = Convert.ToInt32(Session["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindData(db, locationId);
                }
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

        protected void ButtonClear_Click(object sender, EventArgs e)
        {
            TextBoxSearch.Text = String.Empty;

            DataPagerContent.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false);

            if (Session["locationId"] != null)
            {
                int locationId = Convert.ToInt32(Session["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindData(db, locationId);
                }
            }
        }
    }
}