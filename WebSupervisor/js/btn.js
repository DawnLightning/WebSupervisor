// JavaScript Document
$(function(){
    var up = $('#upload').Huploadify({
        requeueErrors:true,
        queueSizeLimit:5,
        //queueID:filelist,
		auto:true,
		fileTypeExts:'*.xls;*.xlsx;',
		multi:true,
		//fileSizeLimit:99999999,
		breakPoints:false,
		saveInfoLocal:true,
		showUploadedPercent:true,//是否实时显示上传的百分比，如20%
		showUploadedSize:true,
		removeTimeout:20,
		uploader: '/Schedule/Upload',
		fileObjName: 'Filedata',
		//method:post,
		//onUploadStart:function(){
		//	//up.settings('formData', {aaaaa:'1111111',bb:'2222'});
		//	up.Huploadify('settings','formData', {aaaaa:'1111111',bb:'2222'});
		//},
		onUploadSuccess: function (file) {
		   

			
		},
		onUploadComplete: function (file,data) {
		  
		    //file.name 文件名
		    //data是后台返回的数据
		    $.ajax({
		        contentType: 'text/json; charset=utf-8',
		        url: '/Schedule/ScheduleInport',
		        dataType: 'json',
		        type:'post',
		        data:data
		    }).done(function (data) {
		        //一个文件上传完毕后
		    }).error(function (data) {
		      
		    });
		},
		/*getUploadedSize:function(file){
			var data = {
				data : {
					fileName : file.name,
					lastModifiedDate : file.lastModifiedDate.getTime()
				}
			};
			var url = 'http://49.4.132.173:8080/admin/uploadfile/index/';
			var uploadedSize = 0;
			$.ajax({
				url : url,
				data : data,
				async : false,
				type : 'POST',
				success : function(returnData){
					returnData = JSON.parse(returnData);
					uploadedSize = returnData.uploadedSize;
				}
			});
			return uploadedSize;
		}	*/	
	});


});
