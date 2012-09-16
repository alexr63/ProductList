<%@ Control language="C#" autoeventwireup="true" codebehind="ProductDetails.ascx.cs"
    inherits=" Cowrie.Modules.ProductList.ProductDetails" %>
<%@ Register tagprefix="telerik" namespace="Telerik.Web.UI" assembly="Telerik.Web.UI" %>

<h1><%# product.Name %></h1>

<div class="productImage">
    <asp:HyperLink CssClass="sfLightBox" runat="server" rel='<%# String.Format("{0}_mainImage", ClientID) %>'
        ID="singleItemLink" ToolTip='<%# product.Name %>' />
</div>

<%# product.Description %>

<telerik:radlistview id="ImageList" itemplaceholderid="ItemsContainer" runat="server"
    enableembeddedskins="false" enableembeddedbasestylesheet="false">
                    <LayoutTemplate>
                        <ul class="sfimagesTmbList sfLightboxMode">
                            <asp:PlaceHolder ID="ItemsContainer" runat="server" />
                        </ul>
                    </LayoutTemplate>
                    <ItemTemplate>
                        <li class="sfimagesTmb">
                            <asp:HyperLink CssClass="sfLightBox" runat="server" rel='<%# String.Format("{0}_mainImageGallery", this.ClientID) %>'
                                ID="HyperLink1" NavigateUrl='<%# String.Format("~/Products/Galleries/{0}/Images/{1}", product.Id, Container.DataItem) %>'
                                ImageUrl='<%# String.Format("~/Products/Galleries/{0}/Thumbs/{1}", product.Id, Container.DataItem) %>'
                                ToolTip='<%# product.Name %>' />
                        </li>
                    </ItemTemplate>
                </telerik:radlistview>
