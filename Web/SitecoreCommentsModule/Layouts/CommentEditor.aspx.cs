using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using BlogCommentModule;

namespace Web.SitecoreCommentsModule.Layouts
{
    public partial class CommentEditor : Page
    {
        private readonly BlogRepository _objBlogRepository = new BlogRepository();
         
        protected void Page_Load(object sender, EventArgs e)
        {
            GetCurrentItem();  
            Pub = _objBlogRepository.RetrieveComments(Result, null, null, null).ToList();          
            if (!IsPostBack)
            {
                LoadComments();

            }
        }
       
           
       
       
        public void GetCurrentItem()
        {
            var currentClientRawUrl = Sitecore.Context.RawUrl;
            if (currentClientRawUrl != null)
            {
                int pFrom = currentClientRawUrl.IndexOf("%7B", StringComparison.Ordinal) + "%7B".Length;
                int pTo = currentClientRawUrl.IndexOf("%7D", StringComparison.Ordinal);
                Result = "{" + currentClientRawUrl.Substring(pFrom, pTo - pFrom) + "}";
            }
        }

        private void LoadComments()
        {          
            if (Pub.Any())
            {
                lblcommentstatus.Visible = false;
                GridComments.DataSource = Pub;
                GridComments.DataBind();
                btnPublish.Visible = true;
                btnDelete.Visible = true;
                CheckPublishComment();
            }
            else
            {
                lblcommentstatus.Visible = true;
                lblcommentstatus.Text = "No comments found";
            }
        }
        protected void CheckPublishComment()
        {
         
            if (Pub.Any())
            {
                foreach (GridViewRow row in GridComments.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkPublish");

                    if (cb != null)
                    {
                        var approveStatus = Pub[row.DataItemIndex].Approved;
                        if (approveStatus)
                        {
                            cb.Checked = true;
                        }
                    }

                }
            }
        }
        protected void PublishSelectedComment(object sender, EventArgs e)
        {

            if (Pub.Any())
            {
                foreach (GridViewRow row in GridComments.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkPublish");

                    if (cb != null && cb.Checked && !Pub[row.DataItemIndex].Approved)
                    {
                        Pub[row.DataItemIndex].Approved = true;
                        _objBlogRepository.UpdateComment(Pub[row.DataItemIndex]);
                    }
                    else if (cb != null && !cb.Checked && Pub[row.DataItemIndex].Approved)
                    {
                        Pub[row.DataItemIndex].Approved = false;
                        _objBlogRepository.UpdateComment(Pub[row.DataItemIndex]);
                    }
                }
            }
        }

        protected void DeleteSelectedComment(object sender, EventArgs e)
        {        
            if (Pub.Any())
            {
                foreach (GridViewRow row in GridComments.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkDelete");

                    if (cb != null && cb.Checked)
                    {
                        _objBlogRepository.DeleteComment(Pub[row.DataItemIndex].Id);
                    }

                }
            }
        }

        protected void GridComments_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridComments.PageIndex = e.NewPageIndex;
            LoadComments();
        }

        protected void CheckAll(object sender, EventArgs e)
        {

            CheckBox chkbox = (CheckBox)sender;
            String chkId = chkbox.ID;
            if (chkbox.Checked && chkId != null)
            {

                CheckAll_Click(chkId);
            }
            else if (!chkbox.Checked && chkId != null)
            {
                UncheckAll_Click(chkId);
            }
        }
        protected void CheckAll_Click(string id)
        {
            ToggleCheckState(true, id);
        }
        protected void UncheckAll_Click(string id)
        {
            ToggleCheckState(false, id);
        }
        private void ToggleCheckState(bool checkState, string id)
        {
            string chkvalue = string.Empty;
            if (id == "chkboxSelectAll")
            {
                chkvalue = "chkPublish";
            }
            if (id == "chkboxDeleteAll")
            {
                chkvalue = "chkDelete";
            }
            // Iterate through the Products.Rows property
            foreach (GridViewRow row in GridComments.Rows)
            {
                // Access the CheckBox
                CheckBox cb = (CheckBox)row.FindControl(chkvalue);
                if (cb != null)
                    cb.Checked = checkState;
            }
        }
        public string Result { get; set; }

        public List<Comment> Pub { get; set; }
    }
}