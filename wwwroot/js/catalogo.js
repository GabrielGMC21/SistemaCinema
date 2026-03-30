
function initializeSeats() {
    const selectedSeats = new Set();
    const assentoContainer = document.getElementById('assentoContainer');
    const reservarBtn = document.getElementById('btnContinuar');

    if (!assentoContainer || !reservarBtn) return;

    function updateFormState() {
        assentoContainer.innerHTML = '';
        
        selectedSeats.forEach(id => {
            const input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'assentoIds';
            input.value = id;
            assentoContainer.appendChild(input);
        });
        
        reservarBtn.disabled = selectedSeats.size === 0;
    }

    document.querySelectorAll('.seat-btn').forEach(function (btn) {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const id = this.dataset.assentoId;
            if (!id) return;

            if (selectedSeats.has(id)) {
                selectedSeats.delete(id);
                this.classList.remove('btn-primary');
                this.classList.add('btn-outline-success');
            } else {
                selectedSeats.add(id);
                this.classList.remove('btn-outline-success');
                this.classList.add('btn-primary');
            }

            updateFormState();
        });
    });

    updateFormState();
}

function initializeResumoCompra() {
    const subtotalIngressosEl = document.getElementById('subtotalIngressos');
    const subtotalProdutosEl = document.getElementById('subtotalProdutos');
    const totalSemDescontoEl = document.getElementById('totalSemDesconto');

    if (!subtotalProdutosEl) return;

    function formatCurrency(v) {
        return v.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
    }

    function extractPrice(priceStr) {
        return parseFloat(priceStr.replace(/[^\d,-]/g, '').replace(',', '.')) || 0;
    }

    function updateTotals() {
        const qtyInputs = document.querySelectorAll('.produto-quantidade');
        let subtotalProdutos = 0;
        qtyInputs.forEach(function (inp) {
            const q = parseInt(inp.value) || 0;
            const p = parseFloat(inp.dataset.preco) || 0;
            subtotalProdutos += q * p;
        });

        const precoIngressos = subtotalIngressosEl ? extractPrice(subtotalIngressosEl.textContent) : 0;
        subtotalProdutosEl.textContent = formatCurrency(subtotalProdutos);
        const total = precoIngressos + subtotalProdutos;
        totalSemDescontoEl.textContent = formatCurrency(total);
    }

    document.querySelectorAll('.produto-quantidade').forEach(inp => {
        inp.addEventListener('input', updateTotals);
    });

    updateTotals();
}

document.addEventListener('DOMContentLoaded', function() {
    initializeSeats();
    initializeResumoCompra();
});
