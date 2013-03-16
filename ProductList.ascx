<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductList.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.ProductList" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
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
            </table>
            <asp:ListView ID="ListViewContent" runat="server" OnPagePropertiesChanging="ListViewContent_PagePropertiesChanging">
                <LayoutTemplate>
                    <h3>Product Listing</h3>
                    <table>
                        <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>

                <ItemSeparatorTemplate>
                    <tr>
                        <td colspan="4">
                            <hr />
                        </td>
                    </tr>
                </ItemSeparatorTemplate>

                <ItemTemplate>
                    <tr>
                        <td style="vertical-align: middle">
                            <asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "ProductDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' />
                        </td>
                        <td style="vertical-align: middle">
                            <asp:Label ID="LabelPrice" runat="server" Text='<%# Eval("UnitCost", "{0:c}") %>' />
                        </td>
                        <td>
                            <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# Eval("Image") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "ProductDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' ImageWidth="100" />
                        </td>
                        <td>
                            <asp:Literal ID="LiteralDescription" runat="server" Text='<%# Eval("Description") != null ? Server.HtmlDecode(Eval("Description").ToString()) : String.Empty %>'></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </asp:View>
    <asp:View ID="ViewHotels" runat="server">
        <div id="categories">
            Search:
            <asp:TextBox ID="TextBoxSearch2" runat="server"></asp:TextBox>&nbsp;<asp:Button ID="ButtonSubmit2" runat="server" Text="Submit" OnClick="ButtonSubmit2_Click" />
            <telerik:radtreeview id="RadTreeViewLocations" runat="server" height="300px" width="100%"
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
                        <asp:Label ID="Label1" runat="server" />
                        hotels found
                    </td>
                    <td>Sort by
                        <asp:DropDownList ID="DropDownList1" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListSortCriterias_SelectedIndexChanged">
                            <asp:ListItem Selected="True">Name</asp:ListItem>
                            <asp:ListItem>Price</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>View
                        <asp:DropDownList ID="DropDownList2" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListPageSizes_SelectedIndexChanged">
                            <asp:ListItem Selected="True">10</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>50</asp:ListItem>
                        </asp:DropDownList>
                    </td>
                    <td>
                        <asp:DataPager ID="DataPager1" runat="server" PagedControlID="ListViewContent">
                            <Fields>
                                <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                                <asp:NumericPagerField />
                                <asp:NextPreviousPagerField ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                            </Fields>
                        </asp:DataPager>
                    </td>
                </tr>
            </table>
            <asp:ListView ID="ListView1" runat="server" OnPagePropertiesChanging="ListViewContent_PagePropertiesChanging">
                <LayoutTemplate>
                    <h3>Product Listing</h3>
                    <table>
                        <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>
                    </table>
                </LayoutTemplate>

                <ItemSeparatorTemplate>
                    <tr>
                        <td colspan="4">
                            <hr />
                        </td>
                    </tr>
                </ItemSeparatorTemplate>

                <ItemTemplate>
                    <tr>
                        <td style="vertical-align: middle">
                            <asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "HotelDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' />
                        </td>
                        <td style="vertical-align: middle">
                            <asp:Label ID="LabelPrice" runat="server" Text='<%# Eval("UnitCost", "{0:c}") %>' />
                        </td>
                        <td>
                            <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# Eval("Image") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(TabId, "HotelDetails", "mid=" + ModuleId, "Id=" + Eval("Id")) %>' ImageWidth="100" />
                        </td>
                        <td>
                            <asp:Literal ID="LiteralDescription" runat="server" Text='<%# Eval("Description") != null ? Server.HtmlDecode(Eval("Description").ToString()) : String.Empty %>'></asp:Literal>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:ListView>
        </div>
    </asp:View>
</asp:MultiView>
