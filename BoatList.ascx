<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="BoatList.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.BoatList" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Import Namespace="ProductList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<telerik:radformdecorator id="QsfFromDecorator" runat="server" decoratedcontrols="All" enableroundedcorners="false" />
<div id="info1">
    If you know the name of the base model you wish to view use the quick navigation drop down below
</div>
<div id="categories">
    Product Range:
            <asp:DropDownList ID="DropDownListModels" runat="server" AppendDataBoundItems="True" DataValueField="Id" DataTextField="Name" AutoPostBack="True" OnSelectedIndexChanged="DropDownListModels_SelectedIndexChanged">
                <Items>
                    <asp:ListItem Value="" Text="Please Select a Model"></asp:ListItem>
                </Items>
            </asp:DropDownList>
</div>
<div id="Div1">
    Else you can browse the thumbnails below for a suitable looking vessel to base your requirements on.
</div>
<div id="products">
    <asp:ListView ID="ListViewContent3" runat="server">
        <LayoutTemplate>
            <table cellpadding="5px">
                <tr>
                    <th>Image
                    </th>
                    <th>Model
                    </th>
                    <th>Description
                    </th>
                </tr>
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>

        <ItemTemplate>
            <tr style="background-color: #80cfff;">
                <td style="vertical-align: middle">
                    <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# String.Format("~/Portals/{0}{1}", PortalId, Eval("Image")) %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' ImageWidth="100" />
                </td>
                <td style="vertical-align: middle">
                    <asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' />
                </td>
                <td style="vertical-align: middle">
                    <asp:Literal ID="LiteralDescription" runat="server" Text='<%# Eval("Description") != null ? Server.HtmlDecode(Eval("Description").ToString().TruncateAtWord(240)) : String.Empty %>'></asp:Literal>
                </td>
            </tr>
        </ItemTemplate>

        <AlternatingItemTemplate>
            <tr style="background-color: white;">
                <td style="vertical-align: middle">
                    <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# String.Format("~/Portals/{0}{1}", PortalId, Eval("Image")) %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' ImageWidth="100" />
                </td>
                <td style="vertical-align: middle">
                    <asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' />
                </td>
                <td style="vertical-align: middle">
                    <asp:Literal ID="LiteralDescription" runat="server" Text='<%# Eval("Description") != null ? Server.HtmlDecode(Eval("Description").ToString().TruncateAtWord(240)) : String.Empty %>'></asp:Literal>
                </td>
            </tr>
        </AlternatingItemTemplate>
    </asp:ListView>
</div>
