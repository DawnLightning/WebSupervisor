// JavaScript Document

function clearform(thisform) {
    // 遍历 form  
    for (var i = 0; i < thisform.elements.length; i++) {
        // 提取控件  
        var input = thisform.elements[i];
        // 检查是否是指定的控件  
        if (input.type === "checkbox") {
            input.checked = false;
        } else if (input.type === "text") {
            input.value = "";
        }
    }
}
function navpage(pagename, t1, t2) {
    $("#title1").html(t1);
    $("#title2").html(t2);
    window.location.hash = "#!" + pagename;
    showpage(pagename);
}
function showpage(url) {
    var xmlhttp = GetXmlHttpObject();

    if (xmlhttp == null) {
        alert("Browser does not support HTTP Request");
        return;
    }

    /* var url=document.URL;
    url= url.substring(0,url.split('#')[0].lastIndexOf('/')+1);
    url += "includes/"+pagename+".html"; */
    // var url = "/includes/"+pagename+".html";
    xmlhttp.onreadystatechange = function () {
        if (xmlhttp.readyState == 4 || xmlhttp.readyState == "complete") {
            $("#wrapper").html(xmlhttp.responseText);
        }
    };
    xmlhttp.open("GET", url, true);
    xmlhttp.send(null);
}


function GetXmlHttpObject() {
    var xmlhttp;
    if (window.XMLHttpRequest) {// code for IE7+, Firefox, Chrome, Opera, Safari
        xmlhttp = new XMLHttpRequest();
    }
    else {// code for IE6, IE5
        try {
            xmlhttp = new ActiveXObject("Msxml2.XMLHTTP");
        }
        catch (e) {
            xmlhttp = new ActiveXObject("Microsoft.XMLHTTP");
        }
    }

    return xmlhttp;
}

//---------Supervisor--------------------

function selectallweek(thisbox,thisform) {
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

