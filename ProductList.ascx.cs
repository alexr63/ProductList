// Copyright (c) 2012 Cowrie

using System;
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
    public partial class ProductList : PortalModuleBase, IActionable
    {
        private void PopulateTree(SelectedHotelsEntities db)
        {
            DNNTreeCategories.TreeNodes.Clear();
            var topCategories = from c in db.Categories
                                where !c.IsDeleted && c.PortalId == PortalId && c.ParentId == null
                                select c;
            foreach (Category category in topCategories)
            {
                DotNetNuke.UI.WebControls.TreeNode objNode = new DotNetNuke.UI.WebControls.TreeNode(category.Name);
                objNode.ToolTip = category.Name;
                objNode.ClickAction = eClickAction.PostBack;
                objNode.Key = category.Id.ToString();
                DNNTreeCategories.TreeNodes.Add(objNode);
                if (category.SubCategories.Any(c => !c.IsDeleted))
                {
                    objNode.HasNodes = true;
                    var subCategories = from c in category.SubCategories
                                        where !c.IsDeleted
                                        select c;
                    foreach (Category subCategory in subCategories)
                    {
                        int index = objNode.TreeNodes.Add();
                        objNode = objNode.TreeNodes[index];
                        objNode.Text = subCategory.Name;
                        objNode.ToolTip = subCategory.Name;
                        objNode.ClickAction = eClickAction.PostBack;
                        objNode.Key = subCategory.Id.ToString();
                    }
                }
            }
        }

        private void PopulateChildrenTreeNodes(DotNetNuke.UI.WebControls.TreeNode objParent)
        {
            int index = 0;
            DotNetNuke.UI.WebControls.TreeNode objTreeNode = new DotNetNuke.UI.WebControls.TreeNode();
            index = objParent.TreeNodes.Add();
            objTreeNode = objParent.TreeNodes[index];
            objTreeNode.Text = "Super-Simple Module (DAL+)";
            objTreeNode.NavigateUrl = "http://www.adefwebserver.com/DotNetNukeHELP/DNN_ShowMeThePages/";
            objTreeNode.ClickAction = eClickAction.Navigate;
            index = objParent.TreeNodes.Add();
            objTreeNode = objParent.TreeNodes[index];
            index += 1;
            objTreeNode.Text = "Super-Fast Super-Easy Module (DAL+)";
            objTreeNode.NavigateUrl = "http://www.adefwebserver.com/DotNetNukeHELP/DNN_Things4Sale/";
            objTreeNode.ClickAction = eClickAction.Navigate;
            index = objParent.TreeNodes.Add();
            objTreeNode = objParent.TreeNodes[index];
            index += 1;
            objTreeNode.Text = "Create a full complete Module";
            objTreeNode.NavigateUrl = "http://www.adefwebserver.com/DotNetNukeHELP/DNN_Module4/";
            objTreeNode.ClickAction = eClickAction.Navigate;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        PopulateTree(db);

                        var query = from p in db.Products
                                    where !p.IsDeleted && p.Categories.Any(c => c.PortalId == PortalId)
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

        protected void DNNTreeCategories_NodeClick(object source, DNNTreeNodeClickEventArgs e)
        {
            int categoryId = int.Parse(e.Node.Key);
        }
    }
}