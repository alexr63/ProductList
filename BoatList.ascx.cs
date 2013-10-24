// Copyright (c) 2012 Cowrie

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;
using Telerik.Web.UI;

namespace Cowrie.Modules.ProductList
{
    public partial class BoatList : PortalModuleBase
    {
        public int DetailsTabId { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var childTabs = TabController.GetTabsByParent(TabId, PortalId);
            if (childTabs.Count() > 0)
            {
                DetailsTabId = childTabs[0].TabID;
            }

            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        IList<Boat> boats = (from p in db.Products
                                             where !p.IsDeleted
                                             select p).OfType<Boat>().OrderBy(p => p.Name).ToList();
                        ListViewContent3.DataSource = boats;
                        ListViewContent3.DataBind();

                        DropDownListModels.DataSource = boats;
                        DropDownListModels.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #region IActionable Members

        #endregion

        protected void DropDownListModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DropDownListModels.SelectedValue != String.Empty)
            {
                Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + DropDownListModels.SelectedValue));
            }
        }
    }
}