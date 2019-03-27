$(function () {
  $(".m-modal .background").on('click', function () {
    $(".m-modal").addClass("hidden");
  });
});


var pageSuggest = {
  suggests: {},
  timeout: null,

  getSuggests: function (evt, input) {
    if (this.timeout) {
      clearTimeout(this.timeout);
    }
    var inputValue = input.value;
    if (inputValue.length > 1) {
      var url = "/api/getPageSuggest?query=" + inputValue;
      this.timeout = setTimeout(function () {
        $.ajax({
          dataType: "json",
          url: url,
          success: function (data) {
            pageSuggest.suggests = data;
            pageSuggest.fillSuggestBox();
          },
          error: function () { alert("xmlhttpproblem: " + xhr.status); }
        });
      }, 600);
    }
  },

  fillSuggestBox: function () {
    var suggestBox = document.querySelector('#suggests');
    let content = "";
    for (var x = 0; x < pageSuggest.suggests.length; x++) {
      var result = pageSuggest.suggests[x];
      if (result) {
        let el = '<div data-url='+result.Url+' data-id=' + result.Id + ' class="list-item">';
        el += '<div class="title">' + result.Title + '</div>';
        el += '<div class="url">' + result.Url + '</div>';
        el += '</div>';
        content += el;
      }
    }
    console.log(content);
    suggestBox.innerHTML = content;
    
    suggestBox.querySelectorAll(".list-item").forEach(function (elem) {     
      elem.addEventListener("click", function (evt) {
        document.querySelector("#inputGroupPrepend").innerHTML = elem.dataset.url;
        document.querySelector("#parentId").value = elem.dataset.id;
        $(".m-modal").addClass("hidden");
      });
    });
  },

  cancelEvent: function (evt) {
    if (evt.keyCode === 13) {

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
    var suggestBox = document.querySelector('.m-modal');
    evt = evt || window.event;
    var el = evt.target || evt.srcElement;
    if (suggestBox && el.id !== 'suggest')
      suggestBox.className = 'suggest-hidden';
  }
};

function OpenModal() {
  $(".m-modal").removeClass("hidden");
}

function htmlToElement(html) {
  var template = document.createElement('template');
  html = html.trim(); // Never return a text node of whitespace as the result
  template.innerHTML = html;
  return template.content.firstChild;
}
