// Copyright (c) 2012 Cowrie

using System;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Common;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using ProductList;

namespace Cowrie.Modules.ProductList
{
    public partial class ProductList : PortalModuleBase, IActionable
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        var query = from p in db.Products
                                    where p.Categories.Any(c => c.PortalId == PortalId)
                                    select p;
                        DataListContent.DataSource = query.ToList();
                        DataListContent.DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        #region IActionable Members

        public DotNetNuke.Entities.Modules.Actions.ModuleActionCollection ModuleActions
        {
            get
            {
                //create a new action to add an item, this will be added to the controls
                //dropdown menu
                ModuleActionCollection actions = new ModuleActionCollection
                                                     {
                                                         {
                                                             GetNextActionID(), "Import Products", "Import.Products",
                                                             String.Empty, String.Empty,
                                                             Globals.NavigateURL(TabId, "ImportProducts", "mid=" + ModuleId,
                                                                                 "pid=" + PortalId),
                                                             false, SecurityAccessLevel.Admin, true, false
                                                             }
                                                     };

                return actions;
            }
        }

        #endregion

        protected void DataListContent_DataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item ||
                e.Item.ItemType == ListItemType.AlternatingItem)
            {
            }
        }
    }
}