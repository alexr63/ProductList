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
                        object setting = Settings["location"];
                        int? locationId = null;
                        if (setting != null)
                        {
                            locationId = Convert.ToInt32(setting);
                            var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                            LabelCurrentLocation.Text = location.Name;
                        }
                        PopulateTree(RadTreeViewLocations, db, null, locationId);
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
                CreateSubLocationNodes(location, e.Node, locationId);
            }
            e.Node.Expanded = true;
        }

        public static int? PopulateTree(RadTreeView radTreeView, SelectedHotelsEntities db, int? locationId = null, int? selectedLocationId = null)
        {
            radTreeView.Nodes.Clear();
            IOrderedQueryable<Location> topLocations = from l in db.Locations
                                                       where !l.IsDeleted &&
                                                           (locationId == null
                                                                ? l.ParentId == null
                                                                : l.ParentId == locationId)
                                                       orderby l.Name
                                                       select l;
            foreach (Location location in topLocations)
            {
                RadTreeNode node = new RadTreeNode();
                node.Text = location.Name;
                node.ToolTip = location.Name;
                node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                node.Value = location.Id.ToString();
                if (selectedLocationId != null && location.Id == selectedLocationId)
                {
                    node.Selected = true;
                }
                radTreeView.Nodes.Add(node);
                //CreateSubLocationNodes(location, objNode, selectedLocationId);
            }
            if (topLocations.Any())
            {
                return topLocations.First().Id;
            }
            return null;
        }

        public static void CreateSubLocationNodes(Location location, RadTreeNode objNode, int? selectedLocationId)
        {
            if (location.SubLocations.Any(l => !l.IsDeleted))
            {
                var subLocations = from l in location.SubLocations
                                   where !l.IsDeleted
                                   orderby l.Name
                                   select l;
                foreach (Location subLocation in subLocations)
                {
                    RadTreeNode node = new RadTreeNode();
                    node.Text = subLocation.Name;
                    node.ToolTip = subLocation.Name;
                    node.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
                    node.Value = subLocation.Id.ToString();
                    if (selectedLocationId != null && location.Id == selectedLocationId)
                    {
                        node.Selected = true;
                    }
                    objNode.Nodes.Add(node);
                    //CreateSubLocationNodes(subLocation, objSubNode, selectedLocationId);
                }
            }
        }
    }
}