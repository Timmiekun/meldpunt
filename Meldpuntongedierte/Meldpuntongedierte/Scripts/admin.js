$(function () {
  $(".subNav").hide();

  $(".menu a").click(function (evt) {
    evt.preventDefault();
    var $el = $(this);
    console.log($el.text());
    $el.parent().find(".subNav").first().slideToggle(80);
  })
})