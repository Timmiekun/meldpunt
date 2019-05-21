$(function () {

  $("[data-action=open-toggle]").on('click', function (evt) {
    evt.preventDefault();
    let target = document.querySelector(evt.target.hash);
    target.classList.remove("hidden");
    evt.target.classList.add("hidden");
  });

  $("[data-role=overlay], [data-role=close-overlay]").on('click', function (evt) {
    if (evt.target.dataset.role === "overlay" || evt.target.dataset.role === "close-overlay") {
      document.body.classList.remove("show-overlay");
    }
  });

  $("[data-action=open-reaction-form]").on('click', function (evt) {
    evt.preventDefault();
    document.body.classList.add("show-overlay");
  });

  $("[data-role=show-contact-details]").on('click', function (evt) {
    if (evt.target.value === "true")
      $(".optional-fields").show();
    else
      $(".optional-fields").hide();
  });
});

function recaptureCallback(opt_widget_id) {
  document.querySelector("#sendreaction").removeAttribute("disabled");
  document.querySelector("#opt_widget_id").value = opt_widget_id;
}
