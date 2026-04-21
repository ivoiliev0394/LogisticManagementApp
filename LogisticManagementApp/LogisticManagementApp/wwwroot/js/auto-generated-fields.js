document.addEventListener('DOMContentLoaded', () => {
    const autoFieldNames = new Set([
        'BranchCode', 'OrderNo', 'ShipmentNo', 'PackageNo',
        'AgreementNumber', 'QuoteNumber', 'ChargeCode', 'InvoiceNo', 'CreditNoteNo',
        'DocumentNo', 'BookingNo', 'ConsolidationNo', 'RouteCode',
        'RegistrationNumber', 'TripNo', 'TailNumber', 'FlightNumber', 'UldNumber', 'ImoNumber',
        'TrainNumber', 'RailCarNumber', 'ServiceCode', 'MovementNo',
        'ContainerNumber', 'SealNumber', 'VoyageNumber',
        'LineNo', 'LegNo', 'SequenceNo', 'SequenceNumber', 'SegmentNo', 'VersionNo'
    ]);

    document.querySelectorAll('input[name], textarea[name], select[name]').forEach((field) => {
        const name = field.getAttribute('name');
        if (!name || !autoFieldNames.has(name)) {
            return;
        }

        if (field.tagName === 'SELECT') {
            field.setAttribute('disabled', 'disabled');

            if (field.name && !field.form.querySelector(`input[type="hidden"][name="${field.name}"]`)) {
                const hidden = document.createElement('input');
                hidden.type = 'hidden';
                hidden.name = field.name;
                hidden.value = field.value;
                field.form.appendChild(hidden);
                field.addEventListener('change', () => hidden.value = field.value);
            }
        } else {
            field.readOnly = true;
        }

        field.classList.add('bg-light');
        field.classList.add('text-muted');
        field.setAttribute('data-auto-generated', 'true');
    });
});
