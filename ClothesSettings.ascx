<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ClothesSettings.ascx.cs" Inherits="Cowrie.Modules.ProductList.ClothesSettings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register assembly="DotNetNuke.WebControls" namespace="DotNetNuke.UI.WebControls" tagPrefix="dnn" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<table cellspacing="0" cellpadding="2" border="0" summary="ModuleName1 Settings Design Table">
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
            <dnn:Label ID="LabelDepartment" runat="server" ControlName="DropDownListDepartments" Suffix=":"
                Text="Department">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:DropDownList ID="DropDownListDepartments" runat="server" DataValueField="Id" DataTextField="Name" Width="250px" AppendDataBoundItems="True">
                <asp:ListItem Value="">All Departments</asp:ListItem>
            </asp:DropDownList>
        </td>
    </tr>
</table>
