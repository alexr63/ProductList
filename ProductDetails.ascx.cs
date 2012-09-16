// Copyright (c) 2012 Cowrie

using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using ProductList;

namespace Cowrie.Modules.ProductList
{
    public partial class ProductDetails : PortalModuleBase
    {
        public Product product;

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
                        product = db.Products.Find(id);
                        if (product != null)
                        {
                            singleItemLink.ImageUrl = product.URL;

                            ImageList.DataSource = product.ProductImages;
                            ImageList.DataBind();
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