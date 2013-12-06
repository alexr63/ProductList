<%@ control language="C#" autoeventwireup="true" codebehind="HotelList.ascx.cs"
    inherits="Cowrie.Modules.ProductList.HotelList" %>
<%@ register assembly="DotNetNuke.WebControls" namespace="DotNetNuke.UI.WebControls" tagprefix="dnn" %>
<%@ import namespace="ProductList" %>
<%@ register tagprefix="telerik" namespace="Telerik.Web.UI" assembly="Telerik.Web.UI" %>
<telerik:radskinmanager id="QsfSkinManager" runat="server" showchooser="false" />
<telerik:radformdecorator id="QsfFromDecorator" runat="server" decoratedcontrols="All" enableroundedcorners="false" />
<div id="categories">
    Search:
            <asp:textbox id="TextBoxSearch" runat="server" width="100px"></asp:textbox>
    &nbsp;<asp:button id="ButtonSubmit" runat="server" text="Go" onclick="ButtonSubmit_Click" ValidationGroup="HotelListSearch" />
    <br />
    <br />
    <telerik:radtreeview id="RadTreeViewLocations" runat="server" height="800px" width="100%"
        onnodeexpand="RadTreeViewLocations_NodeExpand" onnodeclick="RadTreeViewLocations_NodeClick">
            </telerik:radtreeview>
</div>
<div id="products">
    <h1>
        <asp:label id="LabelCurrentLocation" runat="server" />
    </h1>
    <h2>
        <asp:label id="LabelSelectedLocation" runat="server" />&nbsp;<asp:label id="LabelFilteredBy" runat="server" Visible="False" />&nbsp;<asp:button id="ButtonClear" runat="server" text="Clear" onclick="ButtonClear_Click" CausesValidation="False" Visible="False" />
    </h2>
    <table style="width: 100%">
        <tr>
            <td>Sort by
                        <asp:dropdownlist id="DropDownListSortCriterias" runat="server" autopostback="True" onselectedindexchanged="DropDownListSortCriterias_SelectedIndexChanged">
                            <asp:ListItem Selected="True">Name</asp:ListItem>
                            <asp:ListItem>Price</asp:ListItem>
                            <asp:ListItem>Rating</asp:ListItem>
                        </asp:dropdownlist>
            </td>
            <td>View
                        <asp:dropdownlist id="DropDownListPageSizes" runat="server" autopostback="True" onselectedindexchanged="DropDownListPageSizes_SelectedIndexChanged">
                            <asp:ListItem Selected="True">10</asp:ListItem>
                            <asp:ListItem>20</asp:ListItem>
                            <asp:ListItem>50</asp:ListItem>
                        </asp:dropdownlist>
            </td>
            <td>
                <asp:datapager id="DataPagerContent" runat="server" pagedcontrolid="ListViewContent">
                            <Fields>
                                <asp:NextPreviousPagerField ShowFirstPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                                <asp:NumericPagerField />
                                <asp:NextPreviousPagerField ShowLastPageButton="True" ShowNextPageButton="False" ShowPreviousPageButton="False" />
                            </Fields>
                        </asp:datapager>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:label id="LabelCount" runat="server" />
                hotels found
            </td>
        </tr>
    </table>
    <asp:listview id="ListViewContent" runat="server" onpagepropertieschanging="ListViewContent_PagePropertiesChanging" onitemcommand="ListViewContent_ItemCommand">
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
                                        <h1><asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' /></h1>
                                    </td>
                                    <td style="vertical-align: middle">
                                        <telerik:RadRating ID="RadRatingStar" runat="server" Value='<%# Convert.ToDecimal(Eval("Star")) %>' ReadOnly="True" />
                                    </td>
                                    <td style="vertical-align: middle">
                                        (from <asp:Label ID="LabelPrice" runat="server" Text='<%# String.Format("{0}{1:#0.00}", Utils.GetCurrencySymbol(Eval("CurrencyCode") != null ? Eval("CurrencyCode").ToString() : String.Empty), Eval("UnitCost")) %>' /> per night)
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td style="padding: 10px; vertical-align: middle">
                            <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# Eval("Image") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' ImageWidth="150" />
                        </td>
                        <td style="vertical-align: middle">
                            <asp:Literal ID="LiteralDescription" runat="server" Text='<%# Eval("Description") != null ? Server.HtmlDecode(Eval("Description").ToString().TruncateAtWord(240)) : String.Empty %>'></asp:Literal>
                            <br />
                            <br />
                            <div id="footer">
                                <asp:Button ID="ButtonMoreHotelInfo" runat="server" Text="More hotel info" CausesValidation="False" UseSubmitBehavior="False" CommandName="MoreHotelInfo" CommandArgument='<%# Eval("Id") %>' />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<asp:Button ID="ButtonBookNow" runat="server" Text="Book Now!" CausesValidation="False" UseSubmitBehavior="False" CommandName="BookNow" CommandArgument='<%# Eval("URL") %>' />
                            </div>
                        </td>
                    </tr>
                </ItemTemplate>
            </asp:listview>
</div>
