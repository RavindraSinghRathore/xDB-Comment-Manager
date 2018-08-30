define(["sitecore","jquery"], function (Sitecore,$) {
    var BlogCreator = Sitecore.Definitions.App.extend({
        initialized: function () {
            alert("1");
            this.btnGetTemplate.on("click", function () {               
                var myObj = {
                    Ids: this.tvSitecore.viewModel.checkedItemIds._latestValue
                };
                alert(JSON.stringify(myObj));
               // debugger;
                if (myObj != null) {

                    $.ajax({
                        type: "POST",
                        url: "/api/sitecore/Comment/Assign",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: JSON.stringify(myObj),
                        success: function (data) {
                            alert(data);
                            if (data == 'ok') {

                                alert('Success! your blog has been created.');
                            }
                            else {
                                //alert(data);
                                alert('Failed to create blog, check Sitecore logs for errors.');
                            }
                        },
                        error: function (xhr, textStatus, errorThrown) {
                            alert('Failed to create blog, check Sitecore logs for errors. : ' + errorThrown);
                        }
                    });
                }
            }, this);
        }
    });
    return BlogCreator;
});