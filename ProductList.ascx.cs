﻿// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;
using SelectedHotelsModel;
using Telerik.Web.UI;

namespace Cowrie.Modules.ProductList
{
    public partial class ProductList : PortalModuleBase
    {
        public int DetailsTabId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Settings["category"] == null)
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
                        int mode = (int) Enums.DisplayModeEnum.Hotels;
                        mode = Convert.ToInt32(Settings["mode"]);
                        try
                        {
                            mode = Convert.ToInt32(Settings["mode"]);
                        }
                        catch (Exception)
                        {
                        }
                        if (mode == (int)Enums.DisplayModeEnum.Hotels)
                        {
                            MultiView1.SetActiveView(ViewHotels);
                        }
                        else if (mode == (int) Enums.DisplayModeEnum.Boats)
                        {
                            IList<Boat> boats = (from p in db.Products
                                                 where !p.IsDeleted
                                                 select p).OfType<Boat>().OrderBy(p => p.Name).ToList();
                            ListViewContent3.DataSource = boats;
                            ListViewContent3.DataBind();

                            DropDownListModels.DataSource = boats;
                            DropDownListModels.DataBind();

                            MultiView1.SetActiveView(ViewBoats);
                        }
                        else
                        {
                            int categoryId = 1;
                            try
                            {
                                categoryId = Convert.ToInt32(Settings["category"]);
                            }
                            catch (Exception)
                            {
                            }
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
            Session["categoryId"] = categoryId;
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindDataByCategory(db, categoryId);
            }
        }

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Session["categoryId"] != null)
            {
                int categoryId = Convert.ToInt32(Session["categoryId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindDataByCategory(db, categoryId);
                }
            }
        }

        protected void DropDownListSortCriterias2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Session["locationId"] != null)
            {
                int locationId = Convert.ToInt32(Session["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
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
            if (Session["categoryId"] != null)
            {
                int categoryId = Convert.ToInt32(Session["categoryId"]);
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
            if (Session["locationId"] != null)
            {
                int locationId = Convert.ToInt32(Session["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                }
            }
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            if (Session["categoryId"] != null)
            {
                int categoryId = Convert.ToInt32(Session["categoryId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindDataByCategory(db, categoryId);
                }
            }
        }

        protected void ButtonSubmit2_Click(object sender, EventArgs e)
        {
            if (Session["locationId"] != null)
            {
                int locationId = Convert.ToInt32(Session["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                }
            }
        }

        protected void ListViewContent2_ItemCommand(object sender, ListViewCommandEventArgs e)
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

        protected void DropDownListModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListModels.SelectedValue != String.Empty)
            {
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + DropDownListModels.SelectedValue));
            }
        }
    }
}