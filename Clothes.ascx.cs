// Copyright (c) 2012 Cowrie

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Entities.Modules.Definitions;
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
                case "UpdateCategoriesTabs":
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        var parentCategories =
                            db.MerchantCategories.Where(mc => mc.ParentId == null).OrderBy(mc => mc.Name);
                        TabController tabController = new TabController();
                        foreach (var parentCategory in parentCategories)
                        {
                            TabInfo tab = tabController.GetTabByName(parentCategory.Name, PortalId, -1);
                            if (tab == null)
                            {
                                tab = CreateTab(parentCategory.Name);
                                //Clear Cache
                                DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(tab.TabID);
                                var childCategories =
                                    db.MerchantCategories.Where(mc => mc.ParentId == parentCategory.Id)
                                        .OrderBy(mc => mc.Name);
                                foreach (MerchantCategory merchantCategory in childCategories)
                                {
                                    TabInfo childTab = tabController.GetTabByName(merchantCategory.Name, PortalId, tab.TabID);
                                    if (childTab == null)
                                    {
                                        childTab = CreateSubTab(merchantCategory.Name, merchantCategory.Id,
                                            "merchantcategory", tab.TabID);
                                        //Clear Cache
                                        DotNetNuke.Common.Utilities.DataCache.ClearModuleCache(childTab.TabID);
                                    }
                                }
                            }
                        }

                        var portalTabs = tabController.GetTabsByPortal(PortalId);
                        foreach (KeyValuePair<int, TabInfo> pair in portalTabs)
                        {
                            if (!parentCategories.Any(pc => pc.Name == pair.Value.TabName && !pair.Value.TabPath.StartsWith("//Admin") && !pair.Value.TabPath.StartsWith("//Host")))
                            {
                                DeleteTab(pair.Value.TabName);
                            }
                        }
                    }

                    //Clear Cache
                    DotNetNuke.Common.Utilities.DataCache.ClearTabsCache(PortalId);
                    DotNetNuke.Common.Utilities.DataCache.ClearPortalCache(PortalId, false);

                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Categories Tabs Updated", ModuleMessage.ModuleMessageType.GreenSuccess);
                    break;
            }
        }

        private void DeleteTab(string tabName, bool isSuperTab = true)
        {
            TabController tabController = new TabController();
            TabInfo tab = tabController.GetTabByName(tabName, PortalId);
            tabController.DeleteTab(tab.TabID, PortalId, true);
        }
        private void DeleteSubTab(string tabName, int parentId)
        {
            TabController tabController = new TabController();
            TabInfo tab = tabController.GetTabByName(tabName, PortalId, parentId);
            tabController.DeleteTab(tab.TabID, PortalId);
        }
        private TabInfo CreateTab(string tabName, bool isSuperTab = true)
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

            //if this tab has any parents provide parent tab id, so that it will be shown in parent tab menus sub menu list, else is NULL
            //and will be in main menu list
            if (!isSuperTab)
                tab.ParentId = TabId;
            tab.IsDeleted = false;
            tab.Url = "";
            //if true, it has no parents, else false
            tab.IsSuperTab = isSuperTab; 
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

            return tab;
        }

        private TabInfo CreateSubTab(string tabName, int departmentId, string settingsName = "department", int? parentTabID = null)
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

            //if this tab has any parents provide parent tab id, so that it will be shown in parent tab menus sub menu list, else is NULL
            //and will be in main menu list
            if (parentTabID == null)
            {
                tab.ParentId = TabId;
            }
            else
            {
                tab.ParentId = parentTabID.Value;
            }
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
            moduleInfo.ModuleSettings[settingsName] = departmentId;

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

            ModuleController moduleController = new ModuleController();
            ArrayList hotelListModules = moduleController.GetModulesByDefinition(PortalId, "Cloth Details");
            if (hotelListModules.Count > 0)
            {
                ModuleInfo module = hotelListModules.OfType<ModuleInfo>().First();
                DetailsTabId = module.TabID;
            }

            Session["ListTabId"] = TabId;

            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        if (Session["ReturnFromDetails"] != null)
                        {
                            Session["ReturnFromDetails"] = null;
                            LoadPersistentSettings();
                        }

                        RefreshSizesAndData(true);
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void InitFilters(SelectedHotelsEntities db, IQueryable<Cloth> clothes)
        {
            var categories = clothes
                .SelectMany(c => c.Categories)
                .Distinct()
                .OrderBy(c => c.Name);
            if (categories.Count() > 1)
            {
                PanelCategories.Visible = true;
                CheckBoxListCategories.DataSource = categories.ToList();
                CheckBoxListCategories.DataBind();
            }
            else
            {
                PanelCategories.Visible = false;
            }

            var genders = clothes
                .Select(c => c.Gender)
                .Distinct();
            if (genders.Count() > 1)
            {
                PanelGenders.Visible = true;
            }
            else
            {
                PanelGenders.Visible = false;
            }

            var colours = clothes
                .Select(c => c.Colour)
                .Distinct()
                .OrderBy(c => c.Name);
            if (colours.Count() > 1)
            {
                PanelColours.Visible = true;
                CheckBoxListColours.DataSource = colours.ToList();
                CheckBoxListColours.DataBind();
            }
            else
            {
                PanelColours.Visible = false;
            }

            var brands =
                db.Brands.Where(b => b.Clothes.Intersect(clothes).Any())
                    .OrderByDescending(b => b.Clothes.Count)
                    .Take(10)
                    .OrderBy(b => b.Name)
                    .ToList();
            if (brands.Count() > 1)
            {
                PanelBrands.Visible = true;
                CheckBoxListBrands.DataSource = brands;
                CheckBoxListBrands.DataBind();
            }
            else
            {
                PanelBrands.Visible = false;
            }

            var styles =
                clothes.SelectMany(c => c.Styles)
                    .Distinct()
                    .OrderByDescending(s => s.Clothes.Count)
                    .Take(10)
                    .OrderBy(s => s.Name)
                    .ToList();
            if (styles.Count > 1)
            {
                PanelStyles.Visible = true;
                CheckBoxListStyles.DataSource = styles;
                CheckBoxListStyles.DataBind();
            }
            else
            {
                PanelStyles.Visible = false;
            }
        }

        private void LoadPersistentSettings()
        {
            if (Session["search"] != null)
            {
                TextBoxSearch.Text = Session["search"].ToString();
            }
            if (Session["genders"] != null)
            {
                RenderFromSession(CheckBoxListGenders, "genders");
            }
            if (Session["colours"] != null)
            {
                RenderFromSession(CheckBoxListColours, "colours");
            }
            if (Session["brands"] != null)
            {
                RenderFromSession(CheckBoxListBrands, "brands");
            }
            if (Session["styles"] != null)
            {
                RenderFromSession(CheckBoxListStyles, "styles");
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
            }
            if (Session["pageSize"] != null)
            {
                int pageSize = Convert.ToInt32(Session["pageSize"]);
                DropDownListPageSizes.SelectedValue = pageSize.ToString();
                DataPagerContent.SetPageProperties(startRowIndex, pageSize, false);
            }
        }

        private void RenderFromSession(CheckBoxList checkBoxList, string name)
        {
            foreach (var value in Session[name].ToString().Split(';'))
            {
                checkBoxList.Items.FindByValue(value).Selected = true;
            }
        }

        private void SavePersistentSetting()
        {
            if (PanelCategories.Visible)
            {
                UpdateSession(CheckBoxListCategories, "categories");
            }
            if (PanelGenders.Visible)
            {
                UpdateSession(CheckBoxListGenders, "genders");
            }
            if (PanelColours.Visible)
            {
                UpdateSession(CheckBoxListColours, "colours");
            }
            if (PanelBrands.Visible)
            {
                UpdateSession(CheckBoxListBrands, "brands");
            }
            if (PanelStyles.Visible)
            {
                UpdateSession(CheckBoxListStyles, "styles");
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

        private void UpdateSession(CheckBoxList checkBoxList, string name)
        {
            List<string> values = new List<string>();
            foreach (ListItem item in checkBoxList.Items)
            {
                if (item.Selected)
                {
                    values.Add(item.Value);
                }
            }
            if (values.Any())
            {
                Session[name] = String.Join(";", values);
            }
            else
            {
                Session.Remove(name);
            }
        }

        private void BindSizes(SelectedHotelsEntities db, IQueryable<Cloth> clothes)
        {
            var sizes = clothes.SelectMany(c => c.Sizes).Distinct().OrderBy(s => s.Name).ToList();
            if (sizes.Count() > 1)
            {
                CheckBoxListSizes.DataSource = sizes.ToList();
                CheckBoxListSizes.DataBind();
            }
            else
            {
                PanelSizes.Visible = false;
            }
        }

        private IQueryable<Cloth> GetClothes(SelectedHotelsEntities db, int? departmentId = null, int? merchantCategoryId = null)
        {
            var clothes = db.Products.Where(p => !p.IsDeleted).OfType<Cloth>() as IQueryable<Cloth>;
            if (departmentId.HasValue)
            {
                clothes = clothes.Where(c => c.Departments.Any(d => d.Id == departmentId));
            }
            if (merchantCategoryId.HasValue)
            {
                clothes = clothes.Where(c => c.MerchantCategoryId == merchantCategoryId);
            }
            if (TextBoxSearch.Text != String.Empty)
            {
                clothes =
                    clothes.Where(
                        c =>
                        c.Name.ToLower().Contains(TextBoxSearch.Text.ToLower()) ||
                        c.Description.ToLower().Contains(TextBoxSearch.Text.ToLower()));
                LabelFilteredBy.Text = String.Format("Filtered by \"{0}\"", TextBoxSearch.Text);
                LabelFilteredBy.Visible = true;
                ButtonClear.Visible = true;
            }
            else
            {
                LabelFilteredBy.Visible = false;
                ButtonClear.Visible = false;
            }
            return clothes;
        }

        private IQueryable<Cloth> FilterClothes(IQueryable<Cloth> clothes)
        {
            IEnumerable<int> allCheckedCategories = from ListItem item in CheckBoxListCategories.Items
                                                where item.Selected
                                                select int.Parse(item.Value);
            if (allCheckedCategories.Any())
            {
                clothes = clothes.Where(c => allCheckedCategories.Any(cb => c.Categories.Any(cat => cat.Id == cb)));
            }
            IEnumerable<int> allCheckedBrands = from ListItem item in CheckBoxListBrands.Items
                where item.Selected
                select int.Parse(item.Value);
            if (allCheckedBrands.Any())
            {
                clothes = clothes.Where(c => allCheckedBrands.Any(cb => c.BrandId == cb));
            }
            IEnumerable<int> allCheckedStyles = from ListItem item in CheckBoxListStyles.Items
                where item.Selected
                select int.Parse(item.Value);
            if (allCheckedStyles.Any())
            {
                clothes = clothes.Where(c => allCheckedStyles.Any(cs => c.Styles.Any(s => s.Id == cs)));
            }
            IEnumerable<int> allCheckedGenders = from ListItem item in CheckBoxListGenders.Items
                where item.Selected
                select int.Parse(item.Value);
            if (allCheckedGenders.Any())
            {
                clothes = clothes.Where(c => allCheckedGenders.Any(cg => c.GenderId == cg));
            }
            IEnumerable<int> allCheckedColours = from ListItem item in CheckBoxListColours.Items
                where item.Selected
                select int.Parse(item.Value);
            if (allCheckedColours.Any())
            {
                clothes = clothes.Where(c => allCheckedColours.Any(cc => c.ColourId == cc));
            }
            return clothes;
        }

        private void BindData(IQueryable<Cloth> clothes)
        {
            List<int> selectedSizes = new List<int>();
            foreach (ListItem item in CheckBoxListSizes.Items)
            {
                if (item.Selected)
                {
                    selectedSizes.Add(int.Parse(item.Value));
                }
            }
            if (selectedSizes.Any())
            {
                clothes = from c in clothes
                    where c.Sizes.Any(cs => selectedSizes.Any(s => s == cs.Id))
                    select c;
            }

            switch (DropDownListSortCriterias.SelectedValue)
            {
                case "Position":
                    ListViewContent.DataSource = clothes.OrderByDescending(c => c.CustomerRating).ToList();
                    break;
                case "Name: A to Z":
                    ListViewContent.DataSource = clothes.OrderBy(c => c.Name).ToList();
                    break;
                case "Name: Z to A":
                    ListViewContent.DataSource = clothes.OrderByDescending(c => c.Name).ToList();
                    break;
                case "Price: Low to High":
                ListViewContent.DataSource = clothes.OrderBy(c => c.UnitCost).ToList();
                    break;
                case "Price: High to Low":
                    ListViewContent.DataSource = clothes.OrderByDescending(c => c.UnitCost).ToList();
                    break;
            }
            ListViewContent.DataBind();

            LabelCount.Text = clothes.Count().ToString();
        }

        #region IActionable Members

        public ModuleActionCollection ModuleActions
        {
            get
            {
                ModuleActionCollection actions = new ModuleActionCollection
                {
                    {
                        GetNextActionID(), "Update Categories Tabs", "UpdateCategoriesTabs", String.Empty,
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
            Session["sortCriteria"] = DropDownListSortCriterias.SelectedValue;

            RefreshData();
        }

        private void RefreshData()
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                IQueryable<Cloth> clothes = GetClothes(db);
                if (Settings["department"] != null)
                {
                    int departmentId = Convert.ToInt32(Settings["department"]);
                    LabelTitle.Text = db.Departments.Find(departmentId).Name;
                    clothes = GetClothes(db, departmentId);
                }
                else if (Settings["merchantcategory"] != null)
                {
                    int merchantCategoryId = Convert.ToInt32(Settings["merchantcategory"]);
                    LabelTitle.Text = db.MerchantCategories.Find(merchantCategoryId).FullName;
                    clothes = GetClothes(db, null, merchantCategoryId);
                }

                BindData(FilterClothes(clothes));
            }
        }

        protected void DropDownListPageSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            int pageSize = Convert.ToInt32(DropDownListPageSizes.SelectedValue);
            Session["pageSize"] = pageSize;

            DataPagerContent.PageSize = pageSize;
        }

        protected void ListViewContent_PagePropertiesChanging(object sender, PagePropertiesChangingEventArgs e)
        {
            Session["startRowIndex"] = e.StartRowIndex;

            //set current page startindex, max rows and rebind to false
            DataPagerContent.SetPageProperties(e.StartRowIndex, e.MaximumRows, false);

            RefreshData();
        }

        protected void CheckBoxListStyles_SelectedIndexChanged(object sender, EventArgs e)
        {
            SavePersistentSetting();
            RefreshSizesAndData();
        }
        protected void CheckBoxListColours_SelectedIndexChanged(object sender, EventArgs e)
        {
            SavePersistentSetting();
            RefreshSizesAndData();
        }
        protected void CheckBoxListGenders_SelectedIndexChanged(object sender, EventArgs e)
        {
            SavePersistentSetting();
            RefreshSizesAndData();
        }
        protected void CheckBoxListBrands_SelectedIndexChanged(object sender, EventArgs e)
        {
            SavePersistentSetting();
            RefreshSizesAndData();
        }

        private void RefreshSizesAndData(bool initFilters = false)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                IQueryable<Cloth> clothes = GetClothes(db);
                if (Settings["department"] != null)
                {
                    int departmentId = Convert.ToInt32(Settings["department"]);
                    LabelTitle.Text = db.Departments.Find(departmentId).Name;
                    clothes = GetClothes(db, departmentId);
                }
                else if (Settings["merchantcategory"] != null)
                {
                    int merchantCategoryId = Convert.ToInt32(Settings["merchantcategory"]);
                    LabelTitle.Text = db.MerchantCategories.Find(merchantCategoryId).FullName;
                    clothes = GetClothes(db, null, merchantCategoryId);
                }
                if (initFilters)
                {
                    InitFilters(db, clothes);
                }
                var filteredClothes = FilterClothes(clothes);
                BindSizes(db, filteredClothes);
                BindData(filteredClothes);
            }
        }

        protected void CheckBoxListSizes_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshData();
        }

        protected void CheckBoxListCategories_SelectedIndexChanged(object sender, EventArgs e)
        {
            SavePersistentSetting();
            RefreshSizesAndData();
        }

        protected void ButtonSubmit_Click(object sender, EventArgs e)
        {
            Session["search"] = TextBoxSearch.Text;

            DataPagerContent.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false);

            RefreshSizesAndData(true);
        }

        protected void ButtonClear_Click(object sender, EventArgs e)
        {
            TextBoxSearch.Text = String.Empty;
            Session.Remove("search");

            DataPagerContent.SetPageProperties(0, int.Parse(DropDownListPageSizes.SelectedValue), false);

            RefreshSizesAndData(true);
        }
    }
}