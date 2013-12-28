﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HotelSettings.ascx.cs" Inherits="Cowrie.Modules.ProductList.HotelSettings" %>
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
        <td valign="bottom"width="250">
            <asp:Label ID="LabelCurrentLocation" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelLocation" runat="server" ControlName="RadTreeViewLocations" Suffix=":"
                Text="Location">
            </dnn:Label>
        </td>
        <td valign="bottom" width="250">
            <telerik:RadTreeView ID="RadTreeViewLocations" runat="server" Height="300px" Width="250px"
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
        <td valign="bottom" width="250">
            <asp:Label ID="LabelCurrentPreSelectedLocation" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelPreSelectedLocation" runat="server" ControlName="RadTreeViewPreSelectedLocations" Suffix=":"
                Text="Pre-Selected Location">
            </dnn:Label>
        </td>
        <td valign="bottom"width="250">
            <telerik:RadTreeView ID="RadTreeViewPreSelectedLocations" runat="server" Height="300px" Width="250px"
                OnNodeExpand="RadTreeViewLocations_NodeExpand">
            </telerik:RadTreeView>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelSearch" runat="server" Suffix=":"
                Text="Search">
            </dnn:Label>
        </td>
        <td valign="bottom"width="250">
            <asp:TextBox ID="TextBoxSearch" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelType" runat="server" Suffix=":"
                Text="Type">
            </dnn:Label>
        </td>
        <td valign="bottom"width="250">
            <asp:DropDownList ID="DropDownListTypes" runat="server" DataTextField="Name" DataValueField="Id"></asp:DropDownList>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelHideTree" runat="server" Suffix=":"
                Text="Hide the navigation tree">
            </dnn:Label>
        </td>
        <td valign="bottom"width="250">
            <asp:CheckBox ID="CheckBoxHideTree" runat="server"></asp:CheckBox>
        </td>
    </tr>
</table>
