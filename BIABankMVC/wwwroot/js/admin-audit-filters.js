(function() {
    let debounceTimer;
    let filterUrl = '';

    function init(url) {
        filterUrl = url;
        bindEvents();
        restoreFiltersFromUrl();
    }

    function bindEvents() {
        // Debounced text inputs
        ['customerIdFilter', 'searchRef'].forEach(id => {
            const element = document.getElementById(id);
            if (element) {
                element.addEventListener('input', function() {
                    clearTimeout(debounceTimer);
                    debounceTimer = setTimeout(applyFilters, 300);
                });
            }
        });

        // Bind change events for dropdowns and date inputs
        ['actionFilter', 'entityFilter', 'fromDate', 'toDate'].forEach(id => {
            const element = document.getElementById(id);
            if (element) {
                element.addEventListener('change', applyFilters);
            }
        });
    }

    function restoreFiltersFromUrl() {
        const params = new URLSearchParams(window.location.search);
        
        const actionFilter = params.get('actionFilter');
        const entityFilter = params.get('entityFilter');
        const customerIdFilter = params.get('customerIdFilter');
        const searchRef = params.get('searchRef');
        const fromDate = params.get('fromDate');
        const toDate = params.get('toDate');

        if (actionFilter) document.getElementById('actionFilter').value = actionFilter;
        if (entityFilter) document.getElementById('entityFilter').value = entityFilter;
        if (customerIdFilter) document.getElementById('customerIdFilter').value = customerIdFilter;
        if (searchRef) document.getElementById('searchRef').value = searchRef;
        if (fromDate) document.getElementById('fromDate').value = fromDate;
        if (toDate) document.getElementById('toDate').value = toDate;
    }

    function getFilterParams() {
        const params = new URLSearchParams();
        
        const action = document.getElementById('actionFilter')?.value;
        const entity = document.getElementById('entityFilter')?.value;
        const customerId = document.getElementById('customerIdFilter')?.value;
        const searchRef = document.getElementById('searchRef')?.value;
        const fromDate = document.getElementById('fromDate')?.value;
        const toDate = document.getElementById('toDate')?.value;

        if (action) params.append('actionFilter', action);
        if (entity) params.append('entityFilter', entity);
        if (customerId) params.append('customerIdFilter', customerId);
        if (searchRef) params.append('searchRef', searchRef);
        if (fromDate) params.append('fromDate', fromDate);
        if (toDate) params.append('toDate', toDate);
        params.append('page', '1');
        params.append('pageSize', '50');

        return params.toString();
    }

    function applyFilters() {
        const container = document.getElementById('auditLogsContainer');
        if (!container) return;

        container.style.opacity = '0.5';

        const queryString = getFilterParams();
        
        // Update URL without reload
        const newUrl = `${filterUrl}?${queryString}`;
        window.history.pushState({}, '', newUrl);

        fetch(`${filterUrl}/FilterPartial?${queryString}`)
            .then(response => response.text())
            .then(html => {
                container.innerHTML = html;
                container.style.opacity = '1';
                updateLogCount();
            })
            .catch(error => {
                console.error('Error filtering audit logs:', error);
                container.style.opacity = '1';
            });
    }

    function clearFilters() {
        document.getElementById('actionFilter').value = '';
        document.getElementById('entityFilter').value = '';
        document.getElementById('customerIdFilter').value = '';
        document.getElementById('searchRef').value = '';
        document.getElementById('fromDate').value = '';
        document.getElementById('toDate').value = '';
        applyFilters();
    }

    function updateLogCount() {
        const rows = document.querySelectorAll('#auditLogsContainer tbody tr');
        const countBadge = document.getElementById('logCount');
        if (countBadge) {
            countBadge.textContent = `${rows.length} records`;
        }
    }

    function refreshLogs() {
        applyFilters();
    }

    function exportLogs() {
        const queryString = getFilterParams();
        window.location.href = `${filterUrl}/Export?${queryString}`;
    }

    window.AdminAuditFilters = {
        init: init,
        applyFilters: applyFilters,
        clearFilters: clearFilters,
        refreshLogs: refreshLogs,
        exportLogs: exportLogs
    };
})();
