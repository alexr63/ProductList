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

        public static int? PopulateLocationTree(RadTreeView radTreeView, SelectedHotelsEntities db, int? locationId = null, int? selectedLocationId = null, bool createSubLocationNodes = false, int? hotelTypeId = null)
        {
            radTreeView.Nodes.Clear();
            IOrderedQueryable<Location> topLocations;
            if (locationId == null)
            {
                topLocations = from l in db.Locations
                               where !l.IsDeleted &&
                                     l.ParentId == null
                               orderby l.Name
                               select l;
            }
            else
            {
                topLocations = from l in db.Locations
                               where !l.IsDeleted &&
                                     l.Id == locationId
                               orderby l.Name
                               select l;
            }
            foreach (Location location in topLocations)
            {
                if (!Utils.AnyHotelInLocation(db, location.Id, hotelTypeId))
                {
                    continue;
                }
                RadTreeNode node = new RadTreeNode();
                node.Text = location.Name;
                node.ToolTip = location.Name;
                if (location.SubLocations.Any(l => !l.IsDeleted && AnyHotelInLocation(db, l.Id, hotelTypeId)))
                {
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                }
                node.Value = location.Id.ToString();
#if MULTIPLESELECTION
                if (selectedLocations.Any(sl => sl.Id == location.Id))
                {
                    node.Selected = true;
                }
#endif
                if (selectedLocationId != null && location.Id == selectedLocationId)
                {
                    node.Selected = true;
                }
                radTreeView.Nodes.Add(node);
                if (createSubLocationNodes && location.ParentId == null)
                {
                    CreateSubLocationNodes(db, location, node, selectedLocationId, hotelTypeId);
                    node.Expanded = true;
                }
            }
            if (topLocations.Any())
            {
                return topLocations.First().Id;
            }
            return null;
        }

        public static void CreateSubLocationNodes(SelectedHotelsEntities db, Location location, RadTreeNode objNode, int? selectedLocationId, int? hotelTypeId = null)
        {
            if (location.SubLocations.Any(l => !l.IsDeleted) && AnyHotelInLocation(db, location.Id, hotelTypeId))
            {
                var subLocations = from l in location.SubLocations
                                   where !l.IsDeleted
                                   orderby l.Name
                                   select l;
                foreach (Location subLocation in subLocations)
                {
                    if (!AnyHotelInLocation(db, subLocation.Id, hotelTypeId))
                    {
                        continue;
                    }
                    RadTreeNode node = new RadTreeNode();
                    node.Text = subLocation.Name;
                    node.ToolTip = subLocation.Name;
                    if (subLocation.SubLocations.Any(l => !l.IsDeleted) && AnyHotelInLocation(db, location.Id, hotelTypeId))
                    {
                        node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                    }
                    node.Value = subLocation.Id.ToString();
                    objNode.Nodes.Add(node);
#if MULTIPLESELECTION
                    if (selectedLocations.Any(sl => sl.Id == subLocation.Id))
                    {
                        node.Selected = true;
                    }
#endif
                    if (selectedLocationId != null && subLocation.Id == selectedLocationId)
                    {
                        node.Selected = true;
                    }
                }
            }
        }

        public static bool AnyHotelInLocation(SelectedHotelsEntities db, int locationId, int? hotelTypeId)
        {
            return db.HotelLocations.Any(hl => hl.LocationId == locationId && (hl.HotelTypeId == hotelTypeId));
        }
    }
}