<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HotelSettings.ascx.cs" Inherits="Cowrie.Modules.ProductList.HotelSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register assembly="DotNetNuke.WebControls" namespace="DotNetNuke.UI.WebControls" tagPrefix="dnn" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table cellspacing="0" cellpadding="2" border="0" summary="ModuleName1 Settings Design Table">
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
            <dnn:Label ID="LabelLocation" runat="server" ControlName="RadTreeViewLocations" Suffix=":"
                Text="Location">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <telerik:RadTreeView ID="RadTreeViewLocations" runat="server" Height="300px" Width="100%"
                OnNodeExpand="RadTreeViewLocations_NodeExpand" MultipleSelect="True">
            </telerik:RadTreeView>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="DnnLabelCurrentPreSelectedLocation" runat="server" Suffix=":"
                Text="Current Pre-Selected Location">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:Label ID="LabelCurrentPreSelectedLocation" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelPreSelectedLocation" runat="server" ControlName="RadTreeViewPreSelectedLocations" Suffix=":"
                Text="Pre-Selected Location">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <telerik:RadTreeView ID="RadTreeViewPreSelectedLocations" runat="server" Height="300px" Width="100%"
                OnNodeExpand="RadTreeViewLocations_NodeExpand">
            </telerik:RadTreeView>
        </td>
    </tr>
</table>
