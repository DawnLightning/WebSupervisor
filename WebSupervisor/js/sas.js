// JavaScript Document
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
        swal({ title: "Waiting", text: "The file is downloading.",type:"info", timer: 2000, showConfirmButton: false });
        $.fileDownload($(this).prop('href'))
            .done(function () {
                alert('success');
                swal({ title: "Success!", type:"success", timer: 2000, showConfirmButton: false });
            })
            .fail(function () { swal('Error!','File download failed!','error'); });

        return false; //this is critical to stop the click event which will trigger a normal file download
    });
    $(document).on("click", "a.fileDownloadSimpleRichExperience", function () {
        $.fileDownload($(this).prop('href'), {
            preparingMessageHtml: "We are preparing your report, please wait...",
            failMessageHtml: "There was a problem generating your report, please try again."
        });
        return false; //this is critical to stop the click event which will trigger a normal file download!
    });

    //----------------pagination----------------
    $(document).on("click", ".pagination-container .pagination a", function () {

        if (typeof $(this).attr("href") != "undefined") {
            replacetag($(this).attr("href"), $(this).parents(".tab_content"));
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

    //-------------------Menu-------------------------
    /* $(".mm-menu__link").on("click",
     function () {
         //Default Action
         $(".tab_content").hide(); //Hide all content
         $("ul.tabs li:first").addClass("active").show(); //Activate first tab
         $(".tab_content:first").show(); //Show first tab  content
         //Default Action
         $(".set-tab_content").hide(); //Hide all content
         $("ul.set-tabs li:first").addClass("active").show(); //Activate first tab
         $(".set-tab_content:first").show(); //Show first tab  content
     });*/

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

    var menu = new Menu;

    if ("onhashchange" in window) {
        window.onhashchange = _loadpage;
    }

    if (!_loadpage()) {
        $(".mm-menu__link:first").click();
    }


});
