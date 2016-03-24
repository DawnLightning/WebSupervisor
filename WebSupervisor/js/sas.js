

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