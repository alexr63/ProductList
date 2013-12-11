using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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

        public static string TruncateAtWord(this string input, int length)
        {
            if (input == null || input.Length < length)
                return input;
            int iNextSpace = input.LastIndexOf(" ", length);
            return String.Format("{0}...", input.Substring(0, (iNextSpace > 0) ? iNextSpace : length).Trim());
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

        public static int? PopulateLocationTree(RadTreeView radTreeView, SelectedHotelsEntities db, int? locationId = null, int? selectedLocationId = null, bool createSubLocationNodes = false)
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
                RadTreeNode node = new RadTreeNode();
                node.Text = location.Name;
                node.ToolTip = location.Name;
                if (location.SubLocations.Any(l => !l.IsDeleted))
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
                    CreateSubLocationNodes(location, node, selectedLocationId);
                    node.Expanded = true;
                }
            }
            if (topLocations.Any())
            {
                return topLocations.First().Id;
            }
            return null;
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
                    if (subLocation.SubLocations.Any(l => !l.IsDeleted))
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

        public static IEnumerable<Hotel> HotelsInLocation(SelectedHotelsEntities db, int locationId)
        {
            IList<Hotel> hotels = (from p in db.Products
                                   where !p.IsDeleted
                                   select p).OfType<Hotel>().ToList();
            var query = from h in hotels
                        where h.LocationId == locationId || h.Location.ParentId == locationId ||
                              (h.Location.ParentLocation != null && h.Location.ParentLocation.ParentId == locationId)
                        select h;
            return query;
        }

        public static string GetCurrencySymbol(string currencyCode)
        {
            foreach (CultureInfo nfo in CultureInfo.GetCultures(CultureTypes.SpecificCultures))
            {
                RegionInfo region = new RegionInfo(nfo.LCID);
                if (region.ISOCurrencySymbol == currencyCode)
                {
                    return region.CurrencySymbol;
                }
            }
            return currencyCode;
        }
    }
}