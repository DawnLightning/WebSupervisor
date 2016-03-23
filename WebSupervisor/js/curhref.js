/*! Wrote by Jier <naturalwill999@gmail.com> */


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

var curhref = (function (window) {
    var _this = {};

    _this.defaultdata = {
        url: "/Home/Confirm",
        tag: "#wrapper"
    };

    _this.record = {
        data: _this.defaultdata,
        func: _this.replacetag
    };

    _this.save = function (record) {
        _this.record = record;
    };

    /*
    data ：{
        url: 内容的地址
        tag: 标签，可以是 字符串 "#divid" 或 ".divclass"
    }
    */
    _this.replacetag = function (data) {
        xmlhttpget(data.url, function (text) {
            _this.save({ data: data, func: _this.replacetag });
            $(data.tag).html(text);
        });
    };


    _this.reload = function () {

        if (typeof _this.record.func === 'function') {
            if (typeof _this.record.data != "undefined")
                _this.record.func(_this.record.data);
            else {
                _this.record.func();
            }
        }

    };



    var _loadpage_func;
    /*
    data ：{
        url: 内容的地址
        func: 
    }
    */
    _this.loadpage = function (data) {
        var url = data.url ? data.url : "";
        var hash = "#!" + url;
        if (typeof data.func === 'function') {
            _loadpage_func = data.func;
        }

        if (hash != window.location.hash) {
            window.location.hash = hash;
            if ("onhashchange" in window)
                return;
        }
        _this._loadpage();
    }

    _this._loadpage = function () {

        var act = window.location.hash;
        if (act.substring(0, 3) === "#!/") {
            var url = act.substring(2);
            xmlhttpget(url, function (text) {
                _this.save({ data: { url: url, func: _loadpage_func }, func: _this.loadpage });
                $(_this.defaultdata.tag).html(text);
                if (typeof _loadpage_func === 'function') {
                    _loadpage_func();
                    _loadpage_func = null;
                }
            });
            return true;
        }
        return false;
    }



    if ("onhashchange" in window) {
        window.onhashchange = _this._loadpage;
    }

    return _this;
}(window));