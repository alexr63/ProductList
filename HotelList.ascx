﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HotelList.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.HotelList" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="DNN" %>
<%@ Import Namespace="Common" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register Assembly="GMaps" Namespace="Subgurim.Controles" TagPrefix="cc1" %>
<telerik:radskinmanager id="QsfSkinManager" runat="server" showchooser="false" />
<telerik:radformdecorator id="QsfFromDecorator" runat="server" decoratedcontrols="All" enableroundedcorners="false" />
<asp:Panel ID="PanelLocations" runat="server" CssClass="locations">
    <cc1:GMap ID="locationGMap" runat="server"
        onserverevent="locationGMap_ServerEvent" enableServerEvents="True"
        GZoom="10" Width="100%" />
    <br />
<asp:HiddenField ID="HiddenFieldX" runat="server" />
<asp:HiddenField ID="HiddenFieldY" runat="server" />
    Enter a distance&nbsp;
            <asp:DropDownList ID="DropDownListDistance" runat="server" TabIndex="1" Width="50px">
                <asp:ListItem Value="10" Text="10" Selected="True" />
                <asp:ListItem Value="25" Text="25" />
                <asp:ListItem Value="50" Text="50" />
            </asp:DropDownList>
    &nbsp;miles&nbsp;
    <asp:Button ID="ButtonLocate" runat="server" Text="Search" OnClick="ButtonLocate_Click" />
</asp:Panel>
<asp:Panel ID="PanelCategories" runat="server" CssClass="categories" Visible="True">
    <telerik:radtreeview id="RadTreeViewLocations" runat="server" height="1800px" width="100%"
        onnodeexpand="RadTreeViewLocations_NodeExpand" onnodeclick="RadTreeViewLocations_NodeClick" Visible="False">
    </telerik:radtreeview>
</asp:Panel>
<asp:Panel ID="PanelProducts" runat="server" CssClass="products" Width="600px">
    <h1>
        <asp:Label ID="LabelCurrentLocation" runat="server" />
    </h1>
    <div class="search">
        Search:
            <asp:TextBox ID="TextBoxSearch" runat="server" Width="100px"></asp:TextBox>
        &nbsp;<asp:Button ID="ButtonSubmit" runat="server" Text="Go" OnClick="ButtonSubmit_Click" ValidationGroup="HotelListSearch" />&nbsp;<asp:Button ID="ButtonClear" runat="server" Text="Clear" OnClick="ButtonClear_Click" CausesValidation="False" Visible="False" />
    </div>
    <h2>Near&nbsp;<asp:Label ID="LabelSelectedLocation" runat="server" />&nbsp;<asp:Label ID="LabelFilteredBy" runat="server" Visible="False" />
    </h2>
    <table style="width: 100%">
        <tr width="280px">
            <td>Sort by
                        <asp:DropDownList ID="DropDownListSortCriterias" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListSortCriterias_SelectedIndexChanged" Width="200px">
                            <asp:ListItem Value="1" Selected="True">Name</asp:ListItem>
                            <asp:ListItem Value="2">Price (low to high)</asp:ListItem>
                            <asp:ListItem Value="3">Price (high to low)</asp:ListItem>
                            <asp:ListItem Value="4">Rating (low to high)</asp:ListItem>
                            <asp:ListItem Value="5">Rating (high to low)</asp:ListItem>
                        </asp:DropDownList>
            </td>
            <td width="150px">View
                        <asp:DropDownList ID="DropDownListPageSizes" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListPageSizes_SelectedIndexChanged" Width="100px">
                            <asp:ListItem Selected="True">10</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>50</asp:ListItem>
                        </asp:DropDownList>
            </td>
            <td>
                <asp:DataPager ID="DataPagerContent" runat="server" PagedControlID="ListViewContent">
                    <Fields>
                        <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                        <asp:NumericPagerField />
                        <asp:NextPreviousPagerField ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                    </Fields>
                </asp:DataPager>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Label ID="LabelCount" runat="server" />
                hotels found
            </td>
        </tr>
    </table>
    <asp:ListView ID="ListViewContent" runat="server" OnPagePropertiesChanging="ListViewContent_PagePropertiesChanging" OnItemCommand="ListViewContent_ItemCommand">
        <LayoutTemplate>
            <table cellpadding="5px">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
            </table>
        </LayoutTemplate>

        <ItemSeparatorTemplate>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
        </ItemSeparatorTemplate>

        <ItemTemplate>
            <tr>
                <td rowspan="2" style="vertical-align: middle">
                    <h1><%# Container.DataItemIndex + 1 %></h1>
                </td>
                <td style="vertical-align: middle" colspan="2">
                    <table cellpadding="5px">
                        <tr>
                            <td>
                                <asp:HyperLink ID="editLink" NavigateUrl='<%# EditUrl("ItemId", Eval("Id").ToString()) %>'
                                    Visible="<%# IsEditable %>" runat="server">
                                    <asp:Image ID="editLinkImage" AlternateText="Edit" Visible="<%# IsEditable %>" ImageUrl="~/images/edit.gif"
                                        runat="Server" resourcekey="Edit" />
                                </asp:HyperLink>
                            </td>
                            <td style="vertical-align: middle">
                                <h1>
                                    <asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' /></h1>
                            </td>
                            <td style="vertical-align: middle">
                                <telerik:RadRating ID="RadRatingStar" runat="server" Value='<%# Convert.ToDecimal(Eval("Star")) %>' ReadOnly="True" />
                            </td>
                            <td style="vertical-align: middle">
                                <%# Eval("UnitCost") != null ? String.Format("(from {0}{1:#0.00} per night)", Utils.GetCurrencySymbol(Eval("CurrencyCode").ToString()), Eval("UnitCost")) : String.Empty %>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="padding: 10px; vertical-align: top">
                    <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# Eval("Image") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' ImageWidth="150" />
                </td>
                <td style="vertical-align: middle">
                    <asp:Literal ID="LiteralDescription" runat="server" Text='<%# Eval("Description") != null ? Server.HtmlDecode(Eval("Description").ToString().TruncateAtWord(240)) : String.Empty %>'></asp:Literal>
                    <br />
                    <br />
                    <div class="footer">
                        <asp:Button ID="ButtonMoreHotelInfo" runat="server" Text="More hotel info" CausesValidation="False" UseSubmitBehavior="False" CommandName="MoreHotelInfo" CommandArgument='<%# Eval("Id") %>' />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="ButtonBookNow" runat="server" Text="Book Now!" CausesValidation="False" UseSubmitBehavior="False" CommandName="BookNow" CommandArgument='<%# Eval("URL") %>' />
                    </div>
                </td>
            </tr>
        </ItemTemplate>
    </asp:ListView>
    <table style="width: 100%">
        <tr>
            <td width="280px"></td>
            <td width="150px"></td>
            <td>
                <asp:DataPager ID="DataPagerContent2" runat="server" PagedControlID="ListViewContent">
                    <Fields>
                        <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                        <asp:NumericPagerField />
                        <asp:NextPreviousPagerField ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                    </Fields>
                </asp:DataPager>
            </td>
        </tr>
    </table>
</asp:Panel>
