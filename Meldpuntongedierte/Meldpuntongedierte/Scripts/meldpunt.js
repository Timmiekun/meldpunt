$(function () {

  var el = '<button class="goto-menu-mobile">menu</button>';
  document.body.innerHTML += el;

  $(".goto-menu-mobile").on('click', function (evt) {
    evt.preventDefault();
    $("#tabs").toggleClass("showMe");
  })

  document.body.onload = function () {
    var width = window.innerWidth
      || document.documentElement.clientWidth
      || document.body.clientWidth;
    // if (width <= 768) {
    //   window.setTimeout(function () { document.getElementsByClassName('content')[0].scrollIntoView() }, 500);
    // }
  }

  $('.defaultList li .description').hide();
  $('.defaultList li').on('click', function (evt) {    
    $(evt.target).parent().find(".description").toggle();
  });  
})
