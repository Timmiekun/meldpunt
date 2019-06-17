$(function () {
  $("#SenderDescription")[0].oninvalid = function () {  
      let minLength = this.getAttribute("minlength");
      this.setCustomValidity("Gebruik minimaal " + minLength + " tekens");
  };

  // reset customvalidity stil buggy on chrome
  // https://stackoverflow.com/questions/14043589/html5-oninvalid-doesnt-work-after-fixed-the-input-field/30136146#30136146
  $("#SenderDescription")[0].oninput = function () {    
      this.setCustomValidity("");      
  };

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
