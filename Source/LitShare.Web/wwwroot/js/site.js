document.addEventListener("DOMContentLoaded", function () {

    const radios = document.querySelectorAll('input[type="radio"]');
    const checkboxes = document.querySelectorAll('input[type="checkbox"]');
    const filterForm = document.getElementById("filterForm");

    if (radios && filterForm) {
        radios.forEach(radio => {
            radio.addEventListener("change", function () {
                filterForm.submit();
            });
        });
    }

    if (checkboxes && filterForm) {
        let timeout;

        checkboxes.forEach(cb => {
            cb.addEventListener("change", function () {
                clearTimeout(timeout);
                timeout = setTimeout(() => {
                    filterForm.submit();
                }, 400);
            });
        });
    }

});