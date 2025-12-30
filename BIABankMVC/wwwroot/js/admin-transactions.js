(function() {
    let debounceTimer;
    let filterUrl = '';

    function init(url) {
        filterUrl = url;
        bindEvents();
    }

    function bindEvents() {
        const referenceInput = document.getElementById('reference');
        if (referenceInput) {
            referenceInput.addEventListener('input', function() {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(applyFilters, 300);
            });
        }
    }

    function getFilterParams() {
        const params = new URLSearchParams();
        
        const type = document.getElementById('transactionType')?.value;
        const status = document.getElementById('status')?.value;
        const reference = document.getElementById('reference')?.value;
        const fromDate = document.getElementById('fromDate')?.value;
        const toDate = document.getElementById('toDate')?.value;

        if (type) params.append('TransactionType', type);
        if (status) params.append('Status', status);
        if (reference) params.append('Reference', reference);
        if (fromDate) params.append('FromDate', fromDate);
        if (toDate) params.append('ToDate', toDate);
        params.append('Limit', '100');

        return params.toString();
    }

    function applyFilters() {
        const container = document.getElementById('transactionsListContainer');
        if (!container) return;

        container.style.opacity = '0.5';

        fetch(`${filterUrl}?${getFilterParams()}`)
            .then(response => response.text())
            .then(html => {
                container.innerHTML = html;
                container.style.opacity = '1';
                updateTransactionCount();
            })
            .catch(error => {
                console.error('Error filtering transactions:', error);
                container.style.opacity = '1';
            });
    }

    function clearFilters() {
        document.getElementById('transactionType').value = '';
        document.getElementById('status').value = '';
        document.getElementById('reference').value = '';
        document.getElementById('fromDate').value = '';
        document.getElementById('toDate').value = '';
        applyFilters();
    }

    function updateTransactionCount() {
        const rows = document.querySelectorAll('#transactionsListContainer tbody tr');
        const countBadge = document.getElementById('transactionCount');
        if (countBadge) {
            countBadge.textContent = `Showing: ${rows.length}`;
        }
    }

    // Auto-refresh every 30 seconds
    setInterval(() => {
        if (filterUrl) applyFilters();
    }, 30000);

    window.AdminTransactions = {
        init: init,
        applyFilters: applyFilters,
        clearFilters: clearFilters
    };
})();
