// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using SelectedHotelsModel;

namespace Cowrie.Modules.ProductList
{
    public partial class Clothes : PortalModuleBase
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
                        DropDownListBrands.DataSource = db.Brands.OrderBy(b => b.Name).ToList();
                        DropDownListBrands.DataBind();
                        DropDownListMerchantCategories.DataSource =
                            db.MerchantCategories.OrderBy(mc => mc.Name).ToList();
                        DropDownListMerchantCategories.DataBind();
                        DropDownListStyles.DataSource = db.Styles.OrderBy(s => s.Name).ToList();
                        DropDownListStyles.DataBind();
                        var colours = db.Products.Where(p => !p.IsDeleted).OfType<Cloth>()
                            .Select(c => c.Colour)
                            .Distinct()
                            .OrderBy(c => c);
                        DropDownListColours.DataSource = colours.ToList();
                        DropDownListColours.DataBind();
                        DropDownListDepartments.DataSource = db.Departments.OrderBy(s => s.Name).ToList();
                        DropDownListDepartments.DataBind();
                        BindSizes(db);
                        BindData(db);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void BindSizes(SelectedHotelsEntities db)
        {
            var query = db.Products.Where(p => !p.IsDeleted).OfType<Cloth>() as IQueryable<Cloth>;
            if (DropDownListBrands.SelectedValue != String.Empty)
            {
                int brandId = int.Parse(DropDownListBrands.SelectedValue);
                query = query.Where(c => c.BrandId == brandId);
            }
            if (DropDownListMerchantCategories.SelectedValue != String.Empty)
            {
                int categoryId = int.Parse(DropDownListMerchantCategories.SelectedValue);
                query = query.Where(c => c.MerchantCategoryId == categoryId);
            }
            if (DropDownListStyles.SelectedValue != String.Empty)
            {
                int styleId = int.Parse(DropDownListStyles.SelectedValue);
                query = query.Where(c => c.Styles.Any(s => s.Id == styleId));
            }
            if (DropDownListGenders.SelectedValue != String.Empty)
            {
                query = query.Where(c => c.Gender == DropDownListGenders.SelectedValue);
            }
            if (DropDownListColours.SelectedValue != String.Empty)
            {
                query = query.Where(c => c.Colour == DropDownListColours.SelectedValue);
            }
            if (DropDownListDepartments.SelectedValue != String.Empty)
            {
                int departmentId = int.Parse(DropDownListDepartments.SelectedValue);
                query = query.Where(c => c.Departments.Any(d => d.Id == departmentId));
            }
            var clothSizes =
                db.ClothSizes.Where(cs => query.Any(c => c.Id == cs.ClothId))
                    .Select(c => c.Size)
                    .Distinct()
                    .OrderBy(cs => cs);

            CheckBoxListSizes.DataSource = clothSizes.ToList();
            CheckBoxListSizes.DataBind();
            CheckBoxListSizes.Items.Insert(0, new ListItem("All Sizes", String.Empty));
            CheckBoxListSizes.SelectedIndex = 0;
        }

        private void BindData(SelectedHotelsEntities db)
        {
            var query = db.Products.Where(p => !p.IsDeleted).OfType<Cloth>();
            if (DropDownListBrands.SelectedValue != String.Empty)
            {
                int brandId = int.Parse(DropDownListBrands.SelectedValue);
                query = query.Where(c => c.BrandId == brandId);
            }
            if (DropDownListMerchantCategories.SelectedValue != String.Empty)
            {
                int categoryId = int.Parse(DropDownListMerchantCategories.SelectedValue);
                query = query.Where(c => c.MerchantCategoryId == categoryId);
            }
            if (DropDownListStyles.SelectedValue != String.Empty)
            {
                int styleId = int.Parse(DropDownListStyles.SelectedValue);
                query = query.Where(c => c.Styles.Any(s => s.Id == styleId));
            }
            if (DropDownListGenders.SelectedValue != String.Empty)
            {
                query = query.Where(c => c.Gender == DropDownListGenders.SelectedValue);
            }
            if (DropDownListColours.SelectedValue != String.Empty)
            {
                query = query.Where(c => c.Colour == DropDownListColours.SelectedValue);
            }
            if (DropDownListDepartments.SelectedValue != String.Empty)
            {
                int departmentId = int.Parse(DropDownListDepartments.SelectedValue);
                query = query.Where(c => c.Departments.Any(d => d.Id == departmentId));
            }

            List<string> selectedSizes = new List<string>();
            if (!CheckBoxListSizes.Items[0].Selected)
            {
                foreach (ListItem item in CheckBoxListSizes.Items)
                {
                    if (item.Selected)
                    {
                        selectedSizes.Add(item.Value);
                    }
                }
                query = from c in query
                        where c.ClothSizes.Any(cs => selectedSizes.Any(s => s == cs.Size))
                        select c;
            }

            if (DropDownListSortCriterias.SelectedValue == "Name")
            {
                ListViewContent.DataSource = query.OrderBy(c => c.Name).ToList();
            }
            else
            {
                ListViewContent.DataSource = query.OrderBy(c => c.UnitCost).ToList();
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

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db);
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
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db);
            }
        }

        protected void ButtonSearch_Click(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db);
            }
        }

        protected void DropDownListBrands_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindSizes(db);
                BindData(db);
            }
        }

        protected void DropDownListMerchantCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindSizes(db);
                BindData(db);
            }
        }
        protected void DropDownListStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindSizes(db);
                BindData(db);
            }
        }
        protected void DropDownListGenders_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindSizes(db);
                BindData(db);
            }
        }
        protected void DropDownListColours_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindSizes(db);
                BindData(db);
            }
        }
        protected void DropDownListDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindSizes(db);
                BindData(db);
            }
        }
    }
}