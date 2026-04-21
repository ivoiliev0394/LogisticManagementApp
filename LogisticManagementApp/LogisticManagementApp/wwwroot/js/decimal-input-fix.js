(function () {
    function normalizeDecimalValue(value) {
        return typeof value === "string" ? value.replace(/,/g, ".") : value;
    }

    function patchJQueryValidation() {
        if (!window.jQuery || !jQuery.validator) {
            return false;
        }

        jQuery.validator.methods.number = function (value, element) {
            if (this.optional(element)) {
                return true;
            }

            var normalized = normalizeDecimalValue(value);
            return /^-?(?:\d+|\d*[.]\d+)$/.test(normalized);
        };

        jQuery.validator.methods.range = function (value, element, param) {
            if (this.optional(element)) {
                return true;
            }

            var normalizedValue = Number(normalizeDecimalValue(value));
            return !Number.isNaN(normalizedValue) && normalizedValue >= param[0] && normalizedValue <= param[1];
        };

        return true;
    }

    function patchNumericInputs(root) {
        var scope = root || document;
        var candidates = scope.querySelectorAll('input[type="number"], input[data-val-number], input[data-decimal-field="true"]');

        candidates.forEach(function (input) {
            if (input.type === 'number') {
                input.setAttribute('step', 'any');
            }

            if (!input.getAttribute('inputmode')) {
                input.setAttribute('inputmode', 'decimal');
            }
        });
    }

    function normalizeForms(root) {
        var scope = root || document;
        scope.querySelectorAll('form').forEach(function (form) {
            if (form.dataset.decimalFixBound === 'true') {
                return;
            }

            form.dataset.decimalFixBound = 'true';
            form.addEventListener('submit', function () {
                form.querySelectorAll('input[type="number"], input[data-val-number], input[data-decimal-field="true"], input[inputmode="decimal"]').forEach(function (input) {
                    input.value = normalizeDecimalValue(input.value);
                });
            });
        });
    }

    function boot(root) {
        patchNumericInputs(root);
        normalizeForms(root);
    }

    document.addEventListener('DOMContentLoaded', function () {
        boot(document);

        var attempts = 0;
        var timer = window.setInterval(function () {
            attempts += 1;
            if (patchJQueryValidation() || attempts >= 20) {
                window.clearInterval(timer);
            }
        }, 250);
    });
})();
