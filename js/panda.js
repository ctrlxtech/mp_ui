var pandaAjax = {
    fail: function(jqXHR, textStatus, errorThrown){
        //	alert("Sorry, Error Occur !");
    },
    get: function(url, param, callback, controller, returnDataType){
        if (!returnDataType) 
            returnDataType = 'text';
        //callback is only use for easy request, for example load html
        var jqxhr = $.get(url, param, callback, returnDataType);
        jqxhr.done(function(data){
            if (typeof(controller) === 'object' && typeof(controller.afterResponse) === 'function') {
                controller.afterResponse(data);
            }
        });
        jqxhr.fail(function(jqXHR, textStatus){
            if (typeof(controller) === 'object' && typeof(controller.afterFail) === 'function') {
                controller.afterFail(jqXHR, textStatus);
            }
            else 
                pandaAjax.fail(jqXHR, textStatus);
        });
        
    },
    post: function ultramainPost(url, param, controller, returnDataType, sendDataType){
        if (!returnDataType) 
            returnDataType = 'json';
        if (!sendDataType) 
            sendDataType = 'application/json; charset=UTF-8'
        $.ajax({
            type: 'POST',
            url: url,
            data: param,
            contentType: sendDataType,
            dataType: returnDataType,
            beforeSend: function(){
                if (typeof(controller) === 'object' && typeof(controller.beforeSend) === 'function') {
                    var success = controller.beforeSend();
                    
                    if (success === false) {
                        return false;
                    }
                }
            }
        }).done(function(data){
            if (typeof(controller) === 'object' && typeof(controller.afterResponse) === 'function') {
                controller.afterResponse.apply(this, data);
            }
        }).fail(function(jqXHR, textStatus){
            if (typeof(controller) === 'object' && typeof(controller.afterFail) === 'function') {
                controller.afterFail.apply(this, data);
            }
            else 
                pandaAjax.fail(jqXHR, textStatus);
        });
        ;
    }
};


var massageDetailsPage = (function(){
    var publicObj = {
        beforeShow: function(parent){
			var $parent=$(parent);
            var massageName = $parent.find('.mp-massageTypePreview-name').text();
            var massagePrice = $parent.find('.mp-massageTypePreview-price').text();
            var massageType = $parent.find('input[name="panda-massageType"]').val();
			var massageImage = $parent.find('.mp-massageTypePreview-image').attr('src');
            var $massageDetailsPanel = $(this).find('.mp-massageDetails-panel');
            $massageDetailsPanel.find('p.mp-massageDetails-description').hide();
            $massageDetailsPanel.find('p[data-massageType="' + massageType + '"]').show();
            $(this).find('.mp-massageDetails-image').attr('src',massageImage);
			$(this).find('.mp-massageDetails-title').text(massageName + ' ' + massagePrice);
            $(this).find('li.mp-currentDetailsPage').text(massageName + ' ' + massagePrice);
            $(this).find('#datepicker').val('');
            $(this).find('#spinner').val('');
            $(this).find('input[name="timeInput"]').val('');
            $(this).find('input[name="genderPreferred"]').val('');
			window.scrollTo(0, 0);
        },
        afterResponse: function(){
            $("#datepicker").datepicker();
            $("#spinner").spinner({
                spin: function(event, ui){
                    if (ui.value > 100) {
                        $(this).spinner("value", 0);
                        return false;
                    }
                    else 
                        if (ui.value < 0) {
                            $(this).spinner("value", 0);
                            return false;
                        }
                }
            });
        }
        
    };
    return publicObj;
    
}());




var defualtSetting = {
    appBaseURL: './html/',
    initActivePage: 'panda_home',
    loadedPageArray: []
};

var pandaPageObj = {};

var headerAttr = {
    mpController: 'data-mpController',
    mpPageId: 'data-mpPageId',
    mpAjax: 'data-mpAjax', //true(default) or false
    mpPageType: 'data-mpPageType', //default(default) or other 
};

var pageController = {
    init: function($pageHeaders){
    
        var $header;
        for (var i = 0; i < $pageHeaders.length; i++) {
            $header = $($pageHeaders[i]);
            var pageId = $header.attr(headerAttr.mpPageId);
            //check page has been loaded or not
            if (!pageId) 
                continue;
            if (pandaPageObj[pageId].mpLoaded === 'true') 
                continue;
            if (pandaPageObj[pageId].mpShow === 'true') 
                continue;
            
            // only	process header has mpPageId.
            if (pageId) {
                if (defualtSetting.loadedPageArray.indexOf(pageId) == -1) {
                    //page has not been load for sure
                    pageController.load($header);
                }
                
            }
            
        }
    },
    load: function($header, callBack, forceLoad){
        var mpControllerName = $header.attr(headerAttr.mpController);
        var mpNewPageId = $header.attr(headerAttr.mpPageId);
        if (!mpNewPageId) 
            return;
        //page already been loaded
        if ($('#mp-mainContent').find('#' + mpNewPageId).length > 0) 
            return;
        var mpController;
        if (window[mpControllerName]) 
            mpController = window[mpControllerName];
        //only load pageType is default
        if ($header.attr(headerAttr.mpPageType) === 'default' || forceLoad === 'true') {
            var getURL = defualtSetting.appBaseURL + mpNewPageId + '.html';
            pandaAjax.get(getURL, undefined, function(data){
                console.log(mpNewPageId + ' page load success ! ');
                pandaPageObj[mpNewPageId].mpLoaded = 'true';
                defualtSetting.loadedPageArray.push(mpNewPageId);
                $('#mp-mainContent').append(data);
                //this callBack only use for framework. 
                if (typeof(callBack) === 'function') 
                    callBack.apply($header);
            }, mpController);
        }
        
    },
    setup: function(pageId){
        var $pageHeaders;
        if (pageId) 
            $pageHeaders = $('#' + pageId).find('.mp-pageHeader');
        else 
            $pageHeaders = $('.mp-pageHeader');
        //setup default attr
        var $header;
        var headerValidArr = [];
        var realHeaderArr = [];
        for (var i = 0; i < $pageHeaders.length; i++) {
            $header = $($pageHeaders[i]);
            
            var tempPageId = $header.attr(headerAttr.mpPageId);
            
            if (!tempPageId) 
                continue;
            
            if (headerValidArr.indexOf(tempPageId) == -1) {
                headerValidArr.push(tempPageId);
                realHeaderArr.push($header);
            }
            
            if (typeof($header.attr(headerAttr.mpPageType)) === 'undefined') 
                $header.attr(headerAttr.mpPageType, 'default');
            
            
            if (typeof(pandaPageObj[tempPageId]) === 'undefined') {
                pandaPageObj[tempPageId] = {
                    mpLoaded: 'false',
                    mpShow: 'false'
                }
            }
        };
        //setup click event
        $pageHeaders.off('click');
        $pageHeaders.on('click', function(e){
            var $this = $(this);
            var clickPageId = $this.attr(headerAttr.mpPageId);
            var isAjax = $this.attr(headerAttr.mpAjax);
            if (isAjax !== 'false') {
            
                if (pandaPageObj[clickPageId].mpLoaded === 'true') {
                    //TODO add animation
                    pageController.changePage($this);
                }
                else {
                    var callBack = function(){
                        pageController.changePage($(this));
                    }
                    pageController.load($this, callBack, 'true');
                }
                e.preventDefault();
            }
        });
        //call init
        pageController.init(realHeaderArr);
    },
    changePage: function($this){
        var newPageId = $this.attr(headerAttr.mpPageId);
        var activePageId = localStorage.getItem("mp-activePage");
        if (activePageId !== newPageId) {
            $('#' + activePageId).hide(function(){
            
            });
            var newPageControllerName = $this.attr(headerAttr.mpController);
            if (window[newPageControllerName]) {
                var newPageController = window[newPageControllerName];
                if (typeof(newPageController) === 'object' && typeof(newPageController.beforeShow) === 'function') {
                    newPageController.beforeShow.apply($('#' + newPageId), $this);
                }
            }
            $('#' + newPageId).show(0, function(){
                //setup child page
                if (pandaPageObj[newPageId].mpShow === 'false') {
                    pandaPageObj[newPageId].mpShow = 'true';
                    pageController.setup($this.attr(headerAttr.mpPageId));
                }
				
                
            });
            localStorage.setItem('mp-activePage', newPageId);
        }
    }
}

$(function(){
    localStorage.setItem('mp-activePage', defualtSetting.initActivePage);
    defualtSetting.loadedPageArray.push(defualtSetting.initActivePage);
    pandaPageObj[defualtSetting.initActivePage] = {
        mpLoaded: 'true',
        mpShow: 'true'
    };
    pageController.setup();
    //test

});
