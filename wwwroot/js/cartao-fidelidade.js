document.addEventListener('DOMContentLoaded', function () {
    var valorInput = document.getElementById('valorPontos');
    var descontoInfo = document.getElementById('descontoInfo');
    var btnResgatar = document.getElementById('btnResgatar');
    var maxBtn = document.getElementById('maxBtn');

    function update() {
        if (!valorInput || !descontoInfo) return;
        var val = parseInt(valorInput.value, 10);
        if (isNaN(val) || val < 10) {
            descontoInfo.textContent = '0% de desconto';
            descontoInfo.style.color = '#6c757d';
            if (btnResgatar) btnResgatar.disabled = true;
            return;
        }
        var discount = Math.floor(val / 10);
        if (discount > 50) discount = 50;
        descontoInfo.textContent = discount + '% de desconto';
        descontoInfo.style.color = discount > 0 ? '#28a745' : '#6c757d';
        if (btnResgatar) btnResgatar.disabled = false;
    }

    if (valorInput) {
        valorInput.addEventListener('input', update);
    }

    if (maxBtn) {
        maxBtn.addEventListener('click', function () {
            if (!valorInput) return;
            var maxAttr = valorInput.getAttribute('max');
            var max = parseInt(maxAttr, 10) || parseInt(valorInput.max, 10) || 0;
            if (max > 0) {
                valorInput.value = max;
                update();
            }
        });
    }

    update();
});

