using System;
using System.Configuration;
using System.Data;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;
using Telerik.Web.UI;

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
                        object setting = Settings["mode"];
                        int mode = (int)Enums.DisplayModeEnum.Hotels;
                        if (setting != null)
                        {
                            mode = Convert.ToInt32(setting);
                        }
                        RadioButtonListMode.SelectedValue = mode.ToString();

                        setting = Settings["category"];
                        int? categoryId = null;
                        if (setting != null)
                        {
                            categoryId = Convert.ToInt32(setting);
                            var category = db.Categories.SingleOrDefault(c => c.Id == categoryId);
                            LabelCurrentCategory.Text = category.Name;
                        }
                        Utils.PopulateCategoryTree(RadTreeViewCategories, db, null, categoryId);

                        setting = Settings["location"];
                        int? locationId = null;
                        if (setting != null)
                        {
                            locationId = Convert.ToInt32(setting);
                            var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                            LabelCurrentLocation.Text = location.Name;
                        }
                        Utils.PopulateLocationTree(RadTreeViewLocations, db, null, locationId);
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
                controller.UpdateModuleSetting(ModuleId, "mode", RadioButtonListMode.SelectedValue);
                controller.UpdateModuleSetting(ModuleId, "category", RadTreeViewCategories.SelectedValue);
                controller.UpdateModuleSetting(ModuleId, "location", RadTreeViewLocations.SelectedValue);
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }
 
        protected void RadTreeViewCategories_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? categoryId = Convert.ToInt32(e.Node.Value);
                var category = db.Categories.SingleOrDefault(c => c.Id == categoryId);
                Utils.CreateSubCategoryNodes(category, e.Node, categoryId);
            }
            e.Node.Expanded = true;
        }

        protected void RadTreeViewLocations_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? locationId = Convert.ToInt32(e.Node.Value);
                var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                Utils.CreateSubLocationNodes(location, e.Node, locationId);
            }
            e.Node.Expanded = true;
        }

    }
}