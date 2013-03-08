<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClothesDetails.ascx.cs"
    Inherits=" Cowrie.Modules.ProductList.ClothesDetails" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<script src='<%#ResolveUrl("~/DesktopModules/ProductList/Scripts/fancybox/jquery.fancybox-1.3.4.js")%>' type="text/javascript"></script>
<link href='<%#ResolveUrl("~/DesktopModules/ProductList/Scripts/fancybox/jquery.fancybox-1.3.4.css")%>' type="text/css" rel="stylesheet" />
<script type="text/javascript">
    $(document).ready(function () {
        $("a.sfLightBox").fancybox();
    })
</script>

<h1><%# product.Name %></h1>
<h2><%# product.UnitCost.Value.ToString("C") %></h2>
<h3><%# product.Size %></h3>
<div style="padding: 10px; float: left">
    <asp:Image ID="Image1" runat="server" ImageUrl='<%# product.Image %>' />
</div>
<div style="padding: 10px; float: left">
    <%# product.Description %>
</div>
<div style="padding: 10px; clear: both">
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
