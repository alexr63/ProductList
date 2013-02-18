<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Cowrie.Modules.ProductList.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register assembly="DotNetNuke.WebControls" namespace="DotNetNuke.UI.WebControls" tagPrefix="dnn" %>
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
            <dnn:Label ID="LabelLocation" runat="server" ControlName="DNNTreeLocations" Suffix=":"
                Text="Location">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <dnn:DnnTree runat="server" ID="DNNTreeLocations" CollapsedNodeImage="../../Images/Plus.gif" ExpandedNodeImage="../../Images/Minus.gif" PopulateOnDemand="DNNTreeLocations_PopulateOnDemand"></dnn:DnnTree>
        </td>
    </tr>
</table>
