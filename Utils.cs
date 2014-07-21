using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Common;
using SelectedHotelsModel;
using Telerik.Web.UI;

namespace ProductList
{
    public static class Utils
    {
        public static void Raise(this RadTreeViewEventHandler eventToThrow, object sender, RadTreeNodeEventArgs args)
        {
            if (eventToThrow != null)
            {
                eventToThrow(sender, args);
            }
        }

        public static int? PopulateCategoryTree(RadTreeView radTreeView, SelectedHotelsEntities db, int? categoryId = null, int? selectedCategoryId = null)
        {
            radTreeView.Nodes.Clear();
            IOrderedQueryable<Category> topCategories;
            if (categoryId == null)
            {
                topCategories = from c in db.Categories
                                where !c.IsDeleted &&
                                      c.ParentId == null
                                orderby c.Name
                                select c;
            }
            else
            {
                topCategories = from c in db.Categories
                                where !c.IsDeleted &&
                                      c.Id == categoryId
                                orderby c.Name
                                select c;
            }
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

        public static void CreateSubCategoryNodes(Category category, RadTreeNode objNode, int? selectedCategoryId)
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