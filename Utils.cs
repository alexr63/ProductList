﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.UI.WebControls;
using Telerik.Web.UI;
using TreeNode = DotNetNuke.UI.WebControls.TreeNode;

namespace ProductList
{
    public static class Utils
    {
        public static string TruncateAtWord(this string input, int length)
        {
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            return String.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
        }

        public static int? PopulateLocationTree(DnnTree DNNTreeLocations, SelectedHotelsEntities db, int? locationId = null, int? selectedLocationId = null)
        {
            DNNTreeLocations.TreeNodes.Clear();
            IOrderedQueryable<Location> topLocations = from l in db.Locations
                                                       where
                                                           !l.IsDeleted &&
                                                           (locationId == null
                                                                ? l.ParentId == null
                                                                : l.ParentId == locationId)
                                                       orderby l.Name
                                                       select l;
            foreach (Location location in topLocations)
            {
                TreeNode objNode = new TreeNode(location.Name);
                objNode.ToolTip = location.Name;
                objNode.ClickAction = eClickAction.PostBack;
                objNode.Key = location.Id.ToString();
                if (selectedLocationId != null && location.Id == selectedLocationId)
                {
                    objNode.Selected = true;
                    objNode.Expand();
                }
                if (location.SubLocations.Any(l => !l.IsDeleted))
                {
                    objNode.HasNodes = true;
                }
                DNNTreeLocations.TreeNodes.Add(objNode);
                //CreateSubLocationNodes(location, objNode, selectedLocationId);
            }
            if (topLocations.Any())
            {
                return topLocations.First().Id;
            }
            return null;
        }

        public static void CreateSubLocationNodes(Location location, TreeNode objNode, int? selectedLocationId)
        {
            if (location.SubLocations.Any(l => !l.IsDeleted))
            {
                objNode.HasNodes = true;
                var subLocations = from l in location.SubLocations
                                   where !l.IsDeleted
                                   orderby l.Name
                                   select l;
                foreach (Location subLocation in subLocations)
                {
                    int index = objNode.TreeNodes.Add();
                    TreeNode objSubNode = objNode.TreeNodes[index];
                    objSubNode.Text = subLocation.Name;
                    objSubNode.ToolTip = subLocation.Name;
                    objSubNode.ClickAction = eClickAction.PostBack;
                    objSubNode.Key = subLocation.Id.ToString();
                    if (selectedLocationId != null && location.Id == selectedLocationId)
                    {
                        objSubNode.Selected = true;
                        objNode.Expand();
                    }
                    if (location.SubLocations.Any(l => !l.IsDeleted))
                    {
                        objNode.HasNodes = true;
                    }
                    //CreateSubLocationNodes(subLocation, objSubNode, selectedLocationId);
                }
            }
        }

        public static int? PopulateCategoryTree(DnnTree DNNTreeCategories, SelectedHotelsEntities db, int categoryId, int? selectedCategoryId = null)
        {
            DNNTreeCategories.TreeNodes.Clear();
            IOrderedQueryable<Category> topCategories = from c in db.Categories
                                                        where
                                                            !c.IsDeleted &&
                                                            c.Id == categoryId
                                                        orderby c.Name
                                                        select c;
            foreach (Category category in topCategories)
            {
                TreeNode objNode = new TreeNode(category.Name);
                objNode.ToolTip = category.Name;
                objNode.ClickAction = eClickAction.PostBack;
                objNode.Key = category.Id.ToString();
                if (selectedCategoryId != null && category.Id == selectedCategoryId)
                {
                    objNode.Selected = true;
                    objNode.Expand();
                }
                if (category.SubCategories.Any(l => !l.IsDeleted))
                {
                    objNode.HasNodes = true;
                }
                DNNTreeCategories.TreeNodes.Add(objNode);
                //CreateSubLocationNodes(location, objNode, selectedLocationId);
            }
            if (topCategories.Any())
            {
                return topCategories.First().Id;
            }
            return null;
        }

        public static void CreateSubCategoryNodes(Category category, TreeNode objNode, int? selectedCategoryId)
        {
            if (category.SubCategories.Any(l => !l.IsDeleted))
            {
                objNode.HasNodes = true;
                var subCategories = from c in category.SubCategories
                                   where !c.IsDeleted
                                   orderby c.Name
                                   select c;
                foreach (Category subCategory in subCategories)
                {
                    int index = objNode.TreeNodes.Add();
                    TreeNode objSubNode = objNode.TreeNodes[index];
                    objSubNode.Text = subCategory.Name;
                    objSubNode.ToolTip = subCategory.Name;
                    objSubNode.ClickAction = eClickAction.PostBack;
                    objSubNode.Key = subCategory.Id.ToString();
                    if (selectedCategoryId != null && category.Id == selectedCategoryId)
                    {
                        objSubNode.Selected = true;
                        objNode.Expand();
                    }
                    if (category.SubCategories.Any(c => !c.IsDeleted))
                    {
                        objNode.HasNodes = true;
                    }
                    //CreateSubCategoryNodes(subCategory, objSubNode, selectedCategoryId);
                }
            }
        }

        public static int? PopulateCategoryTree(RadTreeView radTreeView, SelectedHotelsEntities db, int? categoryId = null, int? selectedCategoryId = null)
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

        public static int? PopulateLocationTree(RadTreeView radTreeView, SelectedHotelsEntities db, int? locationId = null, int? selectedLocationId = null)
        {
            radTreeView.Nodes.Clear();
            IOrderedQueryable<Location> topLocations = from l in db.Locations
                                                       where !l.IsDeleted &&
                                                           (locationId == null
                                                                ? l.ParentId == null
                                                                : l.ParentId == locationId)
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

        public static void CreateSubLocationNodes(Location location, RadTreeNode objNode, int? selectedLocationId)
        {
            if (location.SubLocations.Any(c => !c.IsDeleted))
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
                }
            }
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