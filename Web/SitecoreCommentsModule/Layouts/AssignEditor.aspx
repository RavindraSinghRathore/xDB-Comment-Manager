<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AssignEditor.aspx.cs" Inherits="Web.SitecoreCommentsModule.Layouts.AssignEditor" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Assign Editor to templates</title>
    <style type="text/css">
         .txtBox-container input[type="text"], .basic-grey input[type="email"], .basic-grey textarea, .basic-grey select {
            border: 1px solid #DADADA;
            color: #888;
            height: 30px;
            margin-bottom: 16px;
            margin-right: 6px;
            margin-top: 2px;
            outline: 0 none;
            padding: 3px 3px 3px 5px;
            width: 70%;
            font-size: 12px;
            line-height: 15px;
            box-shadow: inset 0px 1px 4px #ECECEC;
            -moz-box-shadow: inset 0px 1px 4px #ECECEC;
            -webkit-box-shadow: inset 0px 1px 4px #ECECEC;
        }
         .txtBox-container span {
                float: left;
                width: 20%;
                text-align: right;
                padding-right: 10px;
                margin-top: 10px;
                color: #888;
            }
        .button {
            background: #E27575;
            border: none;
            padding: 10px 25px 10px 25px;
            color: #FFF;
            box-shadow: 1px 1px 5px #B6B6B6;
            border-radius: 3px;
            text-shadow: 1px 1px 1px #9E3F3F;
            cursor: pointer;
        }

            .button:hover {
                background: #CF7A7A;
            }
            .assign-editor-container table {
                margin: 0 auto;
                width: 100%;
            }
            .assign-editor-container table td {
                 font-size: 16px;
                  color: #888;
            }
            .assign-editor-container .button {
                margin-top: 10px;
                /*margin: 0 auto;*/
                display: block;
            }
             .assign-editor-container .lblassign {
                 font-size: 18px;
                 text-align: left;
             }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="txtBox-container">
            <asp:Label ID="lblTemplatePath" runat="server">Please Enter the Templates Path(Ex.-/sitecore/templates/User Defined)</asp:Label><br />
            <asp:TextBox ID="txtTemplatePath" runat="server"></asp:TextBox>
            <asp:Button CssClass="button" ID="btnSubmit" runat="server" Text="Submit" OnClick="btnSubmit_OnClick" />
        </div>
        <div class="assign-editor-container">
            <asp:GridView ID="GridGetSitecoreItems" runat="server"
                AutoGenerateColumns="false" HeaderStyle-BackColor="#888888" PageSize="10" HeaderStyle-ForeColor="WhiteSmoke" AllowPaging="True" OnRowDataBound="GridGetSitecoreItems_OnRowDataBound" OnPageIndexChanging="GridGetSitecoreItems_OnPageIndexChanging">
                <Columns>
                    <asp:TemplateField>
                        <HeaderTemplate>
                            <asp:Label ID="AssignEditor" CssClass="lblassign" runat="server" Text="Assign Editor" />
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkPublish" runat="server" AutoPostBack="False" />
                            <asp:Literal ID="ltItemName" runat="server"></asp:Literal>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
            <asp:Button ID="AssignEditorButton" runat="server" Text="Assign" Visible="False" OnClick="AssignEditortoSelectedTemplate" CssClass="button" Width="100px" />
        </div>
    </form>
</body>
</html>
