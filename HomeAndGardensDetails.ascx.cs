// Copyright (c) 2012 Cowrie

using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using ProductList;
using SelectedHotelsModel;

namespace Cowrie.Modules.ProductList
{
    public partial class HomeAndGardensDetails : PortalModuleBase
    {
        public HomeAndGarden product;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (IsPostBack) return;
                if (Request.QueryString["Id"] != null)
                {
                    int id = int.Parse(Request.QueryString["Id"]);
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        product = db.Products.Find(id) as HomeAndGarden;
                        if (product != null)
                        {
                            Repeater1.DataSource = product.ProductImages;
                            Repeater1.DataBind();
                        }
                        DataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }
    }
}