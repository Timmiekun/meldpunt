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
    var content = "";
    for (var x = 0; x < pageSuggest.suggests.length; x++) {
      var result = pageSuggest.suggests[x];
    console.log(result)
      if (result) {
        content += '<div  data-id=' + result.Id + ' class="list-item">';
        content += '<div class="title">' + result.Title + '</div>';
        content += '<div class="url">' + result.Url + '</div>';
        content += '</div>';
      }
    }
    suggestBox.innerHTML = content;
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
  $(".modal").show();
}

