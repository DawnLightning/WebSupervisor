﻿// JavaScript Document
//--------------Common------------------


var navinfo = {};
function navpage(pagename, t1, t2) {
    $("#title1").html(t1);
    $("#title2").html(t2);
    loadpage(pagename);
}
function navreload() {
    loadpage(this.navinfo.url);
}
function showpage(url) {
    loadpage(url);
}


function loadpage(url, func) {
    window.location.hash = "#!" + url;
    if (typeof func === 'function') {
        _loadpage_func = func;
    }

    if ("onhashchange" in window) {
        return;
    }
    _loadpage();
}
var _loadpage_func;
function _loadpage() {
    var act = window.location.hash;
    if (act.substring(0, 3) === "#!/") {
        xmlhttpget(act.substring(2), function (text) {
            $("#wrapper").html(text);
            if (typeof _loadpage_func === 'function') {
                _loadpage_func();
                _loadpage_func = null;
            }
        });
        return true;
    }
    return false;
}


/*
 * 改变特定标签内的html内容
 * url: 内容的地址
 * tag: 标签，如
 *      <div id="divid" class="divclass"></div>
 *      tag参数可以是 字符串 "#divid" 或 ".divclass"
*/
function replacetag(url, tag) {
    xmlhttpget(url, function (text) {
        $(tag).html(text);
    });
}
function test() {
    $.ajax({
        url: url,
        data: data,
        success: function (response, status, xhr) {
            if (xhr.readyState == 4 || xhr.readyState == "complete") {
                func(response);
            }
        },
        dataType: dataType
    });
}
function xmlhttpget(url, func) {
    var xmlhttp = GetXmlHttpObject();

    if (xmlhttp == null) {
        alert("Browser does not support HTTP Request");
        return;
    }
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 || xmlhttp.readyState == "complete") {
            if (typeof func === 'function') {
                func(xmlhttp.responseText);
            }
        }
    };
    xmlhttp.open("GET", url, true);
    xmlhttp.send(null);
}


function GetXmlHttpObject() {
    var xmlhttp;
    if (window.XMLHttpRequest) { // code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    } else { // code for IE6, IE5
        try {
            xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
        } catch (e) {
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
    }

    return xmlhttp;
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


var sasdata = (function () {
    var _this = this;
    _this._data = {};
    _this._fetchdata_href;

    //convert url to tableid
    _this.url2tableid = function (href) {
        var reg = /(?:.*)\/([^/]+)\/([^?]+)(?:.*)/i;
        var dataid = href.replace(reg, "$1_$2");
        return dataid;
    };
    //convert tableid to url
    _this.tableid2url = function (tableid) {
        var reg = /([A-Za-z0-9]+)_([A-Za-z0-9]+)/i;
        var url = tableid.replace(reg, "/$1/$2?ajax=true");
        return url;
    };

    //fetch data into _this._data
    _this.fetchdata = function (href, func) {
        _this._fetchdata_href = href;
        var dataid = _this.url2tableid(href);
        $.ajax({
            url: href,
            success: function (response, status, xhr) {
                if (xhr.readyState == 4 || xhr.readyState == "complete") {
                    _this._data[dataid] = response;

                    if (typeof func === 'function') {
                        func(response);
                    }
                }
            },
            dataType: "json"
        });
    };

    //fetch data into _this._data
    _this.refetchdata = function (func) {
        _this.fetchdata(_this._fetchdata_href, func);
    };

    // get data by tableid and call func(data)
    _this.getdatabyid = function (tableid, id, func) {
        if (typeof func === 'function') {
            var data = _this._data[tableid];
            if (typeof data == "undefined") {
                _this.fetchdata(_this.tableid2url(tableid),
                    function (data) {
                        func(_this._getdata(data, id));
                    });
            }
            else
                func(_this._getdata(data, id));
        }

    };

    _this._getdata = function (data, id) {

        if (typeof id != "undefined") {
            for (d in data) {
                if (data[d][id[0]] === id[1]) {
                    return data[d];
                }
            }
            return false;
        }
        return data;
    };

    return _this;
}());


//-------------------Ready------------------------
$(document).ready(function () {

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
    //----------------pagination----------------
    $(document).on("click", ".pagination-container .pagination a", function () {

        if (typeof $(this).attr("href") != "undefined") {
            var href = $(this).attr("href");
            sasdata.fetchdata(href + "&ajax=true");
            replacetag(href, $(this).parents(".tab_content"));

        }
        return false;
    });

    //----------------Menu----------------
    $(document).on("click", ".mm-menu__link", function () {

        var url = $(this).attr("href");
        var title = $(this).find(".mm-menu__link-text").text();
        if (typeof url != "undefined") {
            loadpage(url, function () {
                $("#title2").html(title);

            });
        }
        return false;
    });


    //On Click Event
    $(document).on("click", "ul.tabs li",
    function () {
        $("ul.tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".tab_content").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });

    //On Click Event
    $(document).on("click", "ul.set-tabs li",
    function () {
        $("ul.set-tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".set-tab_content").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });
    //----------table------------------------
    $(document).on("click", "table tr td",
    function () {
        var _this = this;
        var id = $(_this).parent().find("[name='id']").val();
        var idname = $(_this).parent().find("[name='idname']").val();
        if (typeof id != "undefined") {
            var tableid = $(_this).parents("table").attr('id');
            var template = $('#' + tableid + '_td_edit_template').html();

            sasdata.getdatabyid(tableid, [idname, id], function (data) {
                var rendered = Mustache.render(template, data);
                $(_this).parent().html(rendered);
            });
        }
        
    });
    $(document).on("click", "table tr td #tdsubmit",
    function () {
        var _this = this;
        var json = {};
        $(_this).parents("tr").find("input").each(function () {
            json[$(this).attr("name")] = $(this).val();
        });
var table = $(_this).parents("table");
        $.ajax({
            url: table.parents("form").attr("action"),
            data: json,
            type: ((typeof table.parents("form").attr("method") != "undefined") ? table.parents("form").attr("method") : "GET"),
            success: function (response, status, xhr) {
                if (xhr.readyState == 4 || xhr.readyState == "complete") {
                    sasdata.refetchdata(function (data) {

                        
                        var tid = table.attr('id');
                        var template = $('#' + tid + '_header_template').html()
                            + $('#' + tid + '_template').html();
                        var rendered = Mustache.render(template, data);
                        table.html(rendered);
                        var len = table.find("tr").length;
                        for (;len<12;len++){
                            table.append($('#' + tid + '_empty_template').html());
                        }

                    });
                }
            },
            dataType: "json"
        });

        return false;
    });


    var menu = new Menu;

    if ("onhashchange" in window) {
        window.onhashchange = _loadpage;
    }

    if (!_loadpage()) {
        $(".mm-menu__link:first").click();
    }


});
