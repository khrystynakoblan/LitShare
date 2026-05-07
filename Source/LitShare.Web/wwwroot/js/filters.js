document.addEventListener('DOMContentLoaded', () => {

    const form = document.getElementById('filterForm');
    if (!form) return;

    const dealTypeInputs = document.querySelectorAll('input[name="dealType"]');
    const genreInputs = document.querySelectorAll('input[name="genres"]');
    const locationInput = document.querySelector('input[name="location"]');

    dealTypeInputs.forEach(radio => {
        radio.addEventListener('change', () => {
            form.submit();
        });
    });

    let timeout;

    genreInputs.forEach(cb => {
        cb.addEventListener('change', () => {
            clearTimeout(timeout);
            timeout = setTimeout(() => {
                form.submit();
            }, 300);
        });
    });

    if (locationInput) {
        locationInput.addEventListener('keydown', (e) => {
            if (e.key === 'Enter') {
                e.preventDefault();
                form.submit();
            }
        });
    }

});