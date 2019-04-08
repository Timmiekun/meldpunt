$(function () {
  $(".m-modal .background").on('click', function () {
    $(".m-modal").addClass("hidden");
  });
});

var pageSuggest = {
  suggests: {},
  timeout: null,
  type: null,
  callback: null,
  imageTemplate: document.querySelector("#image-result"),

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
    suggestBox.innerHTML = "";

    if (!pageSuggest.suggests.length) {
      suggestBox.innerHTML = "Niets gevonden";
      return;
    }

    for (var x = 0; x < pageSuggest.suggests.length; x++) {
      var result = pageSuggest.suggests[x];
      if (result) {
        if (pageSuggest.type === "image")
          suggestBox.appendChild(pageSuggest.createImageElement(result));
        else
          suggestBox.appendChild(pageSuggest.createPageElement(result));
      }     
    }

    suggestBox.querySelectorAll("[data-role=result-item]").forEach(function (item) {
      if (pageSuggest.type === "image") {
        item.querySelector("button").addEventListener("click", function (evt) {        
          if (pageSuggest.callback) {
            var imageUrl = item.dataset.url;
            if (item.dataset.width)
              imageUrl += "&width=" + item.dataset.width;
            if (item.dataset.height)
              imageUrl += "&height=" + item.dataset.height;

            pageSuggest.callback(imageUrl, {
              alt: item.dataset.title
            });
          }
          else {          
            document.querySelector("#Image").value = item.dataset.url;
            document.querySelector("#imagePreview").src = item.dataset.url + "&width=200&height=140";
          }
          $(".m-modal").addClass("hidden");

        });
      }
      else {
        item.addEventListener("click", function () {
          document.querySelector("#parentId").value = item.dataset.id;
          document.querySelector("#inputGroupPrepend").innerHTML = item.dataset.url;
          $(".m-modal").addClass("hidden");
        });
      }
    });
  },

  createPageElement: function (result) {
    let el = document.createElement("div");
    el.dataset.url = result.Url;
    el.dataset.id = result.Id;
    el.dataset.role = "result-item";
    el.className = "list-item";

    el.innerHTML += '<div class="title">' + result.Title + '</div>';
    el.innerHTML += '<div class="url">' + result.Url + '</div>';
    return el;
  },

  createImageElement: function (result) {
    let template = this.imageTemplate;
    let el = document.importNode(template.content, true).querySelector("div");
    let url = "/image?name=" + result.Title;

    el.dataset.url = url;
    el.dataset.title = result.Title;

    el.querySelector("img").src = url + '&width=200&height=140';

    if (pageSuggest.showAdvanced) {
      el.querySelector("[data-role=size-select]").classList.remove("hidden");
    }
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

function addPropertyToDataset(el) {
  el.parentElement.parentElement.dataset[el.name] = el.value;
}

function OpenModal(type, callback, showAdvanced) {
  pageSuggest.type = type;
  pageSuggest.callback = callback;
  pageSuggest.showAdvanced = showAdvanced;

  document.querySelector('#suggests').innerHTML = "";
  document.querySelector('#modal-search').value = "";
  
  $(".m-modal").removeClass("hidden");

  document.querySelector('#modal-search').focus();
}

function htmlToElement(html) {
  var template = document.createElement('template');
  html = html.trim(); // Never return a text node of whitespace as the result
  template.innerHTML = html;
  return template.content.firstChild;
}
