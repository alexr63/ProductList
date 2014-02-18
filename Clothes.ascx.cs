// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Modules.Definitions;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Security;
using DotNetNuke.Security.Permissions;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.Skins.Controls;
using SelectedHotelsModel;

namespace Cowrie.Modules.ProductList
{
    public partial class Clothes : PortalModuleBase, IActionable
    {
        private int categoryId = 4125;
        public int DetailsTabId { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
        }
        private void ModuleAction_Click(object sender, ActionEventArgs e)
        {
            switch (e.Action.CommandName)
            {
                case "CreateDepartmentsSubtabs":
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        foreach (Department department in db.Departments)
                        {
                            if (department.Name.StartsWith("All"))
                            {
                                var tab = CreateSubTab(department.Name, department.Id);
                                //Clear Cache
                                DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(tab.TabID);
                            }
                        }
                    }

                    //Clear Cache
                    DotNetNuke.Common.Utilities.DataCache.ClearTabsCache(PortalId);
                    DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(PortalId, false);

                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Departments Subtabs Created", ModuleMessage.ModuleMessageType.GreenSuccess);
                    break;
            }
        }

        private TabInfo CreateSubTab(string tabName, int departmentId)
        {
            //Create Tab
            TabController tabController = new TabController();

            TabInfo tab = new TabInfo();
            tab.PortalID = PortalId;
            tab.TabName = tabName;
            tab.Title = tabName;

            //works for include in menu option. if true, will be shown in menus, else will not be shown, we have to redirect internally
            tab.IsVisible = true;
            tab.DisableLink = false;

            //if this tab has any parents provide parent tab id, so that it will be shown in parent tab menus sub menu list, else is NULL         //and will be in main menu list
            tab.ParentId = TabId;
            tab.IsDeleted = false;
            tab.Url = "";
            tab.IsSuperTab = false; //if true, it has no parents, else false
            tab.SkinSrc = "[G]Skins/Artfolio001/page.ascx"; //provide skin src, else will take default skin
            tab.ContainerSrc = "[G]Containers/Artfolio001/Block.ascx";
            int tabId = tabController.AddTab(tab, true); //true to load defalut modules

            //Set Tab Permission
            TabPermissionController objTPC = new TabPermissionController();

            TabPermissionInfo tpi = new TabPermissionInfo();
            tpi.TabID = tabId;
            tpi.PermissionID = 3; //for view
            tpi.PermissionKey = "VIEW";
            tpi.PermissionName = "View Tab";
            tpi.AllowAccess = true;
            tpi.RoleID = 0; //Role ID of administrator         
            objTPC.AddTabPermission(tpi);

            TabPermissionInfo tpi1 = new TabPermissionInfo();
            tpi1.TabID = tabId;
            tpi1.PermissionID = 4; //for edit
            tpi1.PermissionKey = "EDIT";
            tpi1.PermissionName = "Edit Tab";
            tpi1.AllowAccess = true;
            tpi1.RoleID = 0; //Role ID of administrator       
            objTPC.AddTabPermission(tpi1);

            TabPermissionInfo tpi4 = new TabPermissionInfo();
            tpi4.TabID = tabId;
            tpi4.PermissionID = 3;
            tpi4.PermissionKey = "VIEW";
            tpi4.PermissionName = "View Tab";
            tpi4.AllowAccess = true;
            tpi4.RoleID = -1; //All users     
            objTPC.AddTabPermission(tpi4);

            //Add Module
            DesktopModuleController objDMC = new DesktopModuleController();
            DesktopModuleInfo desktopModuleInfo = objDMC.GetDesktopModuleByName("ProductList");

            ModuleDefinitionInfo moduleDefinitionInfo = new ModuleDefinitionInfo();
            ModuleInfo moduleInfo = new ModuleInfo();
            moduleInfo.PortalID = PortalId;
            moduleInfo.TabID = tabId;
            moduleInfo.ModuleOrder = 1;
            moduleInfo.ModuleTitle = "Product List";
            moduleInfo.PaneName = "Product List";
            moduleInfo.ModuleDefID = 160;
            moduleInfo.CacheTime = moduleDefinitionInfo.DefaultCacheTime; //Default Cache Time is 0
            moduleInfo.InheritViewPermissions = true; //Inherit View Permissions from Tab
            moduleInfo.AllTabs = false;
            moduleInfo.ModuleSettings["department"] = departmentId;

            ModuleController moduleController = new ModuleController();
            int moduleId = moduleController.AddModule(moduleInfo);

            //Set Module Permission
            ModulePermissionController objMPC = new ModulePermissionController();

            ModulePermissionInfo mpi1 = new ModulePermissionInfo();
            mpi1.ModuleID = moduleId;
            mpi1.PermissionID = 1; //View Permission
            mpi1.AllowAccess = true;
            mpi1.RoleID = 0; //Role ID of Administrator
            objMPC.AddModulePermission(mpi1);

            ModulePermissionInfo mpi2 = new ModulePermissionInfo();
            mpi2.ModuleID = moduleId;
            mpi2.PermissionID = 2; //Edit Permission
            mpi2.AllowAccess = true;
            mpi2.RoleID = 0; //Role ID of Administrator
            objMPC.AddModulePermission(mpi2);

            ModulePermissionInfo mpi5 = new ModulePermissionInfo();
            mpi5.ModuleID = moduleId;
            mpi5.PermissionID = 1; //View Permission
            mpi5.AllowAccess = true;
            mpi5.RoleID = 1; //Role ID of Registered User
            objMPC.AddModulePermission(mpi5);
            return tab;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            AddActionHandler(ModuleAction_Click);

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
            if (Settings["department"] != null)
            {
                int departmentId = Convert.ToInt32(Settings["department"]);
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
            if (Settings["department"] != null)
            {
                int departmentId = Convert.ToInt32(Settings["department"]);
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
                ModuleActionCollection actions = new ModuleActionCollection
                {
                    {
                        GetNextActionID(), "Create Departments Subtabs", "CreateDepartmentsSubtabs", String.Empty,
                        String.Empty,
                        String.Empty, true, SecurityAccessLevel.Admin, true, false
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