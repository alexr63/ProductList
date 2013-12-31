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
using SelectedHotelsModel;
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
                        DropDownListTypes.DataSource = db.HotelTypes.ToList();
                        DropDownListTypes.DataBind();

                        object setting;

                        setting = Settings["location"];
                        int? locationId = null;
                        if (setting != null)
                        {
                            try
                            {
                                locationId = Convert.ToInt32(setting);
                                var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                                if (location != null)
                                {
                                    LabelCurrentLocation.Text = location.Name;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Utils.PopulateLocationTree(RadTreeViewLocations, db, null, locationId);

#if MULTIPLELOCATIONS
                        List<Location> selectedLocations = new List<Location>();
                        setting = Settings["locations"];
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
                        //Utils.PopulateLocationTree(RadTreeViewLocations, db, selectedLocations);
#endif

                        setting = Settings["preselectedlocation"];
                        int? preSelectedLocationId = null;
                        if (setting != null)
                        {
                            try
                            {
                                preSelectedLocationId = Convert.ToInt32(setting);
                                var preSelectedLocation =
                                    db.Locations.SingleOrDefault(l => l.Id == preSelectedLocationId);
                                if (preSelectedLocation != null)
                                {
                                    LabelCurrentPreSelectedLocation.Text = preSelectedLocation.Name;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Utils.PopulateLocationTree(RadTreeViewPreSelectedLocations, db, null, preSelectedLocationId);

                        setting = Settings["search"];
                        if (setting != null)
                        {
                            TextBoxSearch.Text = setting.ToString();
                        }

                        setting = Settings["hoteltype"];
                        if (setting != null)
                        {
                            DropDownListTypes.SelectedValue = setting.ToString();
                        }

                        setting = Settings["hidetree"];
                        if (setting != null)
                        {
                            CheckBoxHideTree.Checked = Convert.ToBoolean(setting);
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
                controller.UpdateModuleSetting(ModuleId, "location", RadTreeViewLocations.SelectedValue);

#if MULTIPLELOCATIONS
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
#endif

                controller.UpdateModuleSetting(ModuleId, "preselectedlocation",
                    RadTreeViewPreSelectedLocations.SelectedValue);
                controller.UpdateModuleSetting(ModuleId, "search", TextBoxSearch.Text);
                controller.UpdateModuleSetting(ModuleId, "hoteltype", DropDownListTypes.SelectedValue);
                controller.UpdateModuleSetting(ModuleId, "hidetree", CheckBoxHideTree.Checked.ToString());
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
                Utils.CreateSubLocationNodes(db, location, e.Node, locationId, locationId);
            }
            e.Node.Expanded = true;
        }
    }
}