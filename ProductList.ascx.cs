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

namespace Cowrie.Modules.ProductList
{
    public partial class ProductList : PortalModuleBase
    {
        private int? PopulateTree(SelectedHotelsEntities db)
        {
            DNNTreeLocations.TreeNodes.Clear();
            var topLocations = from l in db.Locations
                                where !l.IsDeleted && l.ParentId == null
                                select l;
            foreach (Location location in topLocations)
            {
                DotNetNuke.UI.WebControls.TreeNode objNode = new DotNetNuke.UI.WebControls.TreeNode(location.Name);
                objNode.ToolTip = location.Name;
                objNode.ClickAction = eClickAction.PostBack;
                objNode.Key = location.Id.ToString();
                DNNTreeLocations.TreeNodes.Add(objNode);
                if (location.SubLocations.Any(l => !l.IsDeleted))
                {
                    objNode.HasNodes = true;
                    var subLocations = from l in location.SubLocations
                                        where !l.IsDeleted
                                        select l;
                    foreach (Location subLocation in subLocations)
                    {
                        int index = objNode.TreeNodes.Add();
                        objNode = objNode.TreeNodes[index];
                        objNode.Text = subLocation.Name;
                        objNode.ToolTip = subLocation.Name;
                        objNode.ClickAction = eClickAction.PostBack;
                        objNode.Key = subLocation.Id.ToString();
                    }
                }
            }
            if (topLocations.Count() > 0)
            {
                return topLocations.First().Id;
            }
            return null;
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
            IList<Product> products = (from p in db.Products
                        where !p.IsDeleted && p.ProductTypeId == 1
                        select p).ToList();
            IList<Hotel> hotels = products.Cast<Hotel>().ToList();
            DataListContent.DataSource = hotels.Where(h =>  h.LocationId == locationId).ToList();
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