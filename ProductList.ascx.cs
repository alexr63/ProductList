// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;
using TreeNode = DotNetNuke.UI.WebControls.TreeNode;

namespace Cowrie.Modules.ProductList
{
    public partial class ProductList : PortalModuleBase
    {
        private int? PopulateTree(SelectedHotelsEntities db)
        {
            DNNTreeLocations.TreeNodes.Clear();
            var topLocations = from l in db.Locations
                                where !l.IsDeleted && l.ParentId == null && l.Id == 1
                                select l;
            foreach (Location location in topLocations)
            {
                DotNetNuke.UI.WebControls.TreeNode objNode = new DotNetNuke.UI.WebControls.TreeNode(location.Name);
                objNode.ToolTip = location.Name;
                objNode.ClickAction = eClickAction.PostBack;
                objNode.Key = location.Id.ToString();
                DNNTreeLocations.TreeNodes.Add(objNode);
                CreateSubLocationNodes(location, objNode);
            }
            if (topLocations.Any())
            {
                return topLocations.First().Id;
            }
            return null;
        }

        private static void CreateSubLocationNodes(Location location, TreeNode objNode)
        {
            if (location.SubLocations.Any(l => !l.IsDeleted))
            {
                objNode.HasNodes = true;
                var subLocations = from l in location.SubLocations
                                   where !l.IsDeleted
                                   select l;
                foreach (Location subLocation in subLocations)
                {
                    int index = objNode.TreeNodes.Add();
                    DotNetNuke.UI.WebControls.TreeNode objSubNode = objNode.TreeNodes[index];
                    objSubNode.Text = subLocation.Name;
                    objSubNode.ToolTip = subLocation.Name;
                    objSubNode.ClickAction = eClickAction.PostBack;
                    objSubNode.Key = subLocation.Id.ToString();
                    CreateSubLocationNodes(subLocation, objSubNode);
                }
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        int? firstLocationId = PopulateTree(db);
                        if (firstLocationId.HasValue)
                        {
                            BindData(db, firstLocationId.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        private void BindData(SelectedHotelsEntities db, int locationId)
        {
            var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
            if (location != null)
            {
                LabelLocation.Text = location.Name;
            }
            IList<Product> products = (from p in db.Products
                        where !p.IsDeleted && p.ProductTypeId == 1
                        select p).ToList();
            IList<Hotel> hotels = products.Cast<Hotel>().ToList();
            DataListContent.DataSource =
                hotels.Where(
                    h =>
                    h.LocationId == locationId || h.Location.ParentId == locationId ||
                    (h.Location.ParentLocation != null && h.Location.ParentLocation.ParentId == locationId)).ToList();
            DataListContent.DataBind();
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

        protected void DNNTreeLocations_NodeClick(object source, DNNTreeNodeClickEventArgs e)
        {
            int locationId = int.Parse(e.Node.Key);
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, locationId);
            }
        }
    }
}