const linhasInput = document.getElementById('linhas');
const assentosInput = document.getElementById('assentosPorLinha');
const gerarBtn = document.getElementById('gerarPreview');
const previewContainer = document.getElementById('previewContainer');
const preview = document.getElementById('assentosPreview');

function gerarPreview() {
    const linhas = parseInt(linhasInput.value, 10) || 0;
    const colunas = parseInt(assentosInput.value, 10) || 0;
    preview.innerHTML = '';
    if (linhas <= 0 || colunas <= 0) {
        previewContainer.style.display = 'none';
        return;
    }
    previewContainer.style.display = 'block';

    for (let i = 0; i < linhas; i++) {
        const row = document.createElement('div');
        row.className = 'd-flex gap-2 align-items-center';
        const rowLabel = document.createElement('div');
        rowLabel.style.minWidth = '28px';
        rowLabel.textContent = String.fromCharCode(65 + i);
        row.appendChild(rowLabel);

        for (let j = 1; j <= colunas; j++) {
            const seat = document.createElement('div');
            seat.className = 'btn btn-sm btn-outline-secondary';
            seat.style.minWidth = '48px';
            seat.style.padding = '5px 8px';
            seat.style.boxSizing = 'border-box';
            seat.style.whiteSpace = 'nowrap';
            seat.style.textAlign = 'center';
            seat.style.margin = '2px';
            seat.textContent = String.fromCharCode(65 + i) + j;
            row.appendChild(seat);
        }
        preview.appendChild(row);
    }
}

gerarBtn.addEventListener('click', gerarPreview);
linhasInput.addEventListener('change', () => { if (previewContainer.style.display !== 'none') gerarPreview(); });
assentosInput.addEventListener('change', () => { if (previewContainer.style.display !== 'none') gerarPreview(); });