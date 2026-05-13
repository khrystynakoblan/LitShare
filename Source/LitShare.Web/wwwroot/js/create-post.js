document.addEventListener("DOMContentLoaded", function () {
    const input = document.getElementById("imageInput");
    const preview = document.getElementById("imagePreview");
    const placeholder = document.getElementById("placeholderContent"); 

    if (input && preview) {
        input.addEventListener("change", function () {
            const file = this.files[0];

            if (file) {
                preview.src = URL.createObjectURL(file);

                preview.classList.remove("d-none");

                if (placeholder) {
                    placeholder.classList.add("d-none");
                }
            }
        });
    }
});