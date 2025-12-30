(function () {
    let debounceTimer;
    let accountId;

    function init(id) {
        accountId = id;
        bindEvents();
    }

    function bindEvents() {
        const referenceInput = document.getElementById('reference');
        const typeSelect = document.getElementById('transactionType');
        const fromDateInput = document.getElementById('fromDate');
        const toDateInput = document.getElementById('toDate');

        if (referenceInput) {
            referenceInput.addEventListener('input', function () {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(applyFilters, 300);
            });
        }

        if (typeSelect) typeSelect.addEventListener('change', applyFilters);
        if (fromDateInput) fromDateInput.addEventListener('change', applyFilters);
        if (toDateInput) toDateInput.addEventListener('change', applyFilters);
    }

    function applyFilters() {
        const params = new URLSearchParams();

        const type = document.getElementById('transactionType')?.value;
        const fromDate = document.getElementById('fromDate')?.value;
        const toDate = document.getElementById('toDate')?.value;
        const reference = document.getElementById('reference')?.value;

        if (type) params.append('TransactionType', type);
        if (fromDate) params.append('FromDate', fromDate);
        if (toDate) params.append('ToDate', toDate);
        if (reference) params.append('Reference', reference);

        const container = document.getElementById('transactionsContainer');
        if (!container) return;

        container.style.opacity = '0.5';

        fetch(`/Accounts/GetTransactions/${accountId}?${params.toString()}`)
            .then(response => response.text())
            .then(html => {
                container.innerHTML = html;
                container.style.opacity = '1';
            })
            .catch(error => {
                console.error('Error filtering transactions:', error);
                container.style.opacity = '1';
            });
    }

    function clearFilters() {
        const typeSelect = document.getElementById('transactionType');
        const fromDateInput = document.getElementById('fromDate');
        const toDateInput = document.getElementById('toDate');
        const referenceInput = document.getElementById('reference');

        if (typeSelect) typeSelect.value = '';
        if (fromDateInput) fromDateInput.value = '';
        if (toDateInput) toDateInput.value = '';
        if (referenceInput) referenceInput.value = '';

        applyFilters();
    }

    window.AccountTransactions = {
        init: init,
        applyFilters: applyFilters,
        clearFilters: clearFilters
    };
})();
