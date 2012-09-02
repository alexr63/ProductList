<%@ control language="C#" autoeventwireup="true" codebehind="ProductList.ascx.cs"
    inherits=" Cowrie.Modules.ProductList.ProductList" %>
<asp:DataList ID="DataListContent" runat="server" Width="100%" RepeatLayout="Table" OnItemDataBound="DataListContent_DataBound">
    <ItemTemplate>
        <tr>
            <td>
                <asp:Label ID="LabelName" runat="server" Text='<%# Eval("Name") %>' />
            </td>
            <td>
                <asp:Label ID="LabelPrice" runat="server" Text='<%# Eval("Price", "{0:c}") %>' />
            </td>
            <td rowspan="3">
                <asp:Image ID="Image1" runat="server" />
            </td>
            <td colspan="3">
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
