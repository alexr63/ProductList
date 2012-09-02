<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportProducts.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.ImportProducts" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register TagPrefix="mgc" Namespace="UserAdmin.Components" Assembly="UserAdmin.Components" %>
<%@ Register Src="~/Controls/UrlControl.ascx" TagName="URL" TagPrefix="dnn" %>
<asp:Panel runat="server" ID="pnlImport" HorizontalAlign="Center">
    <table cellspacing="0" cellpadding="0" border="0">
        <tbody>
            <tr height="25">
                <td class="SubHead" width="250">
        			<dnn:label id="plReferralsFile" runat="server" suffix=":&nbsp;*" controlname="ctlReferralsFile"></dnn:label>
                </td>
                <td width="600">
			        <dnn:URL id="ctlReferralsFile" runat="server" ShowUrls="False" ShowTabs="False" ShowLog="False"
				        ShowTrack="False" ShowUpload="true" required="true" FileFilter="xml"></dnn:URL>
                </td>
            </tr>
        </tbody>
    </table>
</asp:Panel>
<p>
    <asp:ImageButton ID="ImageButtonImport" runat="server" OnClick="ImageButtonImport_Click" Text="Import" />
</p>
