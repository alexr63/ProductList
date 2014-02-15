// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins.Controls;
using SelectedHotelsModel;

namespace Cowrie.Modules.ProductList
{
    public partial class Clothes : PortalModuleBase, IActionable
    {
        public int DetailsTabId { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            AddActionHandler(new ActionEventHandler(MyActions_Click));
        }
        private void MyActions_Click(object sender, DotNetNuke.Entities.Modules.Actions.ActionEventArgs e)
        {
            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, e.Action.CommandName, ModuleMessage.ModuleMessageType.BlueInfo);

            switch (e.Action.CommandName.ToUpper())
            {
                case "REDIRECT":
                    if (e.Action.CommandArgument.ToUpper() != "CANCEL")
                    {
                        Response.Redirect(e.Action.Url);
                    }
                    else
                    {
                        DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Canceled the Redirect", ModuleMessage.ModuleMessageType.YellowWarning);
                    }
                    break;
            }
        }
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
                        var brands = db.Brands.OrderByDescending(b => b.Clothes.Count).Take(10).OrderBy(b => b.Name).ToList();
                        CheckBoxListBrands.DataSource = brands;
                        CheckBoxListBrands.DataBind();
                        DropDownListMerchantCategories.DataSource =
                            db.MerchantCategories.OrderBy(mc => mc.Name).ToList();
                        DropDownListMerchantCategories.DataBind();
                        DropDownListStyles.DataSource = db.Styles.OrderBy(s => s.Name).ToList();
                        DropDownListStyles.DataBind();
                        var colours = db.Products.Where(p => !p.IsDeleted).OfType<Cloth>()
                            .Select(c => c.Colour)
                            .Distinct()
                            .OrderBy(c => c);
                        CheckBoxListColours.DataSource = colours.ToList();
                        CheckBoxListColours.DataBind();
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
            IEnumerable<int> allCheckedBrands = from ListItem item in CheckBoxListBrands.Items
                                                    where item.Selected
                                                    select int.Parse(item.Value);
            if (allCheckedBrands.Any())
            {
                query = query.Where(c => allCheckedBrands.Any(cb => c.BrandId == cb));
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
            IEnumerable<String> allCheckedGenders = from ListItem item in CheckBoxListGenders.Items
                where item.Selected
                select item.Value;
            if (allCheckedGenders.Any())
            {
                query = query.Where(c => allCheckedGenders.Any(cg => c.Gender == cg));
            }
            IEnumerable<String> allCheckedColours = from ListItem item in CheckBoxListColours.Items
                                                    where item.Selected
                                                    select item.Value;
            if (allCheckedColours.Any())
            {
                query = query.Where(c => allCheckedColours.Any(cc => c.Colour == cc));
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
            IEnumerable<int> allCheckedBrands = from ListItem item in CheckBoxListBrands.Items
                                                where item.Selected
                                                select int.Parse(item.Value);
            if (allCheckedBrands.Any())
            {
                query = query.Where(c => allCheckedBrands.Any(cb => c.BrandId == cb));
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
            IEnumerable<String> allCheckedGenders = from ListItem item in CheckBoxListGenders.Items
                                                    where item.Selected
                                                    select item.Value;
            if (allCheckedGenders.Any())
            {
                query = query.Where(c => allCheckedGenders.Any(cg => c.Gender == cg));
            }
            IEnumerable<String> allCheckedColours = from ListItem item in CheckBoxListColours.Items
                                                    where item.Selected
                                                    select item.Value;
            if (allCheckedColours.Any())
            {
                query = query.Where(c => allCheckedColours.Any(cc => c.Colour == cc));
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
        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection();
                ModuleAction urlEventAction = new ModuleAction(ModuleContext.GetNextActionID());
                urlEventAction.Title = "Action Event Example";
                urlEventAction.CommandName = "redirect";
                urlEventAction.CommandArgument = "cancel";
                urlEventAction.Url = "http://dotnetnuke.com";
                urlEventAction.UseActionEvent = true;
                urlEventAction.Secure = DotNetNuke.Security.SecurityAccessLevel.Admin;
                actions.Add(urlEventAction);
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
        protected void CheckBoxListColours_SelectedIndexChanged(object sender, EventArgs e)
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

        protected void CheckBoxListGenders_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindSizes(db);
                BindData(db);
            }
        }
        protected void CheckBoxListBrands_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindSizes(db);
                BindData(db);
            }
        }
    }
}