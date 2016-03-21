﻿// JavaScript Document
/*
 * 改变特定标签内的html内容
 * url: 内容的地址
 * tag: 标签，如
 *      <div id="divid" class="divclass"></div>
 *      tag参数可以是 字符串 "#divid" 或 ".divclass"
*/
function replacetag(url, tag) {
    xmlhttpget(url, function (text) {
        curhref.save(url, tag);
        $(tag).html(text);
    });
}
//---------Supervisor--------------------
function selectallweek(thisbox, thisform) {
    var check = thisbox.checked;
    // 遍历 form  
    for (var i = 0; i < thisform.elements.length; i++) {
        // 提取控件  
        var checkbox = thisform.elements[i];
        // 检查是否是指定的控件  
        if (checkbox.name != thisbox.name && checkbox.type === "checkbox" && checkbox.checked != check) {
            checkbox.checked = check;
        }
    }
}

function selectday(thisday, dayname) {
    var allCheckBoxs = document.getElementsByName(dayname);
    var selectOrUnselect = true;
    if (thisday.checked) {
        selectOrUnselect = false;
    }

    if (selectOrUnselect) {

        for (var i = 0; i < allCheckBoxs.length; i++) {
            allCheckBoxs[i].checked = false;
        }
    } else {
        for (var i = 0; i < allCheckBoxs.length; i++) {
            allCheckBoxs[i].checked = true;
        }
    }
}


//-------------------Ready------------------------
$(document).ready(function () {
    //----------------checkbox----------------
    $(document).on("click", "input[type='checkbox']", function () {
        var checked = $(this).attr("checked");
        $(this).attr("checked", !checked);
    });

    //----------------Submit------------------
    $(document).on("click", "input#btncancel",
    function () {
        $(this.form).resetForm();
        $("#addtr").hide();
    });
    $(document).on("click", "input#btnsubmit",
    function () {
        $(this.form).ajaxSubmit({
            dataType: 'json',
            success: function (data) {
                if ("string" == typeof (data)) alert(data);
                else if (data.code == 0) {
                    alert("Success!" + '\n' + data.msg);
                    navreload();
                } else {
                    alert("Error!" + '\n' + data.msg);
                }
            },
            error: function (xhr) {
                alert(xhr.responseText);
            },
            resetForm: true
        });
    });


    //--------------download-----------------

    $(document).on("click", "a.fileDownloadPromise", function () {
        swal({ title: "Waiting", text: "The file is downloading.", type: "info", timer: 2000, showConfirmButton: false });

        $.fileDownload($(this).attr('href'), {
            successCallback: function (url) {
                swal({ title: "Success!", type: "success", timer: 2000, showConfirmButton: false });
                // alert('You just got a file download dialog or ribbon for this URL :' + url);
            },
            failCallback: function (html, url) {
                swal('Error!', 'File download failed!', 'error');
                /*alert('Your file download just failed for this URL:' + url + '\r\n' +
                        'Here was the resulting error HTML: \r\n' + html
                        );*/
            }
        });
        return false; //this is critical to stop the click event which will trigger a normal file download
    });
    //----------------tab_content pagination----------------
    $(document).on("click", ".pagination-container .pagination a", function () {

        if (typeof $(this).attr("href") != "undefined") {
            var href = $(this).attr("href");
            curhref.replacetag({
                url: href,
                tag: $(this).parents(".tab_content")
            });
        }
        return false;
    });

    //----------------tab_content input delete----------------
    $(document).on("click", "[name='btntddelete']", function () {

        var url = $(this).attr("formaction");
        var method = ((typeof $(this).attr("formmethod") != "undefined") ? $(this).attr("formmethod") : "POST");
        var tab_content = $(this).parents(".tab_content");
        var id = [];
        tab_content.find("form td input[type='checkbox'][checked='checked']").each(function (i, e) {
            id[i] = $(this).val().replace(/\s+/g, "");
        });

        if (typeof id == "undefined") {
            swal({ title: "提示", text: "请选择要要删除的信息！", timer: 1500, showConfirmButton: false });
        }
        else {
            swal({
                title: "您确定要删除吗？",
                text: "您确定要删除这条数据？",
                type: "warning",
                showCancelButton: true,
                closeOnConfirm: false,
                confirmButtonText: "确认",
                confirmButtonColor: "#ec6c62"
            }, function () {
                $.ajax({
                    contentType: 'text/json; charset=utf-8',
                    url: url,
                    dataType: 'json',
                    type: method,
                    data: JSON.stringify(id)
                }).done(function (data) {

                    //if (data.code == 0) {
                    //  
                    //} else {
                    //    swal("OMG", "删除操作失败了!", "error");
                    //}
                    swal("操作成功!", "已成功删除数据！", "success");
                       curhref.reload();


                }).error(function (data) {
                    swal("OMG", "删除操作失败了!", "error");
                });
            });
        }
        return false;
    });




    //-----------------tab-------------------
    $(document).on("click", "ul.tabs li",
    function () {
        $("ul.tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".tab_content").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });

    $(document).on("click", "ul.set-tabs li",
    function () {
        $("ul.set-tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".set-tab_content").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });

    $(document).on("click", "ul#navtab li",
    function () {
        $("ul#navtab li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".tab_contents").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });



    //----------------Menu----------------
    $(document).on("click", ".mm-menu__link", function () {

        var url = $(this).attr("href");
        var title = $(this).find(".mm-menu__link-text").text();
        if (typeof url != "undefined") {
            curhref.loadpage({
                url: url,
                func: function () {
                    $("#title2").html(title);
                }
            });
        }
        return false;
    });
    var menu = new Menu;


    //-----------execute once----------

    if (!curhref._loadpage()) {
        $(".mm-menu__link:first").click();
    }


});


//------------------wrote by myloverhxx <myloverhxx@163.com>-----------------------
/*临时表删除*/
function deletesure() {
    swal({
        title: "确认删除?",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "确认",
        closeOnConfirm: false
    },
      function () {
          swal("",
            "已删除",
            "success");
      });
}
/*临时表按钮 修改*/
function edite1() {
    document.getElementById("editable1").contentEditable = "true";
};

/*确认表按钮 修改*/
function edite2() {
    document.getElementById("editable2").contentEditable = "true";
};
/*教师信息按钮 修改*/
function edite3() {
    document.getElementById("editable3").contentEditable = "true";
};
/*权限管理按钮 修改*/
function edite4() {
    document.getElementById("editable4").contentEditable = "true";
};
//教师信息按钮 保存
function tsave() {
    swal({
        title: "已保存",//放js显示乱码 所以放这里
        timer: 1500,
        showConfirmButton: false,
        type: "success"
    });
    document.getElementById("editable3").contentEditable = "false";
};