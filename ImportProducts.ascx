<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportProducts.ascx.cs"
    Inherits="Cowrie.Modules.ProductList.ImportProducts" %>
<%@ Register TagPrefix="dnn" TagName="Label" Src="~/controls/LabelControl.ascx" %>
<%@ Register Src="~/Controls/UrlControl.ascx" TagName="URL" TagPrefix="dnn" %>
<asp:Panel runat="server" ID="pnlImport" HorizontalAlign="Center">
    <table cellspacing="0" cellpadding="0" border="0">
        <tbody>
            <tr height="25">
                <td class="SubHead" width="250">
        			<dnn:label id="plProductsFile" runat="server" suffix=":&nbsp;*" controlname="ctlProductsFile"></dnn:label>
                </td>
                <td width="600">
			        <dnn:URL id="ctlProductsFile" runat="server" ShowUrls="False" ShowTabs="False" ShowLog="False"
				        ShowTrack="False" ShowUpload="true" required="true" FileFilter="xml"></dnn:URL>
                </td>
            </tr>
        </tbody>
    </table>
</asp:Panel>
<p>
    <asp:ImageButton ID="ImageButtonImport" runat="server" OnClick="ImageButtonImport_Click" Text="Import" />
</p>
