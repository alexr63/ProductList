using System;
using System.Configuration;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Exceptions;
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
                        var locations = from l in db.Locations
                                        where l.ParentId == null
                                        orderby l.Name
                                        select l;
                        cbLocations.DataSource = locations.ToList();
                        cbLocations.DataBind();

                        object setting = Settings["location"];
                        if (setting != null)
                        {
                            cbLocations.SelectedValue = setting.ToString();
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
                controller.UpdateModuleSetting(ModuleId, "location", cbLocations.SelectedValue);
            }
			catch (Exception ex)
			{
				Exceptions.ProcessModuleLoadException(this, ex);
			}
		}
	}
}