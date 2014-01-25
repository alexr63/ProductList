<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EditHotel.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.EditHotel" %>
<%@ Register TagPrefix="dnn" Assembly="DotNetNuke" Namespace="DotNetNuke.UI.WebControls" %>
<%@ Register TagPrefix="dnn" TagName="texteditor" Src="~/controls/texteditor.ascx" %>
<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>
<%@ Register TagPrefix="dnnweb" Namespace="DotNetNuke.Web.UI.WebControls" Assembly="DotNetNuke.Web" %>

<table border="0" cellpadding="2" cellspacing="2" summary="Edit Hotel Design Table">
    <tr>
        <td class="SubHead" valign="top" width="150">Name
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxName" runat="server" CssClass="NormalTextBox" MaxLength="250"
                Width="525"></asp:TextBox>
            <asp:RequiredFieldValidator ID="RequiredFieldValidatorUserName" runat="server" CssClass="NormalRed"
                Display="Dynamic" ErrorMessage="Name is required" ControlToValidate="TextBoxName">*</asp:RequiredFieldValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">Number
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxNumber" runat="server" CssClass="NormalTextBox" MaxLength="50"
                Width="325"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">Unit Cost
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
        <td class="SubHead" valign="top" width="150">Description
        </td>
        <td valign="top">
            <dnn:texteditor ID="txtDescription" runat="server" Height="400" Width="100%" HtmlEncode="False" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">Extra Description
        </td>
        <td valign="top">
            <dnn:texteditor ID="txtExtraDescription" runat="server" Height="400" Width="100%" HtmlEncode="False" />
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">URL
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxURL" runat="server" CssClass="NormalTextBox" MaxLength="250"
                Width="525"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">Image
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxImage" runat="server" CssClass="NormalTextBox" MaxLength="250"
                Width="525"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">Current Location
        </td>
        <td valign="bottom">
            <asp:Label ID="LabelCurrentLocation" runat="server"></asp:Label>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">Location
        </td>
        <td valign="bottom">
            <telerik:RadTreeView ID="RadTreeViewLocations" runat="server" Skin="Default" Height="200px" Width="325px"
                OnNodeExpand="RadTreeViewLocations_NodeExpand">
            </telerik:RadTreeView>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">Rooms
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxRooms" runat="server" CssClass="NormalTextBox" MaxLength="10"
                Width="50"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidatorRooms" runat="server" CssClass="NormalRed"
                ErrorMessage="Please enter Rooms in valid format"
                ControlToValidate="TextBoxRooms" Display="Dynamic" Operator="DataTypeCheck"
                Type="Integer">*</asp:CompareValidator>
            <asp:RangeValidator ID="RangeValidatorRooms" CssClass="NormalRed" runat="server" ErrorMessage="Please enter Rooms more or equal 0" MinimumValue="0" Type="Integer" ControlToValidate="TextBoxRooms" Text="*" Display="Dynamic" MaximumValue="10000"></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">Star
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxStar" runat="server" CssClass="NormalTextBox" MaxLength="10"
                Width="50"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidatorStar" runat="server" CssClass="NormalRed"
                ErrorMessage="Please enter Star in valid format"
                ControlToValidate="TextBoxStar" Display="Dynamic" Operator="DataTypeCheck"
                Type="Double">*</asp:CompareValidator>
            <asp:RangeValidator ID="RangeValidatorStar" CssClass="NormalRed" runat="server" ErrorMessage="Please enter Star in range from 0 to 5" MaximumValue="5" MinimumValue="0" Type="Double" ControlToValidate="TextBoxStar" Text="*" Display="Dynamic"></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">Customer Rating
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxCustomerRating" runat="server" CssClass="NormalTextBox" MaxLength="10"
                Width="50"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidatorCustomerRating" runat="server" CssClass="NormalRed"
                ErrorMessage="Please enter Customer Rating in valid format"
                ControlToValidate="TextBoxCustomerRating" Display="Dynamic" Operator="DataTypeCheck"
                Type="Double">*</asp:CompareValidator>
            <asp:RangeValidator ID="RangeValidatorCustomerRating" CssClass="NormalRed" runat="server" ErrorMessage="Please enter Customer Rating in range from 0 to 5" MaximumValue="5" MinimumValue="0" Type="Double" ControlToValidate="TextBoxCustomerRating" Text="*" Display="Dynamic"></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">Address
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxAddress" runat="server" CssClass="NormalTextBox"
                Width="525" TextMode="MultiLine" Rows="5"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">Currency Code
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxCurrencyCode" runat="server" CssClass="NormalTextBox"
                Width="325" MaxLength="3"></asp:TextBox>
            <asp:CustomValidator ID="CustomValidatorCurrencyCode" runat="server" ErrorMessage="Please enter Currency Code in valid format" ControlToValidate="TextBoxCurrencyCode" Display="Dynamic" OnServerValidate="CustomValidatorCurrencyCode_ServerValidate">*</asp:CustomValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">Lat
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxLat" runat="server" CssClass="NormalTextBox" MaxLength="50"
                Width="325"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidatorLat" runat="server" CssClass="NormalRed"
                ErrorMessage="Please enter Lat in valid format"
                ControlToValidate="TextBoxLat" Display="Dynamic" Operator="DataTypeCheck"
                Type="Double">*</asp:CompareValidator>
            <asp:RangeValidator ID="RangeValidatorLat" CssClass="NormalRed" runat="server" ErrorMessage="Please enter Lat in range from -90 to +90" MaximumValue="+90" MinimumValue="-90" Type="Double" ControlToValidate="TextBoxLat" Text="*" Display="Dynamic"></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">Lon
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxLon" runat="server" CssClass="NormalTextBox" MaxLength="50"
                Width="325"></asp:TextBox>
            <asp:CompareValidator ID="CompareValidatorLon" runat="server" CssClass="NormalRed"
                ErrorMessage="Please enter Lon in valid format"
                ControlToValidate="TextBoxLon" Display="Dynamic" Operator="DataTypeCheck"
                Type="Double">*</asp:CompareValidator>
            <asp:RangeValidator ID="RangeValidatorLon" CssClass="NormalRed" runat="server" ErrorMessage="Please enter Lon in range from -180 to +180" MaximumValue="+180" MinimumValue="-180" Type="Double" ControlToValidate="TextBoxLon" Text="*" Display="Dynamic"></asp:RangeValidator>
        </td>
    </tr>
    <tr>
        <td class="SubHead" valign="top" width="150">Post Code
        </td>
        <td class="NormalTextBox">
            <asp:TextBox ID="TextBoxPostCode" runat="server" CssClass="NormalTextBox"
                Width="325" MaxLength="10"></asp:TextBox>
        </td>
    </tr>
    <tr>
        <td class="SubHead" width="150" valign="top">Type
        </td>
        <td valign="bottom">
            <asp:DropDownList ID="DropDownListTypes" runat="server" DataTextField="Name" DataValueField="Id"></asp:DropDownList>
        </td>
    </tr>
</table>
<p>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
</p>
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
