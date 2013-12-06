using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web.UI.MobileControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;
using Telerik.Web.UI;

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
                        List<Location> selectedLocations = new List<Location>();
                        object setting = Settings["locations"];
                        if (setting != null)
                        {
                            try
                            {
                                string[] selectedLocationIds = setting.ToString().Split(';');
                                foreach (string s in selectedLocationIds)
                                {
                                    int locationId = int.Parse(s);
                                    var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                                    if (location != null)
                                    {
                                        selectedLocations.Add(location);
                                    }
                                }
                                LabelCurrentLocation.Text = String.Join(", ", selectedLocations);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Utils.PopulateLocationTree(RadTreeViewLocations, db, selectedLocations);

                        setting = Settings["preselectedlocation"];
                        int? preSelectedLocationId = null;
                        if (setting != null)
                        {
                            try
                            {
                                preSelectedLocationId = Convert.ToInt32(setting);
                                var preSelectedLocation = db.Locations.SingleOrDefault(l => l.Id == preSelectedLocationId);
                                if (preSelectedLocation != null)
                                {
                                    LabelCurrentPreSelectedLocation.Text = preSelectedLocation.Name;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Utils.PopulateLocationTree(RadTreeViewPreSelectedLocations, db, selectedLocations, preSelectedLocationId);
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
                string locations = String.Empty;
                foreach (var node in RadTreeViewLocations.SelectedNodes)
                {
                    if (locations != String.Empty)
                    {
                        locations += ";";
                    }
                    locations += node.Value;
                }
                controller.UpdateModuleSetting(ModuleId, "locations", locations);
                controller.UpdateModuleSetting(ModuleId, "preselectedlocation", RadTreeViewPreSelectedLocations.SelectedValue);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }

        protected void RadTreeViewLocations_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? locationId = Convert.ToInt32(e.Node.Value);
                var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                Utils.CreateSubLocationNodes(location, e.Node, new List<Location>());
            }
            e.Node.Expanded = true;
        }
    }
}