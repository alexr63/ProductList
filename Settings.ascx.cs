using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.UI.WebControls;
using ProductList;
using SelectedHotelsModel;
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
                            try
                            {
                                mode = Convert.ToInt32(setting);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        RadioButtonListMode.SelectedValue = mode.ToString();

                        setting = Settings["category"];
                        int? categoryId = null;
                        if (setting != null)
                        {
                            try
                            {
                                categoryId = Convert.ToInt32(setting);
                                var category = db.Categories.SingleOrDefault(c => c.Id == categoryId);
                                if (category != null)
                                {
                                    LabelCurrentCategory.Text = category.Name;
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        Utils.PopulateCategoryTree(RadTreeViewCategories, db, null, categoryId);

                        setting = Settings["location"];
                        if (setting != null)
                        {
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
                controller.UpdateModuleSetting(ModuleId, "mode", RadioButtonListMode.SelectedValue);
                controller.UpdateModuleSetting(ModuleId, "category", RadTreeViewCategories.SelectedValue);
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
    }
}