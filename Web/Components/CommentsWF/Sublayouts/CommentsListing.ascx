<%@ Control Language="C#" AutoEventWireup="true" CodeFile="CommentsListing.ascx.cs" Inherits="Web.Components.CommentsWF.Sublayouts.CommentsListing" %>

<script src="https://www.google.com/recaptcha/api.js?onload=CaptchaCallback&render=explicit" async defer></script>

<div class="basic-grey">
    <div class="blog-container">
        <asp:Repeater runat="server" ID="rptComments" OnItemDataBound="rptComments_OnItemDataBound">
            <HeaderTemplate>
                <h3 class="comment-header">Comments</h3>
                <ul class="comment-Container">
            </HeaderTemplate>
            <ItemTemplate>
                <li id="parentLi" runat="server" class="comment-content" data-level="level-1">
                    <h5 class="comment-author-name">By
                        <asp:Literal ID="ltAuthor" runat="server"></asp:Literal>
                        on  <span class="comment-time">
                            <asp:Literal ID="ltDate" runat="server"></asp:Literal>
                        </span></h5>
                    <span class="comment-body">
                        <asp:Literal ID="ltComment" runat="server"></asp:Literal></span>
                    <asp:HyperLink CssClass="replyComment" ID="replyCommentId" ClientIDMode="Static" runat="server" Visible="False">Reply</asp:HyperLink>
                    <asp:Panel CssClass="panel" ID="pnlForm" runat="server">
                    </asp:Panel>
                </li>
            </ItemTemplate>
            <FooterTemplate>
                </ul>
            </FooterTemplate>

        </asp:Repeater>
        <div class="write-commment-container" id="commentsContainer" style="display: none">
            <div class="comment-comment-append">
                <div class="author-inner-container">
                    <label>
                        <span>Your Name :</span>
                        <asp:TextBox CssClass="txt-author-name" ID="AuthorName" runat="server" ClientIDMode="Static" placeholder="Your Full Name"></asp:TextBox>
                    </label>
                </div>
                <div class="email-inner-container">
                    <label>
                        <span>Your Email :</span>

                        <asp:TextBox CssClass="txt-email" ID="AuthorEmail" TextMode="Email" ClientIDMode="Static" placeholder="Valid Email Address" runat="server"></asp:TextBox>
                    </label>
                </div>
                <div class="comment-inner-container">
                    <label>
                        <span>Comment :</span>
                        <asp:TextBox CssClass="txt-comment-body" ID="AuthorComment" TextMode="MultiLine" ClientIDMode="Static" Columns="20" Rows="5" placeholder="Your Comment" runat="server"></asp:TextBox>
                    </label>
                </div>
                <div class="clear"></div>
                <div class="google-recaptcha-container"></div>
                <div class="submit-container">
                    <a id="submitButton" class="submit-button">Reply</a>
                    <a id="btnCancel" class="cancel-button">Cancel</a>
                </div>
            </div>
        </div>
    </div>
    <a class="load_more-button" id="Btn-More"></a>
</div>
<asp:HiddenField runat="server" ID="hfResultsPerClick" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hfGUID" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hftotalRecords" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hfresultOnPageLoad" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hfParentCommentID" ClientIDMode="Static" />
<asp:HiddenField runat="server" ID="hfSitekey" ClientIDMode="Static" />

