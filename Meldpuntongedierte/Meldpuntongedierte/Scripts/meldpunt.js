$(function () {

  $("[data-role=overlay]").on('click', function (evt) {
    if (evt.target.dataset.role === "overlay") {
      document.body.classList.add("overlay-hidden");
      document.body.classList.remove("show-reaction-form");
    }
  });

  $("[data-action=open-reaction-form]").on('click', function (evt) {
    document.body.classList.remove("overlay-hidden");
    document.body.classList.add("show-reaction-form");
  });

  $("[data-action=toggle-menu]").on('click', function (evt) {
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
});
