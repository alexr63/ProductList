<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="HotelSettings.ascx.cs" Inherits="Cowrie.Modules.ProductList.HotelSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Assembly="DotNetNuke.WebControls" Namespace="DotNetNuke.UI.WebControls" TagPrefix="dnn" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table cellspacing="0" cellpadding="2" border="0" summary="ModuleName1 Settings Design Table">
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelLocation" runat="server" Suffix=":" Text="Location">
            </dnn:Label>
        </td>
        <td valign="bottom" width="250">
            <asp:TextBox ID="TextBoxLocation" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelDistance" runat="server" Suffix=":" Text="Distance">
            </dnn:Label>
        </td>
        <td valign="bottom" width="250">
            <asp:DropDownList ID="DropDownListDistance" runat="server" Width="50px">
                <asp:ListItem Value="2" Text="2" />
                <asp:ListItem Value="5" Text="5" />
                <asp:ListItem Value="10" Text="10" Selected="True" />
                <asp:ListItem Value="25" Text="25" />
                <asp:ListItem Value="50" Text="50" />
            </asp:DropDownList>
            miles
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelFilter" runat="server" Suffix=":" Text="Filter">
            </dnn:Label>
        </td>
        <td valign="bottom" width="250">
            <asp:TextBox ID="TextBoxFilter" runat="server"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelType" runat="server" Suffix=":" Text="Type">
            </dnn:Label>
        </td>
        <td valign="bottom" width="250">
            <asp:DropDownList ID="DropDownListTypes" runat="server" DataTextField="Name" DataValueField="Id"></asp:DropDownList>
        </td>
    </tr>
</table>
