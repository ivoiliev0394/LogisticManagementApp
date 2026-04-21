(function () {
    function shouldAttachTableSearch(table) {
        const explicitValue = table.dataset.tableSearch;
        if (explicitValue === "true") return true;
        if (explicitValue === "false") return false;

        const path = window.location.pathname.toLowerCase();
        return (path.includes("/company") || path.includes("/admin")) && !path.includes("/clientportal");
    }

    function initPaginatedTable(table) {
        const pageSize = parseInt(table.dataset.pageSize || "10", 10);
        const tbody = table.querySelector("tbody");

        if (!tbody) return;

        const allRows = Array.from(tbody.querySelectorAll("tr"));
        if (allRows.length === 0) return;

        const wrapper = table.closest("[data-table-pagination-wrapper]") || table.parentElement;
        if (!wrapper) return;

        let currentPage = 1;
        let filteredRows = [...allRows];

        const headerCells = Array.from(table.querySelectorAll("thead th"));
        const searchableHeaders = headerCells
            .map((cell, index) => ({
                name: cell.textContent.trim(),
                index
            }))
            .filter(x => x.name && !x.name.toLowerCase().includes("действия"));

        let controlsHost = wrapper;

        if (shouldAttachTableSearch(table)) {
            const toolbar = document.createElement("div");
            toolbar.className = "table-search-toolbar";

            const searchGroup = document.createElement("div");
            searchGroup.className = "table-search-group";

            const searchLabel = document.createElement("label");
            searchLabel.className = "form-label mb-1";
            searchLabel.textContent = "Търсене";

            const searchInput = document.createElement("input");
            searchInput.type = "search";
            searchInput.className = "form-control";
            searchInput.placeholder = "Търси във всички колони...";

            searchGroup.appendChild(searchLabel);
            searchGroup.appendChild(searchInput);

            const filterColumnGroup = document.createElement("div");
            filterColumnGroup.className = "table-search-group";

            const filterColumnLabel = document.createElement("label");
            filterColumnLabel.className = "form-label mb-1";
            filterColumnLabel.textContent = "Филтър колона";

            const filterColumnSelect = document.createElement("select");
            filterColumnSelect.className = "form-select";

            const defaultOption = document.createElement("option");
            defaultOption.value = "";
            defaultOption.textContent = "Избери колона";
            filterColumnSelect.appendChild(defaultOption);

            searchableHeaders.forEach(header => {
                const option = document.createElement("option");
                option.value = header.index.toString();
                option.textContent = header.name;
                filterColumnSelect.appendChild(option);
            });

            filterColumnGroup.appendChild(filterColumnLabel);
            filterColumnGroup.appendChild(filterColumnSelect);

            const filterValueGroup = document.createElement("div");
            filterValueGroup.className = "table-search-group";

            const filterValueLabel = document.createElement("label");
            filterValueLabel.className = "form-label mb-1";
            filterValueLabel.textContent = "Стойност";

            const filterValueInput = document.createElement("input");
            filterValueInput.type = "text";
            filterValueInput.className = "form-control";
            filterValueInput.placeholder = "Въведи стойност...";

            filterValueGroup.appendChild(filterValueLabel);
            filterValueGroup.appendChild(filterValueInput);

            const clearGroup = document.createElement("div");
            clearGroup.className = "table-search-group table-search-group-actions";

            const clearButton = document.createElement("button");
            clearButton.type = "button";
            clearButton.className = "btn btn-outline-light";
            clearButton.textContent = "Изчисти";

            clearGroup.appendChild(clearButton);

            toolbar.appendChild(searchGroup);
            toolbar.appendChild(filterColumnGroup);
            toolbar.appendChild(filterValueGroup);
            toolbar.appendChild(clearGroup);

            wrapper.parentNode.insertBefore(toolbar, wrapper);
            controlsHost = wrapper;

            function getRowText(row) {
                return row.textContent.replace(/\s+/g, " ").trim().toLowerCase();
            }

            function getCellText(row, index) {
                const cells = row.querySelectorAll("td");
                if (index < 0 || index >= cells.length) return "";
                return cells[index].textContent.replace(/\s+/g, " ").trim().toLowerCase();
            }

            function applyFilters() {
                const searchTerm = searchInput.value.trim().toLowerCase();
                const selectedColumnIndex = filterColumnSelect.value === ""
                    ? null
                    : parseInt(filterColumnSelect.value, 10);
                const filterValue = filterValueInput.value.trim().toLowerCase();

                filteredRows = allRows.filter(row => {
                    const matchesSearch = !searchTerm || getRowText(row).includes(searchTerm);
                    const matchesColumnFilter = selectedColumnIndex === null || !filterValue
                        ? true
                        : getCellText(row, selectedColumnIndex).includes(filterValue);

                    return matchesSearch && matchesColumnFilter;
                });

                currentPage = 1;
                render();
            }

            searchInput.addEventListener("input", applyFilters);
            filterColumnSelect.addEventListener("change", applyFilters);
            filterValueInput.addEventListener("input", applyFilters);
            clearButton.addEventListener("click", function () {
                searchInput.value = "";
                filterColumnSelect.value = "";
                filterValueInput.value = "";
                applyFilters();
            });
        }

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

        controlsHost.appendChild(controls);

        function getTotalPages() {
            return Math.max(1, Math.ceil(filteredRows.length / pageSize));
        }

        function renderPageNumbers() {
            pageNumbers.innerHTML = "";
            const totalPages = getTotalPages();

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
            const totalPages = getTotalPages();
            currentPage = Math.min(currentPage, totalPages);

            const start = (currentPage - 1) * pageSize;
            const end = start + pageSize;
            const visibleSet = new Set(filteredRows.slice(start, end));

            allRows.forEach(row => {
                row.style.display = visibleSet.has(row) ? "" : "none";
            });

            info.textContent = `Страница ${currentPage} от ${totalPages} (${filteredRows.length} записа)`;

            prevBtn.disabled = currentPage === 1;
            nextBtn.disabled = currentPage === totalPages || filteredRows.length === 0;

            renderPageNumbers();
        }

        prevBtn.addEventListener("click", function () {
            if (currentPage > 1) {
                currentPage--;
                render();
            }
        });

        nextBtn.addEventListener("click", function () {
            if (currentPage < getTotalPages()) {
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

(function () {
    function initCustomDeleteConfirm() {
        const modalElement = document.getElementById("customDeleteConfirmModal");
        const messageElement = document.getElementById("customDeleteConfirmModalMessage");
        const approveButton = document.getElementById("customDeleteConfirmModalApprove");

        if (!modalElement || !messageElement || !approveButton || typeof bootstrap === "undefined") {
            return;
        }

        const modal = new bootstrap.Modal(modalElement);
        let pendingAction = null;

        function normalizeText(value) {
            return (value || "").toString().trim().toLowerCase();
        }

        function hasDeleteKeyword(value) {
            const text = normalizeText(value);
            return text.includes("delete")
                || text.includes("remove")
                || text.includes("destroy")
                || text.includes("изтриване")
                || text.includes("изтрий")
                || text.includes("премахни");
        }

        function shouldConfirmForm(form) {
            if (!form) {
                return false;
            }

            if (form.hasAttribute("data-confirm-message")) {
                return true;
            }

            const action = form.getAttribute("action") || "";
            if (hasDeleteKeyword(action)) {
                return true;
            }

            const submitButton = form.querySelector("button[type='submit'], input[type='submit']");
            if (!submitButton) {
                return false;
            }

            const buttonText = submitButton.value || submitButton.textContent || "";
            const buttonClasses = submitButton.className || "";
            return hasDeleteKeyword(buttonText) || hasDeleteKeyword(buttonClasses);
        }

        function shouldConfirmAnchor(anchor) {
            if (!anchor) {
                return false;
            }

            if (anchor.hasAttribute("data-confirm-message")) {
                return true;
            }

            const href = anchor.getAttribute("href") || "";
            const text = anchor.textContent || "";
            const classes = anchor.className || "";
            return hasDeleteKeyword(href) || hasDeleteKeyword(text) || hasDeleteKeyword(classes);
        }

        function getMessage(trigger, form) {
            return trigger?.dataset?.confirmMessage
                || form?.dataset?.confirmMessage
                || "Сигурни ли сте, че искате да изтриете този запис?";
        }

        function openModal(message, action) {
            pendingAction = action;
            messageElement.textContent = message || "Сигурни ли сте, че искате да изтриете този запис?";
            modal.show();
        }

        document.addEventListener("submit", function (event) {
            const form = event.target.closest("form");
            if (!shouldConfirmForm(form)) {
                return;
            }

            if (form.dataset.confirmed === "true") {
                form.dataset.confirmed = "false";
                return;
            }

            event.preventDefault();
            openModal(getMessage(null, form), {
                type: "submit",
                form: form
            });
        });

        document.addEventListener("click", function (event) {
            const trigger = event.target.closest("a, button, input[type='submit'], [data-confirm-message]");
            if (!trigger || trigger.tagName === "FORM") {
                return;
            }

            const form = trigger.closest("form");

            if ((trigger.matches("button, input[type='submit']") || trigger.hasAttribute("data-confirm-message")) && form && shouldConfirmForm(form)) {
                if (trigger.type === "submit" || trigger.tagName === "BUTTON" || trigger.tagName === "INPUT") {
                    event.preventDefault();
                    openModal(getMessage(trigger, form), {
                        type: "submit",
                        form: form
                    });
                    return;
                }
            }

            if (trigger.tagName === "A" && trigger.href && shouldConfirmAnchor(trigger)) {
                event.preventDefault();
                openModal(getMessage(trigger, form), {
                    type: "navigate",
                    href: trigger.href
                });
            }
        });

        approveButton.addEventListener("click", function () {
            if (!pendingAction) {
                modal.hide();
                return;
            }

            if (pendingAction.type === "submit" && pendingAction.form) {
                pendingAction.form.dataset.confirmed = "true";
                pendingAction.form.requestSubmit();
            }
            else if (pendingAction.type === "navigate" && pendingAction.href) {
                window.location.href = pendingAction.href;
            }

            pendingAction = null;
            modal.hide();
        });

        modalElement.addEventListener("hidden.bs.modal", function () {
            pendingAction = null;
        });
    }

    document.addEventListener("DOMContentLoaded", initCustomDeleteConfirm);
})();
