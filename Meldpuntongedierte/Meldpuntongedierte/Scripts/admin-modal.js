$(function () {
  $(".m-modal .background").on('click', function () {
    $(".m-modal").addClass("hidden");
  });
});


var pageSuggest = {
  suggests: {},
  timeout: null,
  type: null,

  getSuggests: function (evt, input) {

    if (this.timeout) {
      clearTimeout(this.timeout);
    }

    var inputValue = input.value;
    if (inputValue.length > 1) {
      var url = "/api/getPageSuggest?query=" + inputValue;
      if (pageSuggest.type === "image")
        url = "/api/getImageSuggest?query=" + inputValue;

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
        console.log("pageSuggest.type", pageSuggest.type);
        if (pageSuggest.type === "image")
          content += pageSuggest.createImageElement(result);
        else
          content += pageSuggest.createPageElement(result);
      }
    }

    suggestBox.innerHTML = content;
    
    suggestBox.querySelectorAll(".list-item").forEach(function (elem) {     
      elem.addEventListener("click", function (evt) {
        document.querySelector("#inputGroupPrepend").innerHTML = elem.dataset.url;
        if (pageSuggest.type === "image") {
          document.querySelector("#Image").value = elem.dataset.url;
          document.querySelector("#imagePreview").src = elem.dataset.url + "&width=200&height=140";
        }
        else
          document.querySelector("#parentId").value = elem.dataset.id;

        $(".m-modal").addClass("hidden");
      });
    });
  },

  createPageElement: function(result) {
    let el = '<div data-url="' + result.Url + '" data-id="' + result.Id + '" class="list-item">';
    el += '<div class="title">' + result.Title + '</div>';
    el += '<div class="url">' + result.Url + '</div>';
    el += '</div>';
    return el;
  },

  createImageElement: function (result) {
    let url = "/image?name=" + result.Title;

    let el = '<div data-url="' + url + '" class="list-item list-item-image">';
        el += '<img src="' + url +'&width=200&height=140"/>';
        el += '</div>';
    return el;
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

function OpenModal(type) {
  pageSuggest.type = type;
  $(".m-modal").removeClass("hidden");
}

function htmlToElement(html) {
  var template = document.createElement('template');
  html = html.trim(); // Never return a text node of whitespace as the result
  template.innerHTML = html;
  return template.content.firstChild;
}
