﻿using System;
using System.Configuration;
using System.Data;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;
using SelectedHotelsModel;
using Telerik.Web.UI;

namespace Cowrie.Modules.ProductList
{
    public partial class ClothesSettings : ModuleSettingsBase
    {

        /// <summary>
        /// handles the loading of the module setting for this
        /// control
        /// </summary>
        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        DropDownListDepartments.DataSource = db.Departments.OrderBy(s => s.Name).ToList();
                        DropDownListDepartments.DataBind();

                        object setting = Settings["category"];
                        int? categoryId = null;
                        if (setting != null)
                        {
                            categoryId = Convert.ToInt32(setting);
                            var category = db.Categories.SingleOrDefault(c => c.Id == categoryId);
                            LabelCurrentCategory.Text = category.Name;
                        }
                        PopulateTree(RadTreeViewCategories, db, null, categoryId);

                        setting = Settings["department"];
                        if (setting != null)
                        {
                            int departmentId = Convert.ToInt32(setting);
                            DropDownListDepartments.SelectedValue = departmentId.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>
        /// handles updating the module settings for this control
        /// </summary>
        public override void UpdateSettings()
        {
            try
            {
                ModuleController controller = new ModuleController();
                controller.UpdateModuleSetting(ModuleId, "category", RadTreeViewCategories.SelectedValue);
                if (DropDownListDepartments.SelectedValue != String.Empty)
                {
                    controller.UpdateModuleSetting(ModuleId, "department", DropDownListDepartments.SelectedValue);
                }
                else
                {
                    controller.DeleteModuleSetting(ModuleId, "department");
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }
 
        protected void RadTreeViewCategories_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? categoryId = Convert.ToInt32(e.Node.Value);
                var category = db.Categories.SingleOrDefault(c => c.Id == categoryId);
                CreateSubLocationNodes(category, e.Node, categoryId);
            }
            e.Node.Expanded = true;
        }

        public static int? PopulateTree(RadTreeView radTreeView, SelectedHotelsEntities db, int? categoryId = null, int? selectedCategoryId = null)
        {
            radTreeView.Nodes.Clear();
            IOrderedQueryable<Category> topCategories = from c in db.Categories
                                                        where !c.IsDeleted &&
                                                              (categoryId == null
                                                                   ? c.ParentId == null
                                                                   : c.ParentId == categoryId)
                                                        orderby c.Name
                                                        select c;
            foreach (Category category in topCategories)
            {
                RadTreeNode node = new RadTreeNode();
                node.Text = category.Name;
                node.ToolTip = category.Name;
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                node.Value = category.Id.ToString();
                if (selectedCategoryId != null && category.Id == selectedCategoryId)
                {
                    node.Selected = true;
                }
                radTreeView.Nodes.Add(node);
            }
            if (topCategories.Any())
            {
                return topCategories.First().Id;
            }
            return null;
        }

        public static void CreateSubLocationNodes(Category category, RadTreeNode objNode, int? selectedCategoryId)
        {
            if (category.SubCategories.Any(c => !c.IsDeleted))
            {
                var subCategories = from c in category.SubCategories
                                   where !c.IsDeleted
                                   orderby c.Name
                                   select c;
                foreach (Category subCategory in subCategories)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Text = subCategory.Name;
                    node.ToolTip = subCategory.Name;
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                    node.Value = subCategory.Id.ToString();
                    if (selectedCategoryId != null && category.Id == selectedCategoryId)
                    {
                        node.Selected = true;
                    }
                    objNode.Nodes.Add(node);
                }
            }
        }
    }
}