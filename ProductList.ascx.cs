﻿// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;
using Telerik.Web.UI;

namespace Cowrie.Modules.ProductList
{
    public partial class ProductList : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Settings["category"] == null)
            {
                return;
            }

            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        int mode = Convert.ToInt32(Settings["mode"]);
                        if (mode == (int)Enums.DisplayModeEnum.Hotels)
                        {
                            int locationId = Convert.ToInt32(Settings["location"]);
                            var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                            LabelLocation.Text = location.Name;
                            Utils.PopulateLocationTree(RadTreeViewLocations, db, locationId, locationId);
                            ViewState["locationId"] = locationId;
                            BindDataByLocation(db, locationId);
                            MultiView1.SetActiveView(ViewHotels);
                        }
                        else
                        {
                            int categoryId = Convert.ToInt32(Settings["category"]);
                            var category = db.Categories.SingleOrDefault(l => l.Id == categoryId);
                            LabelCategory.Text = category.Name;
                            BindDataByCategory(db, categoryId);
                            MultiView1.SetActiveView(ViewProducts);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void BindDataByCategory(SelectedHotelsEntities db, int categoryId)
        {
            var category = db.Categories.SingleOrDefault(l => l.Id == categoryId);
            if (category != null)
            {
                LabelSelectedCategory.Text = category.Name;
            }
            IList<Product> products = (from p in db.Products
                                       where !p.IsDeleted
                                       select p).OfType<Product>().ToList();
            var query = from p in products
                         where p.Categories.Any(c => c.Id == categoryId || (c.ParentCategory != null && c.ParentCategory.Id == categoryId))
                         select p;
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
                ListViewContent.DataSource = query.OrderBy(p => p.Name).ToList();
            }
            else
            {
                ListViewContent.DataSource = query.OrderBy(p => p.UnitCost).ToList();
            }
            ListViewContent.DataBind();

            LabelCount.Text = query.Count().ToString();
        }

        private void BindDataByLocation(SelectedHotelsEntities db, int locationId)
        {
            var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
            if (location != null)
            {
                LabelSelectedLocation.Text = location.Name;
            }
            IList<Hotel> hotels = (from p in db.Products
                                   where !p.IsDeleted
                                   select p).OfType<Hotel>().ToList();
            var query = from h in hotels
                        where h.LocationId == locationId || h.Location.ParentId == locationId ||
                              (h.Location.ParentLocation != null && h.Location.ParentLocation.ParentId == locationId)
                        select h;
            if (TextBoxSearch2.Text != String.Empty)
            {
                query =
                    query.Where(
                        p =>
                        p.Name.ToLower().Contains(TextBoxSearch2.Text.ToLower()) ||
                        p.Description.ToLower().Contains(TextBoxSearch2.Text.ToLower()));
            }
            if (DropDownListSortCriterias2.SelectedValue == "Name")
            {
                ListViewContent2.DataSource = query.OrderBy(h => h.Name).ToList();
            }
            else
            {
                ListViewContent2.DataSource = query.OrderBy(h => h.UnitCost).ToList();
            }
            ListViewContent2.DataBind();

            LabelCount2.Text = query.Count().ToString();
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

        protected void DNNTreeCategories_NodeClick(object source, DNNTreeNodeClickEventArgs e)
        {
            int categoryId = int.Parse(e.Node.Key);
            ViewState["categoryId"] = categoryId;
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindDataByCategory(db, categoryId);
            }
        }

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewState["categoryId"] != null)
            {
                int categoryId = Convert.ToInt32(ViewState["categoryId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindDataByCategory(db, categoryId);
                }
            }
        }

        protected void DropDownListSortCriterias2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewState["locationId"] != null)
            {
                int locationId = Convert.ToInt32(ViewState["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindDataByLocation(db, locationId);
                }
            }
        }

        protected void DropDownListPageSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataPagerContent.PageSize = Convert.ToInt32(DropDownListPageSizes.SelectedValue);
        }

        protected void DropDownListPageSizes2_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataPagerContent2.PageSize = Convert.ToInt32(DropDownListPageSizes2.SelectedValue);
        }

        protected void ListViewContent_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            //set current page startindex, max rows and rebind to false
            DataPagerContent.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            //rebind List View
            if (ViewState["categoryId"] != null)
            {
                int categoryId = Convert.ToInt32(ViewState["categoryId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindDataByCategory(db, categoryId);
                }
            }
        }

        protected void ListViewContent2_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            //set current page startindex, max rows and rebind to false
            DataPagerContent2.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            //rebind List View
            if (ViewState["locationId"] != null)
            {
                int locationId = Convert.ToInt32(ViewState["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindDataByLocation(db, locationId);
                }
            }
        }

        protected void DNNTreeCategories_PopulateOnDemand(object source, DotNetNuke.UI.WebControls.DNNTreeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? categoryId = Convert.ToInt32(e.Node.Key);
                var category = db.Categories.SingleOrDefault(l => l.Id == categoryId);
                Utils.CreateSubCategoryNodes(category, e.Node, categoryId);
            }
        }

        protected void RadTreeViewLocations_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? locationId = Convert.ToInt32(e.Node.Value);
                var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                CreateSubLocationNodes(location, e.Node, locationId);
            }
            e.Node.Expanded = true;
        }

        public static void CreateSubLocationNodes(Location location, RadTreeNode objNode, int? selectedLocationId)
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
                    if (selectedLocationId != null && location.Id == selectedLocationId)
                    {
                        node.Selected = true;
                    }
                    objNode.Nodes.Add(node);
                    //CreateSubLocationNodes(subLocation, objSubNode, selectedLocationId);
                }
            }
        }

        protected void RadTreeViewLocations_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            int locationId = int.Parse(e.Node.Value);
            ViewState["locationId"] = locationId;
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindDataByLocation(db, locationId);
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (ViewState["categoryId"] != null)
            {
                int categoryId = Convert.ToInt32(ViewState["categoryId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindDataByCategory(db, categoryId);
                }
            }
        }

        protected void ButtonSubmit2_Click(object sender, EventArgs e)
        {
            if (ViewState["locationId"] != null)
            {
                int locationId = Convert.ToInt32(ViewState["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindDataByLocation(db, locationId);
                }
            }
        }
    }
}