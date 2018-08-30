<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GetComments.aspx.cs" Inherits="Web.SitecoreCommentsModule.AJAX.GetComments" %>

<div class="capability-accordion">
     <asp:Repeater runat="server" ID="rptComments" OnItemDataBound="rptComments_OnItemDataBound">
        <HeaderTemplate>
            <h3>Comments</h3>
            <ul style="list-style: none">
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <h5>
                    <asp:Literal ID="ltAuthor" runat="server"></asp:Literal></h5>
                <span>
                    <asp:Literal ID="ltComment" runat="server"></asp:Literal></span>
                <span>
                    <br />
                    <br />
                    <asp:Literal ID="ltDate" runat="server"></asp:Literal>
                </span>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>

    </asp:Repeater>

    <asp:label runat="server" id="lblMoreComments" cssclass="load-more-count" style="display: none;" clientidmode="Static"></asp:label>
</div>