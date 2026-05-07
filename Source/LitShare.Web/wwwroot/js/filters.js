document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('filterForm');
    if (!form) return;

    const inputs = form.querySelectorAll('input[type="radio"], input[type="checkbox"]');
    inputs.forEach(input => {
        input.addEventListener('change', () => form.submit());
    });
});