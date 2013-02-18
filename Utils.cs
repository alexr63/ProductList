using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.UI.WebControls;
using TreeNode = DotNetNuke.UI.WebControls.TreeNode;

namespace ProductList
{
    public static class Utils
    {
        public static int? PopulateTree(DnnTree DNNTreeLocations, SelectedHotelsEntities db, int? locationId = null, int? selectedLocationId = null)
        {
            DNNTreeLocations.TreeNodes.Clear();
            IOrderedQueryable<Location> topLocations;
            if (locationId == null)
            {
                topLocations = from l in db.Locations
                               where !l.IsDeleted && l.ParentId == null
                               orderby l.Name
                               select l;
            }
            else
            {
                topLocations = from l in db.Locations
                               where !l.IsDeleted && l.ParentId == locationId
                               orderby l.Name
                               select l;
            }
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
    }
}