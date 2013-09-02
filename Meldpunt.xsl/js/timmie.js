var isOpera = (typeof(window.opera) != "undefined");
var isIE = (navigator.userAgent.indexOf("MSIE") != -1) && !window.opera;
var isIE7 = (navigator.userAgent.indexOf("MSIE 7") != -1) && !window.opera && (typeof(XMLHttpRequest) != "undefined");
var isGecko = ((navigator.userAgent.indexOf("Gecko") != -1) && (navigator.appVersion.indexOf("AppleWebKit") == -1));
var isSafari = (navigator.appVersion.indexOf("AppleWebKit") != -1);

document.documentElement.className += " " + (isIE7?"ie7 ie":(isIE?"ie":(isGecko?"gecko":(isOpera?"opera":(isSafari?"safari":"unknown")))));

window.addEvent('domready', function() {
    
    $$('.defaultList li').each(function(item, index){
        var description = item.getElement(".description");
        description.o_height = description.getHeight();
        description.setStyle('height', '0px');
        var trigger = item.getElement("h3");
        
        item.addEvent('click',function(){            
            description.set('morph', {duration: 500, transition: 'bounce:out' });
            trigger.set('morph', {duration: 200, transition: 'sine:out' });
            
            //hide
            if(description.className.contains('shown')){
                trigger.morph({
                    'color':'#000000',
                    'background-color' : '#EFEFEF'
                });
                description.morph({
                    'height': '0px',
                    'padding-top': '0px',
                    'padding-bottom': '0px',
                    'border-color': '#EFEFEF'
                    });
                
                description.removeClass('shown');
            }
            //show
            else{
                trigger.morph({
                    'color':'#ffffff',
                    'background-color' : '#83003C'
                });
                description.morph({
                    'height': description.o_height,
                    'padding-top': '10px',
                    'padding-bottom': '10px',
                    'border-color': '#83003C'
                    });
                description.addClass('shown');
                
            }
        })
    });
})