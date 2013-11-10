var suggest = {
  suggestPlaatsen: [],
  timeout: null,

  getSuggests: function (evt, input) {
    if (this.timeout) {
      clearTimeout(this.timeout)
    }
    var inputValue = input.value;
    if (inputValue.length > 1) {
      var url = "/api/getPlaatsNaamSuggest?plaats=" + inputValue;
      this.timeout = setTimeout(function () {
        $.ajax({
          dataType: "json",
          url: url,
          success: function (data) {            
            suggest.suggestPlaatsen = data;
            suggest.fillSuggestBox();
          },
          error: function () { alert("xmlhttpproblem: " + xhr.status); }
        });
      }, 600)
    }
  },

  fillSuggestBox: function () {
    var suggestBox = document.getElementById('suggests');
    var content = ""
    for (var x = 0; x < suggest.suggestPlaatsen.length; x++) {
      var plaats = suggest.suggestPlaatsen[x];
      if (typeof (plaats) == "string")
        content += '<a href="/in/' + plaats + '">' + plaats + '</a>';
    }
    suggestBox.innerHTML = content;
    suggestBox.className = 'suggest-shown';

  },

  cancelEvent: function (evt) {
    if (evt.keyCode == 13) {
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
    if (suggestBox && el.id != 'suggest')
      suggestBox.className = 'suggest-hidden';

  }
}

document.onclick = suggest.hide;