using System;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.SecurityModel;
using SitecoreCommentsModule;


namespace Web.ApiControllers
{
    public class CommentController : Controller, IController
    {
        private readonly ICommentRepository _objRepository = new CommentRepository();

        [HttpPost]
        public ActionResult NewComment(CommentAttributes commentAttributes)
        {
            var itmUrl =
                new UriBuilder(LinkManager.GetItemUrl(Context.Database.GetItem(commentAttributes.BlogPostId),
                    new UrlOptions { AlwaysIncludeServerUrl = true }));
            try
            {
                Comment cmt = new Comment
                {
                    PostId = commentAttributes.BlogPostId,
                    Author = commentAttributes.AuthorName,
                    Email = commentAttributes.AuthorEmail,
                    Date = DateTime.Now,
                    Body = commentAttributes.AuthorComment
                };
                _objRepository.Insert(cmt);
                if (Settings.GetSetting("GET_NEW_COMMENT_NOTIFICATION_TO_ADMIN") == "true")
                {
                    string emailFrom = Settings.GetSetting("EMAIL_FROM");
                    string emailTo = Settings.GetSetting("EMAIL_TO");
                    string greetingmsgSubject = Settings.GetSetting("GREETING_MESSAGE_SUBJECT_FOR_ADMIN") != ""
                        ? Settings.GetSetting("GREETING_MESSAGE_SUBJECT_FOR_ADMIN")
                        : "New comment posted on your blogpost.";
                    string greetingmsgBody = Settings.GetSetting("GREETING_MESSAGE_BODY_FOR_ADMIN") != ""
                        ? Settings.GetSetting("GREETING_MESSAGE_BODY_FOR_ADMIN")
                        : "New Comment Posted on " + Context.Database.GetItem(commentAttributes.BlogPostId).DisplayName +
                          " Blog Post";
                    if (emailFrom != "" && emailTo != "")
                    {
                        Utility.SendMail(emailFrom, emailTo, greetingmsgSubject, greetingmsgBody);
                    }
                }
                var uri = AddQuery(itmUrl, "status", "success");
                Response.Redirect(uri.ToString());
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, this);
                var errorUri = AddQuery(itmUrl, "status", "error");
                Response.Redirect(errorUri.ToString());
            }

            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Assign(TemplateIds templateIds)
        {
            string sitecoreTempItemIds = templateIds.Ids;
            string[] itmIds = sitecoreTempItemIds.Split('|');
            StringBuilder sb = new StringBuilder();

            sb.Append("template Ids: " + sitecoreTempItemIds + "  ");

            foreach (string itm in itmIds)
            {
                Database master = Factory.GetDatabase("master");
                Item item = master.GetItem(new ID(itm));


                try
                {
                    sb.Append("Item " + item.DisplayName + "  ");
                    TemplateItem itmTemplateItem = item;
                    sb.Append("Item Template " + itmTemplateItem.DisplayName + "  ");
                    if (itmTemplateItem.StandardValues != null)
                    {
                        item = itmTemplateItem.StandardValues;
                        InsertEditor(item);
                    }

                }
                catch (Exception ex)
                {
                    sb.Append("Error found: " + ex.Message);
                    Log.Error(ex.Message, this);
                }

            }
            return Json("ok" + " " + sb, JsonRequestBehavior.AllowGet);
        }

        private void InsertEditor(Item itm)
        {
            using (new SecurityDisabler())
            {
                //TemplateItem tempitm = itm;
                //itm = tempitm.StandardValues;
                if (itm != null)
                {
                    itm.Editing.BeginEdit();
                    var editorVal = itm.Fields["__Editors"].Value;
                    if (!editorVal.Contains("{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}"))
                    {
                        if (String.IsNullOrEmpty(editorVal))
                        {
                            itm.Fields["__Editors"].Value = editorVal + "{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}";
                            //Set Editors 
                        }
                        else
                        {
                            itm.Fields["__Editors"].Value = editorVal + "|{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}";
                            //Set Editors 
                        }

                    }

                    itm.Editing.EndEdit();
                }
            }
        }

        [HttpPost]
        public ActionResult GetLoadMoreData(LoadMore loadMore)
        {
            try
            {
                var currentItem = loadMore.itemID;
                var totalcount = int.Parse(loadMore.totalcount);
                var itemsperpage = int.Parse(loadMore.itemsperpage);
                var comments = _objRepository.Retrieve(currentItem, true, null, null, "");
                return Json(comments.Skip(totalcount).Take(itemsperpage));


            }
            catch (Exception e)
            {
                return Json(new { success = false, ex = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        public static Uri AddQuery(UriBuilder uri, string name, string value)
        {
            var ub = new UriBuilder(uri.Uri);

            // decodes urlencoded pairs from uri.Query to HttpValueCollection
            var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

            httpValueCollection.Add(name, value);

            // urlencodes the whole HttpValueCollection
            ub.Query = httpValueCollection.ToString();

            return ub.Uri;
        }
    }

    public class CommentAttributes
    {
        public string BlogPostId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorEmail { get; set; }
        public string AuthorComment { get; set; }
    }

    public class TemplateIds
    {
        public string Ids { get; set; }
    }

    public class LoadMore
    {
        public string itemID { get; set; }
        public string itemsperpage { get; set; }
        public string totalcount { get; set; }
        public string pagenumber { get; set; }
    }
}