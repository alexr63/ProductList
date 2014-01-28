using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Web;
using ProductList;
using SelectedHotelsModel;
using Telerik.Web.UI;
using Telerik.Web.UI.PivotGrid.Core.Totals;

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
                cmdDelete.Attributes.Add("onClick",
                    "javascript:return confirm('Are you sure you wish to delete this item?');");

                using (SelectedHotelsEntities db = new SelectedHotelsEntities())
                {
                    DropDownListTypes.DataSource = db.HotelTypes.ToList();
                    DropDownListTypes.DataBind();

                    if (Request.QueryString["ItemId"] != null)
                    {
                        int id = int.Parse(Request.QueryString["ItemId"]);
                        Hotel hotel = db.Products.Find(id) as Hotel;
                        if (hotel != null)
                        {
                            cmdDelete.Visible = true;

                            TextBoxName.Text = hotel.Name;
                            TextBoxNumber.Text = hotel.Number;
                            if (hotel.UnitCost.HasValue)
                                TextBoxUnitCost.Text = hotel.UnitCost.Value.ToString("#0.00");
                            txtDescription.Text = hotel.Description;
                            txtExtraDescription.Text = hotel.ExtraDescription;
                            TextBoxURL.Text = hotel.URL;
                            TextBoxImage.Text = hotel.Image;

                            LabelCurrentLocation.Text = hotel.Location.Name;
                            Utils.PopulateLocationTree(RadTreeViewLocations, db, null, hotel.LocationId);

                            TextBoxRooms.Text = hotel.Rooms.ToString();
                            if (hotel.Star.HasValue)
                                TextBoxStar.Text = hotel.Star.Value.ToString("0.0");
                            if (hotel.CustomerRating.HasValue)
                                TextBoxCustomerRating.Text = hotel.CustomerRating.Value.ToString("0.0");
                            TextBoxAddress.Text = hotel.Address;
                            TextBoxCurrencyCode.Text = hotel.CurrencyCode;
                            if (hotel.Lat.HasValue)
                                TextBoxLat.Text = hotel.Lat.Value.ToString();
                            if (hotel.Lon.HasValue)
                                TextBoxLon.Text = hotel.Lon.Value.ToString();
                            TextBoxPostCode.Text = hotel.PostCode;
                            DropDownListTypes.SelectedValue = hotel.HotelTypeId.ToString();

                            RadGridAdditionalImages.Visible = true;
                        }
                    }
                    else
                    {
                        Utils.PopulateLocationTree(RadTreeViewLocations, db);
                        RadGridAdditionalImages.Visible = false;
                    }
                }
            }
        }

        protected void cmdSave_Command(object sender, System.Web.UI.WebControls.CommandEventArgs e)
        {
            if (!Page.IsValid)
            {
                return;
            }

            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                Hotel hotel = null;
                if (Request.QueryString["ItemId"] != null)
                {
                    int id = int.Parse(Request.QueryString["ItemId"]);
                    hotel = db.Products.Find(id) as Hotel;
                    if (hotel != null)
                    {
                        hotel.Name = TextBoxName.Text;
                        hotel.Number = TextBoxNumber.Text;
                        if (TextBoxUnitCost.Text != String.Empty)
                        {
                            hotel.UnitCost = decimal.Parse(TextBoxUnitCost.Text);
                        }
                        else
                        {
                            hotel.UnitCost = null;
                        }
                        hotel.Description = txtDescription.Text;
                        hotel.ExtraDescription = txtExtraDescription.Text;
                        hotel.URL = TextBoxURL.Text;
                        hotel.Image = TextBoxImage.Text;
                        if (RadTreeViewLocations.SelectedValue != String.Empty)
                        {
                            var locationId = hotel.LocationId;
                            var newLocationId = int.Parse(RadTreeViewLocations.SelectedValue);
                            if (locationId != newLocationId)
                            {
                                hotel.LocationId = newLocationId;
                                var oldHotelLocations = db.HotelLocations.Where(hl => hl.HotelId == id);
                                db.HotelLocations.RemoveRange(oldHotelLocations);
                                UpdateHotelLocations(db, hotel);
                            }
                        }
                        if (TextBoxRooms.Text != String.Empty)
                        {
                            hotel.Rooms = int.Parse(TextBoxRooms.Text);
                        }
                        else
                        {
                            hotel.Rooms = null;
                        }
                        if (TextBoxStar.Text != String.Empty)
                        {
                            hotel.Star = decimal.Parse(TextBoxStar.Text);
                        }
                        else
                        {
                            hotel.Star = null;
                        }
                        if (TextBoxCustomerRating.Text != String.Empty)
                        {
                            hotel.CustomerRating = decimal.Parse(TextBoxCustomerRating.Text);
                        }
                        else
                        {
                            hotel.CustomerRating = null;
                        }
                        hotel.Address = TextBoxAddress.Text;
                        hotel.CurrencyCode = TextBoxCurrencyCode.Text;
                        if (TextBoxLat.Text != String.Empty)
                        {
                            hotel.Lat = double.Parse(TextBoxLat.Text);
                        }
                        else
                        {
                            hotel.Lat = null;
                        }
                        if (TextBoxLon.Text != String.Empty)
                        {
                            hotel.Lon = double.Parse(TextBoxLon.Text);
                        }
                        else
                        {
                            hotel.Lon = null;
                        }
                        hotel.PostCode = TextBoxPostCode.Text;
                        hotel.HotelTypeId = int.Parse(DropDownListTypes.SelectedValue);
                        db.SaveChanges();
                    }
                }
                else
                {
                    hotel = new Hotel
                    {
                        Name = TextBoxName.Text,
                        Number = TextBoxNumber.Text,
                        Description = txtDescription.Text,
                        ExtraDescription = txtExtraDescription.Text,
                        URL = TextBoxURL.Text,
                        Image = TextBoxImage.Text,
                        LocationId = int.Parse(RadTreeViewLocations.SelectedValue),
                        Address = TextBoxAddress.Text,
                        CurrencyCode = TextBoxCurrencyCode.Text,
                        PostCode = TextBoxPostCode.Text,
                        HotelTypeId = int.Parse(DropDownListTypes.SelectedValue),
                        CreatedByUser = UserId,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false,
                    };
                    if (TextBoxRooms.Text != String.Empty)
                        hotel.Rooms = int.Parse(TextBoxRooms.Text);
                    if (TextBoxUnitCost.Text != String.Empty)
                        hotel.UnitCost = decimal.Parse(TextBoxUnitCost.Text);
                    if (TextBoxStar.Text != String.Empty)
                        hotel.Star = decimal.Parse(TextBoxStar.Text);
                    if (TextBoxCustomerRating.Text != String.Empty)
                        hotel.CustomerRating = decimal.Parse(TextBoxCustomerRating.Text);
                    if (TextBoxLat.Text != String.Empty)
                        hotel.Lat = double.Parse(TextBoxLat.Text);
                    if (TextBoxLon.Text != String.Empty)
                        hotel.Lon = double.Parse(TextBoxLon.Text);
                    db.Products.Add(hotel);
                    db.SaveChanges();

                    db.Entry(hotel).Reference(h => h.Location).Load();
                    UpdateHotelLocations(db, hotel);
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
            int id = int.Parse(Request.QueryString["ItemId"]);
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

        protected void CustomValidatorCurrencyCode_ServerValidate(object source,
            System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            args.IsValid = Common.Utils.GetCurrencySymbol(args.Value) != args.Value;
        }

        private void UpdateHotelLocations(SelectedHotelsEntities db, Hotel hotel)
        {
            if (
                db.HotelLocations.SingleOrDefault(
                    hl =>
                        hl.HotelId == hotel.Id && hl.LocationId == hotel.Location.Id &&
                        hl.HotelTypeId == hotel.HotelType.Id) == null)
            {
                HotelLocation hotelLocation = new HotelLocation
                {
                    HotelId = hotel.Id,
                    LocationId = hotel.Location.Id,
                    HotelTypeId = hotel.HotelType.Id
                };
                hotel.HotelLocations.Add(hotelLocation);
            }
            if (hotel.Location.ParentLocation != null)
            {
                if (
                    db.HotelLocations.SingleOrDefault(
                        hl =>
                            hl.HotelId == hotel.Id && hl.LocationId == hotel.Location.ParentLocation.Id &&
                            hl.HotelTypeId == hotel.HotelType.Id) == null)
                {
                    HotelLocation hotelLocation = new HotelLocation
                    {
                        HotelId = hotel.Id,
                        LocationId = hotel.Location.ParentLocation.Id,
                        HotelTypeId = hotel.HotelType.Id
                    };
                    hotel.HotelLocations.Add(hotelLocation);
                }
                if (hotel.Location.ParentLocation.ParentLocation != null)
                {
                    if (
                        db.HotelLocations.SingleOrDefault(
                            hl =>
                                hl.HotelId == hotel.Id &&
                                hl.LocationId == hotel.Location.ParentLocation.ParentLocation.Id &&
                                hl.HotelTypeId == hotel.HotelType.Id) == null)
                    {
                        HotelLocation hotelLocation = new HotelLocation
                        {
                            HotelId = hotel.Id,
                            LocationId = hotel.Location.ParentLocation.ParentLocation.Id,
                            HotelTypeId = hotel.HotelType.Id
                        };
                        hotel.HotelLocations.Add(hotelLocation);
                    }
                }
            }
            db.SaveChanges();
        }

        protected void RadGridAdditionalImages_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            int id = int.Parse(Request.QueryString["ItemId"]);
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                var hotel = db.Products.Find(id) as Hotel;
                RadGridAdditionalImages.DataSource = hotel.ProductImages.ToList();
            }
        }

        protected void RadGridAdditionalImages_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editableItem = ((GridEditableItem) e.Item);
            var id = (int) editableItem.GetDataKeyValue("Id");

            //retrive entity form the Db 
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                var productImage = db.ProductImages.Find(id);
                if (productImage != null)
                {
                    //update entity's state 
                    editableItem.UpdateValues(productImage);

                    try
                    {
                        //submit chanages to Db 
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        protected void RadGridAdditionalImages_InsertCommand(object sender, GridCommandEventArgs e)
        {
            int id = int.Parse(Request.QueryString["ItemId"]);
            var editableItem = ((GridEditableItem) e.Item);
            //create new entity 
            var productImage = new ProductImage {ProductId = id};
            //populate its properties 
            Hashtable values = new Hashtable();
            editableItem.ExtractValues(values);
            productImage.URL = (string) values["URL"];

            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                db.ProductImages.Add(productImage);
                try
                {
                    //submit chanages to Db 
                    db.SaveChanges();
                }
                catch (Exception)
                {
                }
            }
        }

        protected void RadGridAdditionalImages_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && (e.Item.IsInEditMode))
            {
                GridEditableItem editableItem = (GridEditableItem) e.Item;
                SetupInputManager(editableItem);
            }
        }

        private void SetupInputManager(GridEditableItem editableItem)
        {
            // style and set URL column's textbox as required 
            var textBox =
                ((GridTextBoxColumnEditor) editableItem.EditManager.GetColumnEditor("URL")).TextBoxControl;

            InputSetting inputSetting = RadInputManager1.GetSettingByBehaviorID("TextBoxSetting1");
            inputSetting.TargetControls.Add(new TargetInput(textBox.UniqueID, true));
            inputSetting.InitializeOnClient = true;
            inputSetting.Validation.IsRequired = true;
        }

        protected void RadGridAdditionalImages_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var id = (int) ((GridDataItem) e.Item).GetDataKeyValue("Id");

            //retrive entity form the Db 
            using (SelectedHotelsEntities db = new SelectedHotelsEntities())
            {
                var productImage = db.ProductImages.Find(id);
                if (productImage != null)
                {
                    //add the category for deletion 
                    db.ProductImages.Remove(productImage);
                    try
                    {
                        //submit chanages to Db 
                        db.SaveChanges();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
    }
}