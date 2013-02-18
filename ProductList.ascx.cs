// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;

namespace Cowrie.Modules.ProductList
{
    public partial class ProductList : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        int? locationId = null;
                        if (Settings["location"] != null)
                        {
                            locationId = Convert.ToInt32(Settings["location"]);
                        }
                        var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                        LabelLocation.Text = location.Name;
                        int? firstLocationId = Utils.PopulateTree(DNNTreeLocations, db, locationId);
                        if (firstLocationId.HasValue)
                        {
                            ViewState["locationId"] = firstLocationId;
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
                LabelSelectedLocation.Text = location.Name;
            }
            IList<Product> products = (from p in db.Products
                                       where !p.IsDeleted && p.ProductTypeId == (int)Enums.ProductTypeEnum.Hotels
                                       select p).ToList();
            IList<Hotel> hotels = products.Cast<Hotel>().ToList();
            var query = from h in hotels
                         where h.LocationId == locationId || h.Location.ParentId == locationId ||
                               (h.Location.ParentLocation != null && h.Location.ParentLocation.ParentId == locationId)
                         select h;
            if (DropDownListSortCriterias.SelectedValue == "Name")
            {
                ListViewContent.DataSource = query.OrderBy(h => h.Name).ToList();
            }
            else
            {
                ListViewContent.DataSource = query.OrderBy(h => h.UnitCost).ToList();
            }
            ListViewContent.DataBind();

            LabelCount.Text = query.Count().ToString();
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
            ViewState["locationId"] = locationId;
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                BindData(db, locationId);
            }
        }

        protected void DropDownListSortCriterias_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ViewState["locationId"] != null)
            {
                int locationId = Convert.ToInt32(ViewState["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindData(db, locationId);
                }
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
            if (ViewState["locationId"] != null)
            {
                int locationId = Convert.ToInt32(ViewState["locationId"]);
                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    BindData(db, locationId);
                }
            }
        }

        protected void DNNTreeLocations_PopulateOnDemand(object source, DotNetNuke.UI.WebControls.DNNTreeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? locationId = Convert.ToInt32(e.Node.Key);
                var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                Utils.CreateSubLocationNodes(location, e.Node, locationId);
            }
        }
    }
}