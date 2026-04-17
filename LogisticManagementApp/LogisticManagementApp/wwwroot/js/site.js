(function () {
    function initPaginatedTable(table) {
        const pageSize = parseInt(table.dataset.pageSize || "10", 10);
        const tbody = table.querySelector("tbody");

        if (!tbody) return;

        const rows = Array.from(tbody.querySelectorAll("tr"));
        if (rows.length === 0) return;

        const wrapper = table.closest("[data-table-pagination-wrapper]") || table.parentElement;
        if (!wrapper) return;

        let currentPage = 1;
        const totalPages = Math.ceil(rows.length / pageSize);

        if (totalPages <= 1) return;

        const controls = document.createElement("div");
        controls.className = "table-pagination-controls";

        const info = document.createElement("div");
        info.className = "table-pagination-info";

        const buttons = document.createElement("div");
        buttons.className = "table-pagination-buttons";

        const prevBtn = document.createElement("button");
        prevBtn.type = "button";
        prevBtn.className = "btn btn-sm btn-outline-light";
        prevBtn.textContent = "Предишна";

        const nextBtn = document.createElement("button");
        nextBtn.type = "button";
        nextBtn.className = "btn btn-sm btn-outline-light";
        nextBtn.textContent = "Следваща";

        const pageNumbers = document.createElement("div");
        pageNumbers.className = "table-pagination-pages";

        buttons.appendChild(prevBtn);
        buttons.appendChild(pageNumbers);
        buttons.appendChild(nextBtn);

        controls.appendChild(info);
        controls.appendChild(buttons);

        wrapper.appendChild(controls);

        function renderPageNumbers() {
            pageNumbers.innerHTML = "";

            for (let i = 1; i <= totalPages; i++) {
                const pageBtn = document.createElement("button");
                pageBtn.type = "button";
                pageBtn.className = i === currentPage
                    ? "btn btn-sm btn-success"
                    : "btn btn-sm btn-outline-light";

                pageBtn.textContent = i.toString();

                pageBtn.addEventListener("click", function () {
                    currentPage = i;
                    render();
                });

                pageNumbers.appendChild(pageBtn);
            }
        }

        function render() {
            const start = (currentPage - 1) * pageSize;
            const end = start + pageSize;

            rows.forEach((row, index) => {
                row.style.display = index >= start && index < end ? "" : "none";
            });

            info.textContent = `Страница ${currentPage} от ${totalPages} (${rows.length} записа)`;

            prevBtn.disabled = currentPage === 1;
            nextBtn.disabled = currentPage === totalPages;

            renderPageNumbers();
        }

        prevBtn.addEventListener("click", function () {
            if (currentPage > 1) {
                currentPage--;
                render();
            }
        });

        nextBtn.addEventListener("click", function () {
            if (currentPage < totalPages) {
                currentPage++;
                render();
            }
        });

        render();
    }

    document.addEventListener("DOMContentLoaded", function () {
        const tables = document.querySelectorAll("[data-table-pagination='true']");
        tables.forEach(initPaginatedTable);
    });
})();