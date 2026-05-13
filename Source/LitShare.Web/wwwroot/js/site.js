document.addEventListener("DOMContentLoaded", function () {

    document.querySelectorAll('.alert:not(.alert-permanent)').forEach(alert => {
        setTimeout(() => {
            try {
                new bootstrap.Alert(alert).close();
            } catch (_) { }
        }, 5000);
    });

    const currentPath = window.location.pathname;
    document.querySelectorAll('.navbar-nav .nav-link').forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
        }
    });

    document.querySelectorAll('.needs-validation').forEach(form => {
        form.addEventListener('submit', event => {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
            }
            form.classList.add('was-validated');
        }, false);
    });

    if (typeof $.fn.select2 !== 'undefined') {
        $('.js-select2').select2({
            placeholder: 'Оберіть жанри...',
            allowClear: true,
            width: '100%'
        });
    }

    document.querySelectorAll('[data-confirm]').forEach(el => {
        el.addEventListener('click', function (e) {
            e.preventDefault();
            const message = this.dataset.confirm || 'Ви впевнені?';
            const form = this.closest('form');

            Swal.fire({
                icon: 'warning',
                title: 'Підтвердження',
                text: message,
                confirmButtonText: 'Так',
                cancelButtonText: 'Скасувати',
                showCancelButton: true,
                confirmButtonColor: '#dc3545',
                cancelButtonColor: '#6c757d',
                reverseButtons: true
            }).then(result => {
                if (result.isConfirmed) {
                    if (form) {
                        form.submit();
                    } else if (this.href) {
                        window.location.href = this.href;
                    }
                }
            });
        });
    });
});