$(function () {
  $(".js-delete-page").submit(function () {
    if (!confirm("pagina verwijderen?"))
      return false;
  });

  $("[data-action-confirm]").on('click', function (evt) {
    evt.preventDefault();
    let target = this.href;

    if (confirm('Verwijderen? Zeker weten?'))
      window.location = target;

    return false;
  });

  $("#file").change(function () {
    readURL(this);
  });

});

function readURL(input) {

  if (input.files && input.files[0]) {
    var reader = new FileReader();

    reader.onload = function (e) {
      $('#imagePreview').attr('src', e.target.result);
    };

    reader.readAsDataURL(input.files[0]);
  }
}