<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHotel.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.EditHotel" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="dnnweb" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<table border="0" cellpadding="2" cellspacing="2" summary="Edit Hotel Design Table">
    <tr>
        <td class="SubHead" valign="top" width="150">
            Name
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxName" runat="server" CssClass="NormalTextBox" MaxLength="250"
                Width="325"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorUserName" runat="server" CssClass="NormalRed"
                Display="Dynamic" ErrorMessage="Name is required" ControlToValidate="TextBoxName">*</asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">
            Number
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxNumber" runat="server" CssClass="NormalTextBox" MaxLength="50"
                Width="325"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">
            Unit Cost
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxUnitCost" runat="server" CssClass="NormalTextBox" MaxLength="10"
                Width="325"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidatorUnitCost" runat="server" CssClass="NormalRed"
                ErrorMessage="Please enter Unit Cost in valid format"
                ControlToValidate="TextBoxUnitCost" Display="Dynamic" Operator="DataTypeCheck"
                Type="Currency">*</asp:CompareValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">
            Description
        </td>
        <td valign="top">
            <dnn:texteditor ID="txtDescription" runat="server" Height="400" Width="100%" HtmlEncode="False" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">
            URL
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxURL" runat="server" CssClass="NormalTextBox" MaxLength="250"
                Width="325"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">
            Image
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxImage" runat="server" CssClass="NormalTextBox" MaxLength="250"
                Width="325"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            Current Location
        </td>
        <td valign="bottom">
            <asp:Label ID="LabelCurrentLocation" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            Location
        </td>
        <td valign="bottom">
            <telerik:RadTreeView ID="RadTreeViewLocations" runat="server" Skin="Default" Height="200px" Width="250px"
                OnNodeExpand="RadTreeViewLocations_NodeExpand">
            </telerik:RadTreeView>
            <dnnweb:DnnTreeView ID="DnnTreeViewLocations" runat="server" OnNodeExpand="DnnTreeViewLocations_NodeExpand" EnableViewState="true" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">
            Type
        </td>
        <td valign="bottom">
            <asp:DropDownList ID="DropDownListTypes" runat="server" DataTextField="Name" DataValueField="Id"></asp:DropDownList>
        </td>
    </tr>
</table>
<p>
    <dnn:CommandButton ID="cmdSave" runat="server" class="CommandButton" ResourceKey="cmdSave"
        ImageUrl="~/images/save.gif" oncommand="cmdSave_Command" />
    &nbsp;
    <dnn:CommandButton ID="cmdCancel" runat="server" class="CommandButton" ResourceKey="cmdCancel"
        CausesValidation="False" ImageUrl="~/images/lt.gif" 
        oncommand="cmdCancel_Command" />
    &nbsp;
    <dnn:CommandButton ID="cmdDelete" runat="server" class="CommandButton" ResourceKey="cmdDelete"
        CausesValidation="False" ImageUrl="~/images/action_delete.gif" oncommand="cmdDelete_Command" />
</p>
