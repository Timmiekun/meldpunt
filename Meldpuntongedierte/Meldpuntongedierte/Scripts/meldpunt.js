$(function () {

  $('.defaultList li .description').hide();
  $('.defaultList li').on('click', function (evt) {
    $(evt.target).parent().find(".description").toggle();
  });
})