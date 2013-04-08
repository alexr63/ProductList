<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Cowrie.Modules.ProductList.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register assembly="DotNetNuke.WebControls" namespace="DotNetNuke.UI.WebControls" tagPrefix="dnn" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table cellspacing="0" cellpadding="2" border="0" summary="ModuleName1 Settings Design Table">
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelMode" runat="server" Suffix=":"
                Text="Mode">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:RadioButtonList ID="RadioButtonListMode" runat="server">
                <asp:ListItem Value="1">Hotels</asp:ListItem>
                <asp:ListItem Value="2">Products</asp:ListItem>
            </asp:RadioButtonList>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="DnnLabelCurrentCategory" runat="server" Suffix=":"
                Text="Current Category">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:Label ID="LabelCurrentCategory" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelCategory" runat="server" ControlName="DNNTreeCategories" Suffix=":"
                Text="Category">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <telerik:RadTreeView ID="RadTreeViewCategories" runat="server" Height="300px" Width="100%"
                OnNodeExpand="RadTreeViewCategories_NodeExpand">
            </telerik:RadTreeView>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="DnnLabelCurrentLocation" runat="server" Suffix=":"
                Text="Current Location">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:Label ID="LabelCurrentLocation" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelLocation" runat="server" ControlName="DNNTreeLocations" Suffix=":"
                Text="Location">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <telerik:RadTreeView ID="RadTreeViewLocations" runat="server" Height="300px" Width="100%"
                OnNodeExpand="RadTreeViewLocations_NodeExpand">
            </telerik:RadTreeView>
        </td>
    </tr>
</table>
