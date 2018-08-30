using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BlogCommentModule;

namespace Web.SitecoreCommentsModule.AJAX
{
    public partial class GetComments : Page
    {
        readonly BlogRepository _objBlogRepository = new BlogRepository();
        protected void Page_Load(object sender, EventArgs e)
        {
            var currentItem = Request.QueryString["itemID"];
            var totalcount = int.Parse(Request.QueryString["totalcount"]);
            var itemsperpage = int.Parse(Request.QueryString["itemsperpage"]);
            var comments = _objBlogRepository.Retrieve(currentItem, true, null, null);
            if (comments.Any())
            {
                int countDiffernce = comments.Count() - (totalcount + itemsperpage);
                lblMoreComments.Text = countDiffernce > 0
                    ? (_objBlogRepository.RetrieveComments(currentItem, true, null, null).Count() -
                       (totalcount + itemsperpage)).ToString()
                    : "0";

                rptComments.DataSource = comments.Skip(totalcount).Take(itemsperpage);
                rptComments.DataBind();
            }
        }
        protected void rptComments_OnItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                var itm = (Comment)e.Item.DataItem;
                if (itm == null) return;
                Literal ltAuthor = (Literal)e.Item.FindControl("ltAuthor");
                Literal ltComment = (Literal)e.Item.FindControl("ltComment");
                Literal ltDate = (Literal)e.Item.FindControl("ltDate");
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
                    ltDate.Text = itm.Date.ToString(CultureInfo.InvariantCulture);
                }


            }
        }
    }
}