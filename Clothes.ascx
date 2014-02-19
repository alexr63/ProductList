<%@ control language="C#" autoeventwireup="true" codebehind="Clothes.ascx.cs"
    inherits="Cowrie.Modules.ProductList.Clothes" %>
<%@ Import Namespace="Common" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<div class="sizes">
    <h2>Filter by</h2>
    <h3>Gender</h3>
    <asp:CheckBoxList ID="CheckBoxListGenders" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CheckBoxListGenders_SelectedIndexChanged">
        <asp:ListItem Value="male">Male</asp:ListItem>
        <asp:ListItem Value="female">Female</asp:ListItem>
    </asp:CheckBoxList>
    <h3>Colour</h3>
    <asp:CheckBoxList ID="CheckBoxListColours" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CheckBoxListColours_SelectedIndexChanged" RepeatColumns="2">
    </asp:CheckBoxList>
    <h3>Brand</h3>
    <asp:CheckBoxList ID="CheckBoxListBrands" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CheckBoxListBrands_SelectedIndexChanged" DataTextField="Name" DataValueField="Id">
    </asp:CheckBoxList>
    <h3>Style</h3>
    <asp:CheckBoxList ID="CheckBoxListStyles" runat="server" AutoPostBack="True" OnSelectedIndexChanged="CheckBoxListStyles_SelectedIndexChanged" DataTextField="Name" DataValueField="Id">
    </asp:CheckBoxList>
    <h3>Sizes</h3>
    <asp:Button ID="ButtonSearch1" runat="server" Text="Search" OnClick="ButtonSearch_Click" />
    <asp:CheckBoxList ID="CheckBoxListSizes" runat="server" AppendDataBoundItems="False">
    </asp:CheckBoxList>
    <asp:Button ID="ButtonSearch2" runat="server" Text="Search" OnClick="ButtonSearch_Click" />
</div>
<div class="products" style="width: 660px;">
    <h1><asp:Label ID="LabelLocation" runat="server" /></h1>
    <table style="width: 100%">
        <tr>
            <td>
                <asp:Label ID="LabelCount" runat="server" /> items found
            </td>
            <td>
                Sort by <asp:DropDownList ID="DropDownListSortCriterias" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListSortCriterias_SelectedIndexChanged">
                    <asp:ListItem Selected="True">Name</asp:ListItem>
                    <asp:ListItem>Price</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                View <asp:DropDownList ID="DropDownListPageSizes" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DropDownListPageSizes_SelectedIndexChanged">
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
            <div style="width: 100%">
                <asp:PlaceHolder runat="server" ID="itemPlaceholder"></asp:PlaceHolder>                        
            </div>
        </LayoutTemplate>
        
        <ItemTemplate>
            <div style="margin: 5px; float: left; width: 210px; border-top-style: dotted; border-top-width: 1px; border-top-color: #C0C0C0; min-height: 280px">
                <table>
                    <tr>
                        <td style="vertical-align: middle">
                            <asp:HyperLink ID="HyperLinkName" runat="server" Text='<%# Eval("Name") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' />
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle">
                            <%# Eval("UnitCost") != null ? String.Format("{0}{1:#0.00}", Utils.GetCurrencySymbol(Eval("CurrencyCode").ToString()), Eval("UnitCost")) : String.Empty %>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:HyperLink ID="HyperLinkImage" runat="server" ImageUrl='<%# Eval("Image") %>' NavigateUrl='<%# DotNetNuke.Common.Globals.NavigateURL(DetailsTabId, "", "Id=" + Eval("Id")) %>' ImageWidth="100" />
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
