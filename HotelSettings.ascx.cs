using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using SelectedHotelsModel;

namespace Cowrie.Modules.ProductList
{
    public partial class HotelSettings : ModuleSettingsBase
    {

        /// <summary>
        /// handles the loading of the module setting for this
        /// control
        /// </summary>
        public override void LoadSettings()
        {
            try
            {
                if (!IsPostBack)
                {
                    using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                    {
                        DropDownListTypes.DataSource = db.HotelTypes.ToList();
                        DropDownListTypes.DataBind();

                        object setting;

                        setting = Settings["location"];
                        if (setting != null)
                        {
                            DNNTxtLocation.Text = setting.ToString();
                        }

                        setting = Settings["distance"];
                        if (setting != null)
                        {
                            DropDownListDistance.SelectedValue = setting.ToString();
                        }

                        setting = Settings["filter"];
                        if (setting != null)
                        {
                            TextBoxFilter.Text = setting.ToString();
                        }

                        setting = Settings["hoteltype"];
                        if (setting != null)
                        {
                            DropDownListTypes.SelectedValue = setting.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        /// <summary>
        /// handles updating the module settings for this control
        /// </summary>
        public override void UpdateSettings()
        {
            try
            {
                ModuleController controller = new ModuleController();
                controller.UpdateModuleSetting(ModuleId, "location", DNNTxtLocation.Text);
                Session.Remove("HiddenFieldX");
                Session.Remove("HiddenFieldY");
                controller.UpdateModuleSetting(ModuleId, "distance", DropDownListDistance.SelectedValue);
                Session.Remove("distance");
                controller.UpdateModuleSetting(ModuleId, "filter", TextBoxFilter.Text);
                controller.UpdateModuleSetting(ModuleId, "hoteltype", DropDownListTypes.SelectedValue);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void PopulateLocationsOnDemand(object source, DNNTextSuggestEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                var query = db.GeoNames.Where(gn => gn.Name.StartsWith(e.Text) && gn.FeatureClass == "P").Select(gn => gn.Name).Distinct().OrderBy(gn => gn).Take(10);
                foreach (string geoName in query)
                {
                    var objNode = new DNNNode(geoName) { ID = e.Nodes.Count.ToString() };
                    e.Nodes.Add(objNode);
                }
            }
        }
    }
}