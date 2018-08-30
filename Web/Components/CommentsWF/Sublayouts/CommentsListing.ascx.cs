using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Sitecore.Data;
using xDBCommentsManager;
using Sitecore.StringExtensions;

namespace Web.Components.CommentsWF.Sublayouts
{
   
    public partial class CommentsListing : UserControl
    {
        public static GetSettings setting { get; set; }        
        readonly ICommentRepository _objRepository = new CommentRepository();
        private readonly ID _currentItem = Sitecore.Context.Item.ID;
        int _resultOnPageLoad, _resultPerClick;
        private string siteKey = "";
        GetSettings getSettings = new GetSettings();               
        /// <summary>
        /// Display the comments on page load.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            // GetSettings getSettings=new GetSettings();
            var settingsValue = getSettings.GetSetting();
            siteKey = settingsValue.SiteKey;
            if (!siteKey.IsNullOrEmpty())
            {
                hfSitekey.Value = siteKey;
                //recaptchaPanel.Attributes.Add("data-sitekey", siteKey);
            }
            GetCommonSettings objCommonSettings = new GetCommonSettings();
            var setting = objCommonSettings.GetCommonSetting();
            _resultOnPageLoad = setting.CommentOnPageLoad;
            _resultPerClick = setting.CommentOnLoadMore;
            hfGUID.Value = Sitecore.Context.Item.ID.ToString();
            hfResultsPerClick.Value = _resultPerClick.ToString();
            hftotalRecords.Value = _objRepository.Retrieve(_currentItem.ToString(), true, null, null, "Ascending", "00000000-0000-0000-0000-000000000000").Count().ToString();
            hfresultOnPageLoad.Value = _resultOnPageLoad.ToString();
            if (_currentItem.ToString() != "")
            {
                var comments = _objRepository.Retrieve(_currentItem.ToString(), true, null, null, "Ascending", "00000000-0000-0000-0000-000000000000");
                if (comments.Any())
                {
                    rptComments.DataSource = comments.Take(_resultOnPageLoad);
                    rptComments.DataBind();
                }
            }
        }

        /// <summary>
        /// Data bind of repeator.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void rptComments_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            GetCommonSettings objCommonSettings = new GetCommonSettings();
            var setting = objCommonSettings.GetCommonSetting();
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                int index = 2;
                var itm = (Comment)e.Item.DataItem;
                if (itm == null) return;
                Literal ltAuthor = (Literal)e.Item.FindControl("ltAuthor");
                Literal ltComment = (Literal)e.Item.FindControl("ltComment");
                Literal ltDate = (Literal)e.Item.FindControl("ltDate");
                var parentLi = (HtmlGenericControl)e.Item.FindControl("parentLi");
                HyperLink replyCommentId = (HyperLink)e.Item.FindControl("replyCommentId");
                if (ltAuthor != null)
                {
                    ltAuthor.Text = itm.Author;
                }
                if (ltAuthor != null)
                {
                    ltComment.Text = itm.Body;
                }
                if (ltAuthor != null)
                {
                    ltDate.Text = itm.Date.ToString("f");
                }
               
                if (itm.CommentId != "" && setting.Enable.ToLower()=="true")
                {
                    if (replyCommentId != null)
                    {
                        replyCommentId.Visible = true;
                        replyCommentId.Attributes.Add("parentcomment-id", itm.CommentId);
                    }
                    var child =
                        _objRepository.Retrieve(_currentItem.ToString(), true, null, null, "Ascending", itm.CommentId)
                            .ToList();
                    if (child.Any())
                    {                        
                        var listItem = RenderCommentListing(child, index,setting.Level);
                        if (listItem != null)
                        {
                            Repeater rptSecondLevel = listItem;
                            parentLi.Controls.Add(rptSecondLevel);
                        }
                    }
                   
                }
            }
        }
        int _oldindex;
        int _newindex;
        private Repeater RenderCommentListing(List<Comment> list, int index, int replyLevel, bool hasChild = false)
        {
            _newindex = index;
            if (!hasChild)
            {
                _oldindex = index;
            }
            Repeater rptcommentListing = new Repeater();
            rptcommentListing.DataSource = list;
            rptcommentListing.DataBind();
            var ulTag = new HtmlGenericControl("ul class='comment-Container'");
            foreach (RepeaterItem rptItem in rptcommentListing.Items)
            {
                                             
                Comment itm =((IList<Comment>)rptcommentListing.DataSource)[rptItem.ItemIndex];
                HtmlGenericControl liTag;
                if (hasChild)
                {
                     liTag = new HtmlGenericControl("li class='comment-content'" + "data-level=level-" + _newindex);
                }
                else
                {
                    _newindex = _oldindex;
                    liTag = new HtmlGenericControl("li class='comment-content'" + "data-level=level-" + _oldindex);
                }
                var h5Tag = new HtmlGenericControl("h5 class='comment-author-name'");
                var spanTag = new HtmlGenericControl("span class='comment-time'");
                var spanCommentBodyTag = new HtmlGenericControl("span class='comment-body'");
                var replyTag= new HtmlGenericControl("a id='replyCommentId' class='replyComment'");
                var panel = new HtmlGenericControl("div class='panel'");
                replyTag.InnerHtml = "Reply";
                replyTag.Attributes.Add("parentcomment-id", itm.CommentId);
                spanCommentBodyTag.InnerHtml = itm.Body;
                spanTag.InnerHtml = itm.Date.ToString("f");
                h5Tag.InnerHtml = "By "+itm.Author +" on ";
                if (rptItem.ItemIndex == 0)
                {
                    rptItem.Controls.Add(ulTag);
                }
                ulTag.Controls.Add(liTag);
                liTag.Controls.Add(h5Tag);
                h5Tag.Controls.Add(spanTag);
                liTag.Controls.Add(spanCommentBodyTag);
                if (_newindex < replyLevel)
                {
                    liTag.Controls.Add(replyTag);
                    liTag.Controls.Add(panel);
                    if (itm.CommentId != "")
                    {
                        var child = _objRepository.Retrieve(_currentItem.ToString(), true, null, null, "Ascending",
                            itm.CommentId).ToList();
                        _newindex++;
                        var listItem = RenderCommentListing(child, _newindex, replyLevel, true);
                        hasChild = false;
                        if (child.Any())
                        {
                            if (listItem != null)
                            {
                                Repeater rptlisting = listItem;
                                liTag.Controls.Add(rptlisting);
                            }
                        }
                    }
                }
                
            }

            return rptcommentListing;
        }

    }
}