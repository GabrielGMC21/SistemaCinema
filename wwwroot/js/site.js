
document.addEventListener('DOMContentLoaded', function () {
	document.querySelectorAll('.delete-form').forEach(function (form) {
		form.addEventListener('submit', function (e) {
			var confirmed = confirm('Tem certeza que deseja excluir este filme?');
			if (!confirmed) {
				e.preventDefault();
			}
		});
	});
});
