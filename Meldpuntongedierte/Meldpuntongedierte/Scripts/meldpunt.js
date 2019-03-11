$(function () {
  

  $(".goto-menu-mobile").on('click', function (evt) {
    evt.preventDefault();

    // prevent scroll trigger from closing menu
    window.menuWasOpen = true;
    $("html").toggleClass("showMe");
  });

  $('.defaultList li .description').hide();
  $('.defaultList li').on('click', function (evt) {
    $(evt.target).parent().find(".description").toggle();
  });

  var scrollTimeout;
  var previousScroll = 0;
  window.addEventListener("scroll", function (event) {
    let scrollTop = $(window).scrollTop();
    let diff = scrollTop - previousScroll;
    previousScroll = scrollTop;
    if (Math.abs(diff) < 5)
      return;

    // prevent scroll trigger from closing menu
    if (window.menuWasOpen) {
      window.menuWasOpen = false;
      return;
    }

    if (diff < 0) {
      $(".goto-menu-mobile").removeClass("hidden");
    }
    else {
      $(".goto-menu-mobile").addClass("hidden");
    }
  });
})
