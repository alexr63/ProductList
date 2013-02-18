using System;
using System.Configuration;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;

namespace Cowrie.Modules.ProductList
{
    public partial class Settings : ModuleSettingsBase
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
                        object setting = Settings["location"];
                        int? locationId = null;
                        if (setting != null)
                        {
                            locationId = Convert.ToInt32(setting);
                            var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                            LabelCurrentLocation.Text = location.Name;
                        }
                        Utils.PopulateTree(DNNTreeLocations, db, null, locationId);
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
                if (DNNTreeLocations.SelectedTreeNodes.Count > 0)
                {
                    foreach (TreeNode selectedTreeNode in DNNTreeLocations.SelectedTreeNodes)
                    {
                        controller.UpdateModuleSetting(ModuleId, "location", selectedTreeNode.Key);
                    }
                }
                else
                {
                    controller.DeleteModuleSetting(ModuleId, "location");
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
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