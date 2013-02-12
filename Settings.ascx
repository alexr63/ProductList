<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Settings.ascx.cs" Inherits="Cowrie.Modules.ProductList.Settings" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="dnn" TagName="TextEditor" Src="~/controls/TextEditor.ascx" %>
<table cellspacing="0" cellpadding="2" border="0" summary="ModuleName1 Settings Design Table">
    <tr>
        <td class="SubHead" width="150" valign="top">
            <dnn:Label ID="lblLocation" runat="server" ControlName="cbLocations" Suffix=":&nbsp;*"
                Text="Location">
            </dnn:Label>
        </td>
        <td valign="bottom">
            <asp:DropDownList ID="cbLocations" runat="server" CssClass="Normal" DataTextField="Name"
                DataValueField="Id" AppendDataBoundItems="True">
                <asp:ListItem Value="" Selected="True">(none)</asp:ListItem>
            </asp:DropDownList>
            <asp:RequiredFieldValidator ID="valLocations" runat="server" ErrorMessage="Location Name required"
                CssClass="NormalRed" ControlToValidate="cbLocations" InitialValue=""></asp:RequiredFieldValidator>
        </td>
    </tr>
</table>
