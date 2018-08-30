$(window).load(function () {
    if ($('#hfSitekey').val() != "") {
        var captchaContainer = null;
        var loadCaptcha = function () {
            captchaContainer = grecaptcha.render('newComment', {
                'sitekey': $('#hfSitekey').val()
            });
        };
        loadCaptcha();
    }
});

$(document).ready(function () {

    var $captchaContainer = null;
    $(document).on("click", ".replyComment", function () {

        var $replyForm = $('.write-commment-container').html();
        var $this = $(this);
        var $panel = $this.parent(".comment-content").find('> .panel');
        if (!$panel.hasClass("form-append")) {
            $panel.append($replyForm);
            var $commentsId = $this.attr('parentcomment-id');
            var $submitContainer = $panel.find('.google-recaptcha-container').attr('id', $commentsId);
            if ($('#hfSitekey').val() != "") {
                var loadCaptcha = function () {
                    $captchaContainer = grecaptcha.render($submitContainer.attr('id'), {
                        'sitekey': $('#hfSitekey').val()
                    });
                };
                loadCaptcha();
            }
            $panel.addClass("form-append");
            var $parentId = $this.attr("parentcomment-id");
            $('#hfParentCommentID').val($parentId);

        } else {
            $panel.html("");
            $panel.removeClass("form-append");
        }
        if ($this.hasClass('active')) {
            $this.removeClass("active");
        } else {
            $this.addClass("active");
        }
        var panel = this.nextElementSibling;
        if (panel.style.maxHeight) {
            panel.style.maxHeight = null;
        } else {
            panel.style.maxHeight = panel.scrollHeight + "px";
        }
    });

    $(".basic-grey").on("click", " .cancel-button", function () {
        var $this = $(this);
        var $btnReply = $this.closest(".comment-content").find(' >.replyComment');
        $btnReply.click();
    });

    //$(".basic-grey").on('click', '.submit-button', function () {

    //    var name = $("#AuthorName").val();
    //    var email = $("#AuthorEmail").val();
    //    var comment = $("#AuthorComment").val();
    //    var currentItem = $("#hfGUID").val();
    //    var hfParentCommentID = $("#hfParentCommentID").val();
    //    var CaptchaResponse = grecaptcha.getResponse($captchaContainer);
    //    alert(CaptchaResponse);
    //    $.ajax({
    //        type: "POST",
    //        url: "/Components/CommentsWF/Handler/SubmitHandler.ashx",
    //        data: { Name: name, Email: email, Comment: comment, CurrentItem: currentItem, hfParentCommentId: hfParentCommentID, captchaResponse: CaptchaResponse },
    //        dataType: "json",
    //        success: function (response) {
    //            alert(response.d);
    //        }
    //    });
    //});


    function isEmail(email) {
        var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        return regex.test(email);
    }

    function validateData(myObj, $this) {
        var validate = false;       
        var lastwidth = $this.parent().parent().parent().height();
        var finalheight = lastwidth;
        var name = true;
        var email = true;
        var comment = true;
        if (myObj.Name.length <= 0) {           
            if ($this.parent().parent().find("#AuthorName").next(".validation").length == 0) // only add if not added
            {
                finalheight = finalheight + 40;
                $this.parent().parent().parent().css('max-height', finalheight + 'px');
                $this.parent().parent().find("#AuthorName").after("<div class='validation' style='color:red;margin-bottom: 20px;'>Please enter your Name</div>");
                
            }
            
        }
        else {
            name = true;           
            $this.parent().parent().find("#AuthorName").next(".validation").remove(); // remove it
        }
        if (!isEmail(myObj.Email)) {
            email = false;
            if ($this.parent().parent().find("#AuthorEmail").next(".validation").length == 0) // only add if not added
            {
                finalheight = finalheight + 40;
                $this.parent().parent().parent().css('max-height', finalheight + 'px');
                $this.parent().parent().find("#AuthorEmail").after("<div class='validation' style='color:red;margin-bottom: 20px;'>Please enter valid email address</div>");
                
            }
            
        }
        else {
            email = true;
            $this.parent().parent().find("#AuthorEmail").next(".validation").remove(); // remove it
        }
        if (myObj.Comment.length <= 0) {
            comment = false;
            if ($this.parent().parent().find("#AuthorComment").next(".validation").length == 0) // only add if not added
            {
                finalheight = finalheight + 40;
                $this.parent().parent().parent().css('max-height', finalheight + 'px');
                $this.parent().parent().find("#AuthorComment").after("<div class='validation' style='color:red;margin-bottom: 20px;'>Please enter Comment</div>");
               
            }
           
        }
        else {
            comment = true;
            $this.parent().parent().find("#AuthorComment").next(".validation").remove(); // remove it
        }
        if (name && email && comment) {
            validate = true;
        }
        return validate;
    }



    $(".basic-grey").on('click', '.submit-button', function () {
        var $this = $(this);
        var $parentEle = $this.parent().parent();
        var name = $parentEle.find(".txt-author-name").val();
        var email = $parentEle.find(".txt-email").val();
        var comment = $parentEle.find(".txt-comment-body").val();
        var currentItem = $("#hfGUID").val();
        var hfParentCommentID = $("#hfParentCommentID").val();
        var CaptchaResponse = "";
        if ($('#hfSitekey').val() != "") {
            CaptchaResponse = grecaptcha.getResponse($captchaContainer);
        }

        var myObj = {
            Name: name,
            Email: email,
            Comment: comment,
            CurrentItem: currentItem,
            hfParentCommentId: hfParentCommentID,
            captchaResponse: CaptchaResponse
        };
        if (validateData(myObj, $this)) {
            $this.text("Submitting...");
            $.ajax({
                type: "POST",
                url: '/api/sitecore/Comment/ReplyComment',
                contentType: "application/json; charset=utf-8",
                data: JSON.stringify(myObj),
                dataType: "json",
                cache: false,
                success: function (data) {                   
                    if (data.indexOf("success") > -1) {

                        document.location.search = "?status=success";
                        // location.reload();
                    } else if (data.indexOf("captchaerror") > -1) {
                        document.location.search = "?status=captchaerror";
                    } else if (data.indexOf("error") > -1) {
                        document.location.search = "?status=error";
                    }
                },
                error: function () {
                    alert("Error");
                }
            });
        }
    });


    // Load more comments
    var pagenumber = 1;
    var $remaingitems = $('#hftotalRecords').val() - $('#hfresultOnPageLoad').val();
    if ($remaingitems <= 0) {
        $('#Btn-More').hide();
    }
    $('#Btn-More').text($remaingitems + " More comments");
    $('#Btn-More').on("click", function (e) {
        var itemId = $('#hfGUID').val();
        var resultsPerClick = $('#hfResultsPerClick').val();
        var itemCount = $('.blog-container > ul > li:visible').length;
        pagenumber = pagenumber + 1;
        var myObj = {
            itemID: itemId,
            itemsperpage: resultsPerClick,
            totalcount: itemCount,
            pagenumber: pagenumber
        };
        e.preventDefault();
        $.ajax({
            type: "POST",
            url: '/api/sitecore/Comment/GetLoadMoreData',
            contentType: "application/json; charset=utf-8",
            data: JSON.stringify(myObj),
            dataType: "json",
            cache: false,
            success: function (data) {
                if (data.length > 0) {
                    $('.blog-container >.comment-Container').append(data);
                    $remaingitems = $remaingitems - itemCount;
                    $('#Btn-More').text($remaingitems + " More comments");
                    if ($remaingitems === 0 || $remaingitems <= 0) {
                        $('#Btn-More').hide();
                    }
                }
                else {
                    $('#Btn-More').hide();
                }
            },
            error: function () {
                alert("Error");
            }
        });
    });

});



