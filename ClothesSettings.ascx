<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClothesSettings.ascx.cs" Inherits="Cowrie.Modules.ProductList.ClothesSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register assembly="DotNetNuke.WebControls" namespace="DotNetNuke.UI.WebControls" tagPrefix="dnn" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table cellspacing="0" cellpadding="2" border="0" summary="ModuleName1 Settings Design Table">
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="LabelDepartment" runat="server" ControlName="DropDownListDepartments" Suffix=":"
                Text="Department">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:DropDownList ID="DropDownListDepartments" runat="server" DataValueField="Id" DataTextField="Name" Width="250px" AppendDataBoundItems="True">
                <asp:ListItem Value="">All Departments</asp:ListItem>
            </asp:DropDownList>
        </td>
        <td valign="bottom">
            <asp:DropDownList ID="DropDownListMerchantCategories" runat="server" DataValueField="Id" DataTextField="FullName" Width="250px" AppendDataBoundItems="True">
                <asp:ListItem Value="">All Merchant Categories</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
</table>
