$(function () {
	$(".js-delete-page").submit(function() {
		if (!confirm("pagina verwijderen?"))
			return false;
	});
})