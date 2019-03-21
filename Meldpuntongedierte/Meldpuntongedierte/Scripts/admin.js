$(function () {
  $(".js-delete-page").submit(function () {
    if (!confirm("pagina verwijderen?"))
      return false;
  });


  $("[data-action-remove-redirect]").on('click', function (evt) {
    evt.preventDefault();
    let id = this.dataset.redirect;
    console.log(this, id);
    if (confirm('Verwijderen? Zeker weten?'))
      window.location = "/admin/removeRedirect?id=" + id;
  
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