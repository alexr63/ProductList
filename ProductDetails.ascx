<%@ control language="C#" autoeventwireup="true" codebehind="ProductDetails.ascx.cs"
    inherits=" Cowrie.Modules.ProductList.ProductDetails" %>
<%@ register tagprefix="telerik" namespace="Telerik.Web.UI" assembly="Telerik.Web.UI" %>

<h1><%# product.Name %></h1>
<h2><%# product.UnitCost.Value.ToString("C") %></h2>
<div style="padding: 10px; float: left">
    <asp:Image ID="Image1" runat="server" ImageUrl='<%# product.Image %>' />
</div>
<div style="padding: 10px; float: left">
    <%# product.Description %>
</div>
<div style="padding: 10px; clear: both">
    <asp:Repeater ID="Repeater1" runat="server">
        <ItemTemplate>
            <asp:Image ID="Image2" runat="server" ImageUrl='<%# Eval("URL") %>' Width="100px" />
        </ItemTemplate>
    </asp:Repeater>
</div>
