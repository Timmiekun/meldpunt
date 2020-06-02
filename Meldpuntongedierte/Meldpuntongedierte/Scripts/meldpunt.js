$(function () {

  $("[data-role=overlay]").on('click', function (evt) {
    if (evt.target.dataset.role === "overlay") {
      document.body.classList.add("overlay-hidden");
      document.body.classList.remove("show-reaction-form");
    }
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

  $("[data-role=phone-desktop]").on('click', function (evt) {
    evt.preventDefault();

    let el = document.querySelector("[data-role=phone-desktop]");
    if (el.innerText == "Maak een afspraak") {

      el.innerText =  el.dataset.number;
    }
    else {
      el.innerText =  "Maak een afspraak";
    }
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