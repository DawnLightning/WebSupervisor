// JavaScript Document
var navinfo = {};
function navpage(pagename, t1, t2) {
    $("#title1").html(t1);
    $("#title2").html(t2);
    window.location.hash =pagename;
    showpage(pagename);
}
function navreload() {
    showpage(this.navinfo.url);
}
function showpage(url) {
    this.navinfo.url = url;
    var xmlhttp = GetXmlHttpObject();

    if (xmlhttp == null) {
        alert("Browser does not support HTTP Request");
        return;
    }

    /* var url=document.URL;
    url= url.substring(0,url.split('#')[0].lastIndexOf('/')+1);
    url += "includes/"+pagename+".html"; */
    // var url = "/includes/"+pagename+".html";
    xmlhttp.onreadystatechange = function() {
        if (xmlhttp.readyState == 4 || xmlhttp.readyState == "complete") {
            $("#wrapper").html(xmlhttp.responseText);
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
        } catch(e) {
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
$(document).ready(function() {

    //----------------Submit------------------
    $(document).on("click", "input#btncancel",
    function() {
        $(this.form).resetForm();
        $("#addtr").hide();
    });
    $(document).on("click", "input#btnsubmit",
    function() {
        $(this.form).ajaxSubmit({
            dataType: 'json',
            success: function(data) {
                if ("string" == typeof(data)) alert(data);
                else if (data.code == 0) {
                    alert("Success!" + '\n' + data.msg);
                    navreload();
                } else {
                    alert("Error!" + '\n' + data.msg);
                }
            },
            error: function(xhr) {
                alert(xhr.responseText);
            },
            resetForm: true
        });
    });

    //-------------------Menu-------------------------
    $(".mm-menu__link").on("click",
    function() {
        //Default Action
        $(".tab_content").hide(); //Hide all content
        $("ul.tabs li:first").addClass("active").show(); //Activate first tab
        $(".tab_content:first").show(); //Show first tab  content
        //Default Action
        $(".set-tab_content").hide(); //Hide all content
        $("ul.set-tabs li:first").addClass("active").show(); //Activate first tab
        $(".set-tab_content:first").show(); //Show first tab  content
    });

    //On Click Event
    $(document).on("click", "ul.tabs li",
    function() {
        $("ul.tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".tab_content").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });

    //On Click Event
    $(document).on("click", "ul.set-tabs li",
    function() {
        $("ul.set-tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".set-tab_content").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });

});
