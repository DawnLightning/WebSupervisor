// JavaScript Document
$(document).ready(function() {
  $(".mm-menu__link").on("click",function(){
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
  $(document).on("click","ul.tabs li",function(){
        $("ul.tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".tab_content").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
  });



    //On Click Event
  $(document).on("click","ul.set-tabs li",
    function() {
        $("ul.set-tabs li").removeClass("active"); //Remove any "active" class
        $(this).addClass("active"); //Add "active" class to selected tab
        $(".set-tab_content").hide(); //Hide all tab content
        var activeTab = $(this).find("a").attr("href"); //Find the rel attribute value to identify the active tab + content
        $(activeTab).fadeIn(); //Fade in the active content
        return false;
    });



/* =============================================================================
 table add & delete
 ============================================================================= */

  $(document).on("click","btn1",function(){
    var oTab = document.getElementById('tb1');

    var oNo = document.getElementById('teacherNo');
    var oName = document.getElementById('teacherName');
    var oTitle = document.getElementById('teacherTitle');
    var oRoom = document.getElementById('teacherRoom');
    var oTel = document.getElementById('teacherTel');
    var oEmail = document.getElementById('teacherEmail');

    var oTr = document.createElement('tr'); //创建一个tr
        //创建第一个td
        var oTd = document.createElement('td');
        oTd.innerHTML = oNo.value;
        oTr.appendChild(oTd); //将td插入tr
        //创建第二个td
        var oTd = document.createElement('td');
        oTd.innerHTML = oName.value;
        oTr.appendChild(oTd); //将td插入tr
        //创建第三个td
        var oTd = document.createElement('td');
        oTd.innerHTML = oTitle.value;
        oTr.appendChild(oTd); //将td插入tr
        //创建第四个td
        var oTd = document.createElement('td');
        oTd.innerHTML = oRoom.value;
        oTr.appendChild(oTd); //将td插入tr
        //创建第五个td
        var oTd = document.createElement('td');
        oTd.innerHTML = oTel.value;
        oTr.appendChild(oTd); //将td插入tr
        //创建第二个td
        var oTd = document.createElement('td');
        oTd.innerHTML = oEmail.value;
        oTr.appendChild(oTd); //将td插入tr
        oTab.tBodies[0].appendChild(oTr); //将整个tr插入到表格中
  });

});

