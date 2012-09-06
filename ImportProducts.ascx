<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportProducts.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.ImportProducts" %>
<%@ Register Src="~/Controls/UrlControl.ascx" TagName="URL" TagPrefix="dnn" %>
<%@ Register TagPrefix="wc" Namespace="DotNetNuke.UI.WebControls" Assembly="CountryListBox" %>
<asp:Panel runat="server" ID="pnlImport" HorizontalAlign="Center">
    <table cellspacing="0" cellpadding="0" border="0">
        <tbody>
            <tr height="25">
                <td class="SubHead" width="250">
        			Products File:&nbsp;*
                </td>
                <td width="600">
			        <dnn:URL id="ctlProductsFile" runat="server" ShowUrls="False" ShowTabs="False" ShowLog="False"
				        ShowTrack="False" ShowUpload="true" required="true" FileFilter="xml"></dnn:URL>
                </td>
            </tr>
            <tr height="25">
                <td class="SubHead" width="250">
        			Country:&nbsp;*
                </td>
                <td width="600">
                    <wc:CountryListBox ID="cboCountry" runat="server" Width="200px" CssClass="dnnFormItem"
                        TestIP="" DataValueField="Value" DataTextField="Text" AutoPostBack="False">
                    </wc:CountryListBox>
                    <asp:RequiredFieldValidator ID="valCountry" runat="server" CssClass="NormalRed" Display="Dynamic"
                        ErrorMessage="<br />Please choose your country" ControlToValidate="cboCountry"
                        InitialValue="" ValidationGroup="ImportProducts"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </tbody>
    </table>
</asp:Panel>
<p>
    <asp:Button ID="ButtonImport" runat="server" OnClick="ButtonImport_Click" Text="Import" ValidationGroup="ImportProducts" />
</p>
