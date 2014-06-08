using System;
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
    public partial class ClothesSettings : ModuleSettingsBase
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
                        DropDownListDepartments.DataSource = db.Departments.OrderBy(s => s.Name).ToList();
                        DropDownListDepartments.DataBind();
                        DropDownListMerchantCategories.DataSource = db.MerchantCategories.ToList().OrderBy(s => s.FullName);
                        DropDownListMerchantCategories.DataBind();

                        object setting = Settings["department"];
                        if (setting != null)
                        {
                            int departmentId = Convert.ToInt32(setting);
                            DropDownListDepartments.SelectedValue = departmentId.ToString();
                        }
                        setting = Settings["merchantcategory"];
                        if (setting != null)
                        {
                            int merchantCategoryId = Convert.ToInt32(setting);
                            DropDownListMerchantCategories.SelectedValue = merchantCategoryId.ToString();
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
                if (DropDownListDepartments.SelectedValue != String.Empty)
                {
                    controller.UpdateModuleSetting(ModuleId, "department", DropDownListDepartments.SelectedValue);
                }
                else
                {
                    controller.DeleteModuleSetting(ModuleId, "department");
                }
                if (DropDownListMerchantCategories.SelectedValue != String.Empty)
                {
                    controller.UpdateModuleSetting(ModuleId, "merchantcategory", DropDownListMerchantCategories.SelectedValue);
                }
                else
                {
                    controller.DeleteModuleSetting(ModuleId, "merchantcategory");
                }
            }
            catch (Exception ex)
            {
                Exceptions.ProcessModuleLoadException(this, ex);
            }
        }
    }
}