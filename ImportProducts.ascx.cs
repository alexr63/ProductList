using System;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.FileSystem;
using DotNetNuke.Services.Log.EventLog;

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
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void cmdImport_Click(object sender, EventArgs e)
        {
            EventLogController eventLogController = new EventLogController();

            try
            {
                if (ctlReferralsFile.Url.StartsWith("FileID="))
                {
                    int fileId = int.Parse(ctlReferralsFile.Url.Substring(7));
                    FileController fileController = new FileController();
                    FileInfo fileInfo = fileController.GetFileById(fileId, PortalId);
                    if (fileInfo.FileName != String.Empty)
                    {
                        string xmlFilename = PortalSettings.HomeDirectoryMapPath + fileInfo.Folder + fileInfo.FileName;

                        string schemaFilename = Server.MapPath("~/DesktopModules/ProductList/Components/referrals.xsd");
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
                            DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "There are errors in XML file", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.RedError);
                            return;
                        }

                        XDocument referralsXML = XDocument.Load(xmlFilename);
                        var _referrals = from r in referralsXML.Descendants("referral")
                                        select new
                                        {
                                            Name = r.Element("name").Value,
                                            WebSite = r.Element("website") == null ? String.Empty : r.Element("website").Value,
                                            Address = r.Element("address") == null ? String.Empty : r.Element("address").Value,
                                            City = r.Element("city") == null ? String.Empty : r.Element("city").Value,
                                            State = r.Element("state") == null ? String.Empty : r.Element("state").Value,
                                            Zip = r.Element("zip") == null ? String.Empty : r.Element("zip").Value,
                                            Priority = r.Element("priority") == null ? 0 : Convert.ToInt32(r.Element("priority").Value)
                                        };
                        using (MegottaDataContext db = new MegottaDataContext(ConfigurationManager.ConnectionStrings["SiteSqlServer"].ConnectionString))
                        {
                            foreach (var _referral in _referrals)
                            {
                                Referral referral = db.FindReferral(ModuleId, _referral.Name);
                                if (referral == null)
                                {
                                    referral = new Referral
                                                       {
                                                           ModuleID = ModuleId,
                                                           Name = _referral.Name,
                                                           WebSite = _referral.WebSite,
                                                           Address = _referral.Address,
                                                           City = _referral.City,
                                                           State = _referral.State,
                                                           Zip = _referral.Zip,
                                                           Priority = _referral.Priority
                                                       };
                                    db.Referrals.InsertOnSubmit(referral);
                                }
                                else
                                {
                                    referral.ModuleID = ModuleId;
                                    referral.WebSite = _referral.WebSite;
                                    referral.Address = _referral.Address;
                                    referral.City = _referral.City;
                                    referral.State = _referral.State;
                                    referral.Zip = _referral.Zip;
                                    referral.Priority = _referral.Priority;
                                }
                                db.SubmitChanges();
                            }
                        }
                    }
                    DotNetNuke.UI.Skins.Skin.AddModuleMessage(this, "Referrals were imported successfully.", DotNetNuke.UI.Skins.Controls.ModuleMessage.ModuleMessageType.GreenSuccess);
                }
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