﻿var suggest = {
  suggests: {},
  timeout: null,

  getSuggests: function (evt, input) {
    if (this.timeout) {
      clearTimeout(this.timeout);
    }
    var inputValue = input.value;
    if (inputValue.length > 1) {
      var url = "/api/getSuggest?query=" + inputValue;
      this.timeout = setTimeout(function () {
        $.ajax({
          dataType: "json",
          url: url,
          success: function (data) {
            suggest.suggests = data;
            suggest.fillSuggestBox();
          },
          error: function () { alert("xmlhttpproblem: " + xhr.status); }
        });
      }, 600);
    }
  },

  fillSuggestBox: function () {
    var suggestBox = document.getElementById('suggests');
    var content = "";
    for (var x = 0; x < suggest.suggests.length; x++) {
      var result = suggest.suggests[x];
    
      if (result)
        content += '<a href="' + result.Url + '">' + result.Title + '</a>';
    }
    suggestBox.innerHTML = content;
    suggestBox.className = 'suggest-shown';

  },

  cancelEvent: function (evt) {
    if (evt.keyCode === 13) {
      console.log(evt.target.value);
      if (isIE || isIE7) {
        evt.cancelBubble = true;
        evt.returnValue = 0;
      }
      else {
        evt.stopPropagation();
        evt.preventDefault();
      }
    }
  },

  hide: function (evt) {
    var suggestBox = document.getElementById('suggests');
    evt = evt || window.event;
    var el = evt.target || evt.srcElement;
    if (suggestBox && el.id !== 'suggest')
      suggestBox.className = 'suggest-hidden';
  }
};

document.onclick = suggest.hide;