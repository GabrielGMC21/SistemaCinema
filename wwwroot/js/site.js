
document.addEventListener('DOMContentLoaded', function () {
	document.querySelectorAll('.delete-form').forEach(function (form) {
		form.addEventListener('submit', function (e) {
			let mensagem = 'Tem certeza que deseja excluir?';
			var confirmed = confirm(mensagem);
			if (!confirmed) {
				e.preventDefault();
			}
		});
	});
});

