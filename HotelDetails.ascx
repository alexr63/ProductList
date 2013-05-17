<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HotelDetails.ascx.cs"
    Inherits=" Cowrie.Modules.ProductList.HotelDetails" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<script src='<%#ResolveUrl("~/DesktopModules/ProductList/Scripts/fancybox/jquery.fancybox-1.3.4.js")%>' type="text/javascript"></script>
<link href='<%#ResolveUrl("~/DesktopModules/ProductList/Scripts/fancybox/jquery.fancybox-1.3.4.css")%>' type="text/css" rel="stylesheet" />
<script type="text/javascript">
    $(document).ready(function () {
        $("a.sfLightBox").fancybox();
    })
</script>

<div id="images" style="padding: 10px; float: left; width: 470px">
    <div id="image">
        <asp:Image ID="Image1" runat="server" ImageUrl='<%# hotel.Image %>' />
    </div>
    <div id="gallery" style="padding: 10px; clear: both">
        <asp:Repeater ID="Repeater1" runat="server">
            <HeaderTemplate>
            </HeaderTemplate>
            <ItemTemplate>
                <asp:HyperLink ID="HyperLink1" CssClass="sfLightBox" runat="server"
                        rel='<%# String.Format("{0}_mainImageGallery", this.ClientID) %>'
                        NavigateUrl='<%# Eval("URL") %>' Width="100px" Height="100px">
                    <asp:Image ID="Image2" runat="server" ImageUrl='<%# Eval("URL") %>' Width="100px" />
                </asp:HyperLink>
            </ItemTemplate>
            <FooterTemplate>
            </FooterTemplate>
        </asp:Repeater>
    </div>
</div>
<div id="details" style="padding: 10px;">
    <h1><%# hotel.Name %></h1>
    <h2><%# hotel.UnitCost.Value.ToString("C") %></h2>
    <telerik:RadRating ID="RadRatingStar" runat="server" Value='<%# Convert.ToDecimal(hotel.Star) %>' ReadOnly="True" />
    <asp:Button ID="ButtonBookNow" runat="server" Text="Book Now!" CausesValidation="False" OnClick="ButtonBookNow_Click" UseSubmitBehavior="False" />
    <div id="description" style="padding: 10px;">
        <%# hotel.Description %>
    </div>
</div>
