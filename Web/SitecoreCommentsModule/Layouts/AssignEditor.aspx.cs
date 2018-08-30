using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.SecurityModel;

namespace Web.SitecoreCommentsModule.Layouts
{
    public partial class AssignEditor : Page
    {
        static List<Item> _listOfItems = new List<Item>();
        static List<Item> _tempList = new List<Item>();
        static readonly Database MasterDb = Factory.GetDatabase("master");
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnSubmit_OnClick(object sender, EventArgs e)
        {

            //var templatePath = dropTemplatePath.SelectedValue;
            var templatePath = txtTemplatePath.Text;
            if (templatePath != "")
            {
                if (MasterDb != null)
                {
                    //Retrieving an item with a path through the database
                    Item myItem = MasterDb.Items[(string) templatePath];
                    if (myItem != null)
                    {
                        _tempList =
                            myItem.Axes.GetDescendants()
                                .Where(i => i.TemplateID.ToString().Equals("{AB86861A-6030-46C5-B394-E8F99E8B87DB}"))
                                .ToList();
                        if (_tempList.Any())
                        {
                            _tempList =
                                (from tempItem in _tempList
                                 where ((TemplateItem)tempItem).StandardValues != null
                                 select (TemplateItem)tempItem).Select(dummy => (Item)dummy).ToList();
                            if (_tempList.Any())
                            {
                                LoadTemplates(_tempList);
                                AssignEditorButton.Visible = true;
                            }
                            else
                            {
                                lblTemplatePath.Text = "Template not Found";
                            }
                        }
                    }

                }
            }
        }
        public void LoadTemplates(List<Item> tempListItem)
        {

            if (!tempListItem.Any()) return;
            GridGetSitecoreItems.DataSource = tempListItem;
            GridGetSitecoreItems.DataBind();

        }

        protected void GridGetSitecoreItems_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {

            Item itmDefaultValue = (Item)e.Row.DataItem;
            Item itm = (Item)e.Row.DataItem;
            if (itm != null)
            {
                TemplateItem itmTemplateItem = itm;
                if (itmTemplateItem.StandardValues != null)
                {
                    itm = itmTemplateItem.StandardValues;
                    if (e.Row.RowType == DataControlRowType.DataRow)
                    {
                        Literal lt = e.Row.FindControl("ltItemName") as Literal;
                        CheckBox cb = e.Row.FindControl("chkPublish") as CheckBox;

                        if (lt != null && itmDefaultValue.Name != null)
                        {

                            itm.Editing.BeginEdit();
                            var editorVal = itm.Fields["__Editors"].Value;
                            if (editorVal.Contains("{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}"))
                            {
                                lt.Text = itmDefaultValue.Name;
                                if (cb != null) cb.Checked = true;
                            }
                            else
                            {
                                lt.Text = itmDefaultValue.Name;
                            }
                            itm.Editing.EndEdit();
                        }
                    }
                }
            }
        }

        protected void AssignEditortoSelectedTemplate(object sender, EventArgs e)
        {
            var itemList = _tempList;
            if (itemList != null)
            {
                foreach (GridViewRow row in GridGetSitecoreItems.Rows)
                {
                    // Access the CheckBox
                    CheckBox cb = (CheckBox)row.FindControl("chkPublish");

                    if (cb != null && cb.Checked)
                    {
                        InsertEditor(itemList[row.DataItemIndex]);
                    }
                    else
                    {
                        RemoveEditor(itemList[row.DataItemIndex]);
                    }

                }
            }
        }

        private void InsertEditor(Item itm)
        {
            using (new SecurityDisabler())
            {
                TemplateItem tempitm = itm;
                itm = tempitm.StandardValues;
                if (itm != null)
                {
                    itm.Editing.BeginEdit();
                    var editorVal = itm.Fields["__Editors"].Value;
                    if (!editorVal.Contains("{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}"))
                    {
                        if (editorVal == "")
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
        private void RemoveEditor(Item itm)
        {
            using (new SecurityDisabler())
            {
                TemplateItem tempitm = itm;
                itm = tempitm.StandardValues;
                if (itm != null)
                {
                    itm.Editing.BeginEdit();
                    var editorVal = itm.Fields["__Editors"].Value;
                    if (editorVal.Contains("{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}"))
                    {
                        itm.Fields["__Editors"].Value = editorVal.Replace("{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}", "").Replace("|{5BA2FC20-45DD-4F32-B66A-85CBAC412DE9}", "");
                    }

                    itm.Editing.EndEdit();
                }
            }
        }


        protected void GridGetSitecoreItems_OnPageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridGetSitecoreItems.PageIndex = e.NewPageIndex;
            LoadTemplates(_tempList);
        }
    }
}