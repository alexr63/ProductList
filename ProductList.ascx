﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductList.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.ProductList" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Import Namespace="ProductList" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<telerik:RadSkinManager ID="QsfSkinManager" runat="server" ShowChooser="false" />
<telerik:RadFormDecorator ID="QsfFromDecorator" runat="server" DecoratedControls="All" EnableRoundedCorners="false" />
<asp:MultiView ID="MultiView1" runat="server">
    <asp:View ID="ViewProducts" runat="server">
        <div id="categories">
            Search:
            <asp:TextBox ID="TextBoxSearch" runat="server"></asp:TextBox>&nbsp;<asp:Button ID="ButtonSubmit" runat="server" Text="Submit" OnClick="ButtonSubmit_Click" />
        </div>
        <div id="products">
            <h1>
                <asp:Label ID="LabelCategory" runat="server" /></h1>
            <h2>
                <asp:Label ID="LabelSelectedCategory" runat="server" /></h2>
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="LabelCount" runat="server" />
                        products found
                    </td>
                    <td>Sort by
                        <asp:DropDownList ID="DropDownListSortCriterias" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListSortCriterias_SelectedIndexChanged">
                            <asp:ListItem Selected="True">Name</asp:ListItem>
                            <asp:ListItem>Price</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>View
                        <asp:DropDownList ID="DropDownListPageSizes" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListPageSizes_SelectedIndexChanged">
                            <asp:ListItem Selected="True">12</asp:ListItem>
                            <asp:ListItem>24</asp:ListItem>
                            <asp:ListItem>56</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DataPager ID="DataPagerContent" runat="server" PagedControlID="ListViewContent" PageSize="12">
                            <Fields>
                                <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                                <asp:NumericPagerField />
                                <asp:NextPreviousPagerField ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                            </Fields>
                        </asp:DataPager>
                    </td>
                </tr>
            </table>
            <asp:ListView ID="ListViewContent" runat="server" OnPagePropertiesChanging="ListViewContent_PagePropertiesChanging">
                <LayoutTemplate>
                    <h3>Product Listing</h3>
                    <div style="width: 100%">
                        <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                    </div>
                </LayoutTemplate>

                <ItemTemplate>
                    <div style="margin: 5px; float: left; width: 210px; border-top-style: dotted; border-top-width: 1px; border-top-color: #C0C0C0; min-height: 280px">
                        <table>
                            <tr>
                                <td style="vertical-align: middle">
                                    <asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "ProductDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' />
                                </td>
                            </tr>
                            <tr>
                                <td style="vertical-align: middle">
                                    <asp:Label ID="LabelPrice" runat="server" Text='<%# Eval("UnitCost", "{0:c}") %>' />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# Eval("Image") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "ClothesDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' ImageWidth="100" />
                                </td>
                            </tr>
                            <tr>
                                <td>
                                    <asp:Literal ID="LiteralDescription" runat="server" Text='<%# Eval("Description") != null ? Server.HtmlDecode(Eval("Description").ToString().TruncateAtWord(120)) : String.Empty %>'></asp:Literal>
                                </td>
                            </tr>
                        </table>
                    </div>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </asp:View>
    <asp:View ID="ViewHotels" runat="server">
        <div id="categories">
            Search:
            <asp:TextBox ID="TextBoxSearch2" runat="server" Width="100px"></asp:TextBox>&nbsp;<asp:Button ID="ButtonSubmit2" runat="server" Text="Go" OnClick="ButtonSubmit2_Click" />
            <telerik:radtreeview id="RadTreeViewLocations" runat="server" height="800px" width="100%"
                onnodeexpand="RadTreeViewLocations_NodeExpand" onnodeclick="RadTreeViewLocations_NodeClick">
            </telerik:radtreeview>
        </div>
        <div id="products">
            <h1>
                <asp:Label ID="LabelLocation" runat="server" /></h1>
            <h2>
                <asp:Label ID="LabelSelectedLocation" runat="server" /></h2>
            <table style="width: 100%">
                <tr>
                    <td>
                        <asp:Label ID="LabelCount2" runat="server" />
                        hotels found
                    </td>
                    <td>Sort by
                        <asp:DropDownList ID="DropDownListSortCriterias2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListSortCriterias2_SelectedIndexChanged">
                            <asp:ListItem Selected="True">Name</asp:ListItem>
                            <asp:ListItem>Price</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>View
                        <asp:DropDownList ID="DropDownListPageSizes2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListPageSizes2_SelectedIndexChanged">
                            <asp:ListItem Selected="True">10</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>50</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DataPager ID="DataPagerContent2" runat="server" PagedControlID="ListViewContent2">
                            <Fields>
                                <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                                <asp:NumericPagerField />
                                <asp:NextPreviousPagerField ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                            </Fields>
                        </asp:DataPager>
                    </td>
                </tr>
            </table>
            <asp:ListView ID="ListViewContent2" runat="server" OnPagePropertiesChanging="ListViewContent2_PagePropertiesChanging">
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
                        <td style="vertical-align: middle" colspan="2">
                            <table cellpadding="5px">
                                <tr>
                                    <td style="vertical-align: middle">
                                        <h3><asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "ProductDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' /></h3>
                                    </td>
                                    <td style="vertical-align: middle">
                                        <telerik:RadRating ID="RadRatingCustomerRating" runat="server" Value='<%# Convert.ToDecimal(Eval("CustomerRating")) %>' ReadOnly="True" />
                                    </td>
                                    <td style="vertical-align: middle">
                                        (from <asp:Label ID="LabelPrice" runat="server" Text='<%# Eval("UnitCost", "{0:c}") %>' /> per night)
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle">
                            <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# Eval("Image") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "ProductDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' ImageWidth="100" />
                        </td>
                        <td style="vertical-align: middle">
                            <asp:Literal ID="LiteralDescription" runat="server" Text='<%# Eval("Description") != null ? Server.HtmlDecode(Eval("Description").ToString().TruncateAtWord(240)) : String.Empty %>'></asp:Literal>
                            <br />
                            <br />
                            <asp:HyperLink ID="HyperLinkMoreHotelInfo" runat="server" Text='More hotel info' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "ProductDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:HyperLink ID="HyperLinkBookNow" runat="server" Text='Book Now!' NavigateUrl='<%# Eval("URL") %>' Target="_blank" />
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </asp:View>
</asp:MultiView>
