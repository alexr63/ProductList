// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
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
                        int locationId = 1;
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
                            if (Settings["preselectedlocation"] != String.Empty)
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
                        var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                        LabelLocation.Text = location.Name;
                        var selectedLocation = db.Locations.SingleOrDefault(l => l.Id == preSelectedLocationId);
                        LabelSelectedLocation.Text = selectedLocation.Name;
                        Utils.PopulateLocationTree(RadTreeViewLocations, db, locationId, preSelectedLocationId);
                        Session["locationId"] = preSelectedLocationId;
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
            IList<Hotel> hotels = (from p in db.Products
                                   where !p.IsDeleted
                                   select p).OfType<Hotel>().ToList();
            var query = from h in hotels
                        where h.LocationId == locationId || h.Location.ParentId == locationId ||
                              (h.Location.ParentLocation != null && h.Location.ParentLocation.ParentId == locationId)
                        select h;
            if (TextBoxSearch.Text != String.Empty)
            {
                query =
                    query.Where(
                        p =>
                        p.Name.ToLower().Contains(TextBoxSearch.Text.ToLower()) ||
                        p.Description.ToLower().Contains(TextBoxSearch.Text.ToLower()));
            }
            if (DropDownListSortCriterias.SelectedValue == "Name")
            {
                ListViewContent.DataSource = query.OrderBy(h => h.Name).ToList();
            }
            else if (DropDownListSortCriterias.SelectedValue == "Price")
            {
                ListViewContent.DataSource = query.OrderBy(h => h.UnitCost).ToList();
            }
            else
            {
                ListViewContent.DataSource = query.OrderBy(h => h.Star).ToList();
            }
            ListViewContent.DataBind();

            LabelCount.Text = query.Count().ToString();
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
            int locationId = int.Parse(e.Node.Value);
            ViewState["locationId"] = locationId;
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, locationId);
            }
        }

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewState["locationId"] != null)
            {
                int locationId = Convert.ToInt32(ViewState["locationId"]);
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
            if (ViewState["locationId"] != null)
            {
                int locationId = Convert.ToInt32(ViewState["locationId"]);
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
    }
}