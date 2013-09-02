var xhr = XHR();

var suggest = {
  suggestPlaatsen: [],
  timeout: null,
  getSuggests: function (evt, input) {
    if (this.timeout) {
      clearTimeout(this.timeout)
    }
    var inputValue = input.value;
    if (inputValue.length > 1) {
      var url = "/getPlaatsNaam.aspx?plaats=" + inputValue;
      this.timeout = setTimeout(function () {
        xhr.open("GET", url, true);
        xhr.onreadystatechange = suggest.handleSuggestXhr;
        xhr.send(null);
      }, 600)
    }
  },
  handleSuggestXhr: function () {
    if (xhr.readyState == 4) {
      if (xhr.status == 200) {
        suggest.suggestPlaatsen = eval('[' + xhr.responseText + ']');
        suggest.fillSuggestBox();
      }
      else {
        alert("xmlhttpproblem: " + xhr.status);
      }
    }
  },
  fillSuggestBox: function () {
    var suggestBox = document.getElementById('suggests');
    var content = ""
    for (var x in suggest.suggestPlaatsen) {

      var plaats = suggest.suggestPlaatsen[x];
      if (typeof (plaats) == "string")
        content += '<a href="/' + plaats + '">' + plaats + '</a>';
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