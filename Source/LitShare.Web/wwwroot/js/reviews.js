document.addEventListener("DOMContentLoaded", function () {

    const stars = document.querySelectorAll(".star-btn");
    const input = document.getElementById("ratingInput");

    if (!stars.length || !input) return;

    function setStars(value) {
        stars.forEach((s, i) => {
            s.classList.toggle("bi-star-fill", i < value);
            s.classList.toggle("bi-star", i >= value);
        });
    }

    stars.forEach(star => {

        star.addEventListener("click", function () {
            const value = parseInt(this.dataset.value);
            input.value = value;
            setStars(value);
        });

        star.addEventListener("mouseover", function () {
            setStars(parseInt(this.dataset.value));
        });

        star.addEventListener("mouseout", function () {
            setStars(parseInt(input.value || 0));
        });

    });

});