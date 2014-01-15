using System;
using System.Linq;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Web;
using ProductList;
using SelectedHotelsModel;
using Telerik.Web.UI;

namespace Cowrie.Modules.ProductList
{
    public partial class EditHotel : PortalModuleBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //MessageWindowParameters messageWindowParameters = new MessageWindowParameters("Are you sure you wish to delete this item?", "Confirm Detele");
                //cmdDelete.OnClientClick = Utilities.GetOnClientClickConfirm(ref cmdDelete, messageWindowParameters);
                cmdDelete.Attributes.Add("onClick", "javascript:return confirm('Are you sure you wish to delete this item?');");

                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    DropDownListTypes.DataSource = db.HotelTypes.ToList();
                    DropDownListTypes.DataBind();

                    if (Request.QueryString["Id"] != null)
                    {
                        int id = int.Parse(Request.QueryString["Id"]);
                        Hotel hotel = db.Products.Find(id) as Hotel;
                        if (hotel != null)
                        {
                            cmdDelete.Visible = true;

                            TextBoxName.Text = hotel.Name;
                            TextBoxNumber.Text = hotel.Number;
                            if (hotel.UnitCost.HasValue)
                            {
                                TextBoxUnitCost.Text = hotel.UnitCost.Value.ToString("#0.00");
                            }
                            txtDescription.Text = hotel.Description;
                            TextBoxURL.Text = hotel.URL;
                            TextBoxImage.Text = hotel.Image;

                            LabelCurrentLocation.Text = hotel.Location.Name;
                            Utils.PopulateLocationTree(RadTreeViewLocations, db, null, hotel.LocationId);

                            Utils.PopulateLocationTree(DnnTreeViewLocations, db, null, hotel.LocationId);
                        }
                    }
                    else
                    {
                        Utils.PopulateLocationTree(RadTreeViewLocations, db);
                    }
                }
            }
        }

        protected void cmdSave_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                Hotel hotel = null;
                if (Request.QueryString["Id"] != null)
                {
                    int id = int.Parse(Request.QueryString["Id"]);
                    hotel = db.Products.Find(id) as Hotel;
                    if (hotel != null)
                    {
                        hotel.Name = TextBoxName.Text;
                        hotel.Number = TextBoxNumber.Text;
                        hotel.Description = txtDescription.Text;
                        hotel.URL = TextBoxURL.Text;
                        hotel.Image = TextBoxImage.Text;
                        db.SaveChanges();
                    }
                }
                else
                {
                    hotel = new Hotel
                    {
                        Name = TextBoxName.Text,
                        Description = txtDescription.Text,
                        CreatedByUser = UserId,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                        URL = TextBoxURL.Text,
                        Image = TextBoxImage.Text
                    };
                    db.Products.Add(hotel);
                    db.SaveChanges();
                }
            }

            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected void cmdCancel_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected void cmdDelete_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            int id = int.Parse(Request.QueryString["Id"]);
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                var hotel = db.Products.Find(id) as Hotel;
                if (hotel != null)
                {
                    hotel.IsDeleted = true;
                    db.SaveChanges();
                }
            }
            Response.Redirect(DotNetNuke.Common.Globals.NavigateURL(TabId));
        }

        protected void RadTreeViewLocations_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                int? locationId = Convert.ToInt32(e.Node.Value);
                var location = db.Locations.SingleOrDefault(l => l.Id == locationId);
                Utils.CreateSubLocationNodes(db, location, e.Node, locationId);
            }
            e.Node.Expanded = true;
        }

        protected void DnnTreeViewLocations_NodeExpand(object o, Telerik.Web.UI.RadTreeNodeEventArgs e)
        {

            DnnTreeNode nodeClicked = (DnnTreeNode)e.Node;

            nodeClicked.Nodes.Clear();

        }
    }
}