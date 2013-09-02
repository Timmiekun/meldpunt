$(function () {
  $(".defaultList .description").hide();

  $(".defaultList h3, .defaultList .description").click(function () {
    $(this).parent().find(".description").slideToggle('slow', 'easeOutBounce', null);
    return false;
  })

  $("#mainNav ul").hide();

  $("#mainNav li").mouseenter(function () {
    $(this).find("ul").show();
  });

  $("#mainNav li").mouseleave(function () {
    $(this).find("ul").hide();
  });
});
