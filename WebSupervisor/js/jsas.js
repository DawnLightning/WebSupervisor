// JavaScript Document

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
    $(document).on("click", ".mm-menu__link", function (eventObject) {

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



    //---------Supervisor-----------
    function check_box(box, force) {
        var checked = $(box).hasClass("checked_box");
        if (typeof force === "boolean") {
            checked = force ? false : true;
        }
        if (checked) {
            $(box).removeClass("checked_box");
            $(box).addClass("checked_box_blank");
        }
        else {
            $(box).removeClass("checked_box_blank");
            $(box).addClass("checked_box");
        }
    }
    var free_time_config = {
        table_selector: "div[name='freetime']",
        week_load_selector: "div#freetime_week_load"
    }
    window.free_time = (function () {
        var _this = {
            data: {
                tid: null,
                week: 1,
                week_to_save: [],
                free_time_data: {}
            },
            i2dt: function (index) {
                var d = index % 7 + 1, t = parseInt(index / 7) + 1;
                return { day: d, time: t }
            },
            dt2i: function (data) {
                return (data.time - 1) * 7 + data.day - 1;
            },
        };
        _this.get_cur_tid = function () {
            return _this.data.tid;
        }
        _this.show_free_time_by_week = function (week) {
            _this.data.week = week ? week : _this.data.week;
            check_box(free_time_config.table_selector, false);
            if (typeof _this.data.free_time_data === "undefined") {
                return;
            }
            var weekdata = _this.data.free_time_data[_this.data.week];
            if (typeof weekdata === "undefined") {
                return;
            }
            for (d in weekdata) {
                for (t in weekdata[d]) {
                    var i = _this.dt2i({ day: d, time: weekdata[d][t] });
                    check_box($(free_time_config.table_selector).eq(i), true);
                }
            }
        };
        _this.get_week_to_save = function () {
            var weeks = [];
            $("#freetime_week_save .divbutton_active").each(function () {
                weeks.push($(this).text());
            });
            _this.data.week_to_save = weeks;
            return weeks;
        };
        _this.get_free_time_by_table = function () {
            var freetime = {};
            $(free_time_config.table_selector).each(function (i, e) {
                var d = i % 7 + 1, t = parseInt(i / 7) + 1;
                /*  
                //二维
                  if (typeof freetime[d] == "undefined") {
                      freetime[d] = {};
                  }
                  freetime[d][t] = $(e).hasClass("checked_box");
                  */
                //一维
                if ($(e).hasClass("checked_box")) {
                    if (typeof freetime[d] == "undefined") {
                        freetime[d] = [];
                    }
                    freetime[d].push(t);
                }
            });
            return freetime;
        };
        _this.show_free_time_by_tid = function (tid) {
            _this.data.tid = tid;
            $.ajax({
                url: '/Supervisor/ShowSpareTime',
                type: 'get',
                dataType: "json",
                data: { tid: _this.data.tid },
                success: function (data) {
                    _this.data.free_time_data = data.free_time;
                    //_this.show_free_time_by_week();
                    $(free_time_config.week_load_selector + " .divbutton").eq(_this.data.week - 1).click();

                },
                error: function () {
                    alert("出错了");
                }
            });
        };
        return _this;
    }());

    $(document).on("click", "#submit_freetime", function () {

        $.ajax({
            url: '/Supervisor/SaveSpareTime',
            type: 'post',
            dataType: "json",
            data: {
                tid: free_time.get_cur_tid,
                freetime: JSON.stringify(free_time.get_free_time_by_table()),
                week: JSON.stringify(free_time.get_week_to_save())
            },
            async: false,
            success: function () {
                //这里
            },
            error: function () {
                alert("出错了");
            }
        })
    });

    $(document).on("click", ".table-freetime", function (event) {
        var tag = $(event.target);
        var checked;
        if (tag.attr("name") == 'freetime') {
            check_box(tag);
        }
        else if (tag.attr("name") == 'freetime_check_row') {
            if (tag.data("checked")) {
                tag.removeData("checked");
                checked = false;
            } else {
                tag.data("checked", true);
                checked = true;
            }
            tag.parents("tr").find("div[name='freetime']").each(function () {
                check_box(this, checked);
            });
        }
        else if (tag.attr("name") == 'freetime_check_week') {
            if (tag.data("checked")) {
                tag.removeData("checked");
                checked = false;
            } else {
                tag.data("checked", true);
                checked = true;
            }
            tag.parents("table").find("div[name='freetime']").each(function () {
                check_box(this, checked);
            });
        }
        else if (tag.attr("name") == 'freetime_check_day') {
            var i = $("div[name='freetime_check_day']").index(tag);

            if (tag.data("checked")) {
                tag.removeData("checked");
                checked = false;
            } else {

                tag.data("checked", true);
                checked = true;
            }
            tag.parents("table").find("tr").each(function () {

                check_box($(this).find("div[name='freetime']").eq(i), checked);
            });
        }
    });

    $(document).on("click", "div#freetime_week_save",
    function (event) {
        if ($(event.target).hasClass("divbutton_active")) {
            $(event.target).removeClass("divbutton_active");
        } else {
            $(event.target).addClass("divbutton_active");
        }
    });

    $(document).on("click", free_time_config.week_load_selector,
    function (event) {
        if ($(event.target).hasClass("divbutton")) {
            $(event.target).siblings().removeClass("divbutton_active");
            $(event.target).addClass("divbutton_active");
            free_time.show_free_time_by_week($(event.target).text());
        }
    });
    //---------Share/_ArrageAdd-----------
    $(document).on("click", "#tablesupervisor1 tr", function () {
        $(this).siblings().find("td:first").removeClass("checked_box");
        $(this).children("td:first").addClass("checked_box");
        $("#tablesupervisor").find("#shead").html($(this).find("td:eq(1)").clone());

    });
    $(document).on("click", "#tablesupervisor2 tr", function () {
        if ($(this).children("td:first").hasClass("checked_box")) {
            $(this).children("td:first").removeClass("checked_box");
            $("#tablesupervisor tbody").find("tr[name='" + $(this).attr("name") + "']").remove();
        }
        else {
            $(this).children("td:first").addClass("checked_box");
            $("#tablesupervisor tbody").append($(this).find("td:eq(1)").clone());
            $("#tablesupervisor tbody > td").wrap("<tr name=\"" + $(this).attr("name") + "\"></tr>");

        }
    });
});
