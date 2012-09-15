using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using DotNetNuke.Common.Lists;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Localization;
using DotNetNuke.Services.Log.EventLog;
using ProductList;

namespace Cowrie.Modules.ProductList
{
    public partial class ImportProducts : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    var ctlEntry = new ListController();
                    var entryCollection = ctlEntry.GetListEntryInfoItems("Country");
                    cboCountry.DataSource = entryCollection;
                    cboCountry.DataBind();
                    cboCountry.Items.Insert(0, new ListItem("Please Select"));
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        static IEnumerable<XElement> StreamRootChildDoc(string uri)
        {
            using (XmlReader reader = XmlReader.Create(uri))
            {
                reader.MoveToContent();
                // Parse the file and display each of the nodes.
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == "hotel")
                            {
                                XElement el = XElement.ReadFrom(reader) as XElement;
                                if (el != null)
                                    yield return el;
                            }
                            break;
                    }
                }
            }
        }

        protected void ButtonImport_Click(object sender, EventArgs e)
        {
            EventLogController eventLogController = new EventLogController();

            string categoryRootName = "Hotels";
            string countryFilter = cboCountry.SelectedItem.Text;

            try
            {
                if (ctlProductsFile.Url.StartsWith("FileID="))
                {
                    int fileId = int.Parse(ctlProductsFile.Url.Substring(7));
                    FileController fileController = new FileController();
                    FileInfo fileInfo = fileController.GetFileById(fileId, PortalId);
                    if (fileInfo.FileName != String.Empty)
                    {
                        string xmlFilename = PortalSettings.HomeDirectoryMapPath + fileInfo.Folder + fileInfo.FileName;

                        string schemaFilename = Server.MapPath("~/DesktopModules/ProductList/Hotels_Standard.xsd");
                        PortalTemplateValidator xval = new PortalTemplateValidator();
                        if (!xval.Validate(xmlFilename, schemaFilename))
                        {
                            foreach (var error in xval.Errors)
                            {
                                LogInfo logInfo = new LogInfo();
                                logInfo.LogPortalID = PortalId;
                                logInfo.LogPortalName = PortalSettings.PortalName;
                                logInfo.LogUserID = UserId;
                                logInfo.LogTypeKey = "ADMIN_ALERT";
                                logInfo.AddProperty("XMLError", error.ToString());
                                eventLogController.AddLog(logInfo);
                            }
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "There are errors in XML file",
                                                                      DotNetNuke.UI.Skins.Controls.ModuleMessage.
                                                                          ModuleMessageType.RedError);
                            return;
                        }

                        // read hotels from XML
                        // use XmlReader to avoid huge file size dependence
                        var xmlProducts =
                            from el in StreamRootChildDoc(xmlFilename)
                            select new
                                {
                                    Country = (string) el.Element("hotel_country"),
                                    County = (string) el.Element("hotel_county"),
                                    City = (string) el.Element("hotel_city"),
                                    Number = (string) el.Element("hotel_ref"),
                                    Name = (string) el.Element("hotel_name"),
                                    Images = el.Element("images"),
                                    UnitCost = (decimal) el.Element("PricesFrom"),
                                    Description = (string) el.Element("hotel_description"),
                                    DescriptionHTML = (string) el.Element("alternate_description"),
                                    URL = (string) el.Element("hotel_link")
                                };

                        if (!String.IsNullOrEmpty(countryFilter))
                        {
                            xmlProducts = xmlProducts.Where(p => p.Country == countryFilter);
                        }

                        using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                        {
                            foreach (var xmlProduct in xmlProducts)
                            {
                                var xmlProduct1 = xmlProduct;
                                Product product =
                                    db.Products.SingleOrDefault(
                                        p => p.Number == xmlProduct1.Number && p.CreatedByUser == UserId);
                                if (product == null)
                                {
                                    product = new Product
                                        {
                                            Name = xmlProduct1.Name,
                                            Number = xmlProduct1.Number,
                                            UnitCost = xmlProduct1.UnitCost,
                                            Description = xmlProduct1.Description,
                                            URL = xmlProduct.URL.Replace("[[PARTNERID]]", "2248").Trim(' '),
                                            Image = (string) xmlProduct1.Images.Element("url"),
                                            CreatedByUser = UserId,
                                            IsDeleted = false
                                        };
                                    db.Products.Add(product);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    bool isChanged = false;
                                    if (product.Name != xmlProduct1.Name)
                                    {
                                        product.Name = xmlProduct1.Name;
                                        isChanged = true;
                                    }
                                    if (product.Number != xmlProduct1.Number)
                                    {
                                        product.Number = xmlProduct1.Number;
                                        isChanged = true;
                                    }
                                    if (product.UnitCost != xmlProduct1.UnitCost)
                                    {
                                        product.UnitCost = xmlProduct1.UnitCost;
                                        isChanged = true;
                                    }
                                    if (product.Description != xmlProduct1.Description)
                                    {
                                        product.Description = xmlProduct1.Description;
                                        isChanged = true;
                                    }
                                    if (product.URL != xmlProduct1.URL.Replace("[[PARTNERID]]", "2248").Trim(' '))
                                    {
                                        product.URL = xmlProduct1.URL.Replace("[[PARTNERID]]", "2248").Trim(' ');
                                        isChanged = true;
                                    }
                                    if (product.Image != (string) xmlProduct1.Images.Element("url"))
                                    {
                                        product.Image = (string) xmlProduct1.Images.Element("url");
                                        isChanged = true;
                                    }
                                    if (isChanged)
                                    {
                                        db.SaveChanges();
                                    }
                                }

                                var catergoryRoot = db.Categories.SingleOrDefault(
                                    c =>
                                    c.PortalId == PortalId && c.Name == categoryRootName);
                                if (catergoryRoot == null)
                                {
                                    catergoryRoot = new Category
                                        {PortalId = PortalId, Name = categoryRootName, IsDeleted = false};
                                    db.Categories.Add(catergoryRoot);
                                    db.SaveChanges();
                                }
                                if (product.Categories.SingleOrDefault(c => c.Id == catergoryRoot.Id) == null)
                                {
                                    product.Categories.Add(catergoryRoot);
                                    db.SaveChanges();
                                }

                                int parentId = catergoryRoot.Id;
                                if (!String.IsNullOrEmpty(xmlProduct1.Country))
                                {
                                    Category categoryCountry =
                                        db.Categories.SingleOrDefault(
                                            c =>
                                            c.PortalId == PortalId && c.Name == xmlProduct1.Country &&
                                            c.ParentId == parentId);
                                    if (categoryCountry == null)
                                    {
                                        categoryCountry = new Category
                                            {
                                                PortalId = PortalId,
                                                Name = xmlProduct1.Country,
                                                ParentId = parentId,
                                                IsDeleted = false
                                            };
                                        db.Categories.Add(categoryCountry);
                                        db.SaveChanges();
                                    }
                                    if (product.Categories.SingleOrDefault(c => c.Id == categoryCountry.Id) == null)
                                    {
                                        product.Categories.Add(categoryCountry);
                                        db.SaveChanges();
                                    }
                                }

                                product.ProductImages.Clear();
                                db.SaveChanges();

                                foreach (var image in xmlProduct1.Images.Elements("url"))
                                {
                                    if (!image.Value.Contains("/thumbnail/") && !image.Value.Contains("/detail/"))
                                    {
                                        ProductImage productImage = new ProductImage();
                                        productImage.URL = image.Value;
                                        product.ProductImages.Add(productImage);
                                    }
                                }
                                db.SaveChanges();
                            }
                        }
                    }
                }
                DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Products were imported successfully.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
            }
            catch (Exception ex)
            {
                LogInfo logInfo = new LogInfo();
                logInfo.LogPortalID = PortalId;
                logInfo.LogPortalName = PortalSettings.PortalName;
                logInfo.LogUserID = UserId;
                logInfo.LogTypeKey = "ADMIN_ALERT";
                logInfo.AddProperty("Message", "Username is missed");
                eventLogController.AddLog(logInfo);

                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }
    }
}