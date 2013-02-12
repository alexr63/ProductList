<%@ control language="C#" autoeventwireup="true" codebehind="ProductList.ascx.cs"
    inherits=" Cowrie.Modules.ProductList.ProductList" %>
<%@ Register assembly="DotNetNuke.WebControls" namespace="DotNetNuke.UI.WebControls" tagPrefix="dnn" %>
<div id="categories">
    <dnn:DnnTree runat="server" ID="DNNTreeLocations" CollapsedNodeImage="../../Images/Plus.gif" ExpandedNodeImage="../../Images/Minus.gif" OnNodeClick="DNNTreeLocations_NodeClick"></dnn:DnnTree>
</div>
<div id="products">
    <h1><asp:Label ID="LabelLocation" runat="server" /></h1>
    <asp:DataList ID="DataListContent" runat="server" Width="100%" RepeatLayout="Table" OnItemDataBound="DataListContent_DataBound" CellPadding="5">
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
        <SeparatorTemplate>
            <tr>
                <td colspan="4">
                    <hr />
                </td>
            </tr>
        </SeparatorTemplate>
    </asp:DataList>
</div>
