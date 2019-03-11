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
    else
      alert('nou goed dan');

    return false;
  });
});