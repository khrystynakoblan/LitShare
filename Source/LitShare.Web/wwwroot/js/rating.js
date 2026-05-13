document.addEventListener("DOMContentLoaded", function () {
    const stars = document.querySelectorAll('.star-btn');
    const input = document.getElementById('ratingInput');

    if (stars.length > 0 && input) {
        stars.forEach(star => {
            star.addEventListener('click', function() {
                const val = this.dataset.value;
                input.value = val;
                updateStars(val);
            });
        });
    }

    function updateStars(val) {
        stars.forEach((s, i) => {
            s.classList.toggle('bi-star-fill', i < val);
            s.classList.toggle('bi-star', i >= val);
        });
    }
});