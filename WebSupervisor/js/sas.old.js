
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



/*
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
}());*/

/*
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
*/






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