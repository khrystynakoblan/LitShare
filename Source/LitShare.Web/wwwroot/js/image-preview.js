document.addEventListener("DOMContentLoaded", function () {

    const input = document.getElementById("imageInput");

    if (!input) return;

    input.addEventListener("change", function () {
        const file = this.files[0];
        if (!file) return;

        const preview = document.getElementById("imagePreview");
        const placeholder = document.getElementById("placeholderContent");

        if (preview) {
            preview.src = URL.createObjectURL(file);
            preview.style.display = "block";
        }

        if (placeholder) {
            placeholder.style.display = "none";
        }
    });

});