document.addEventListener("DOMContentLoaded", function () {

    const input = document.getElementById("imageInput");
    const preview = document.getElementById("imagePreview");
    const container = document.getElementById("imagePreviewContainer");
    const placeholder = document.getElementById("placeholderContent");
    const btn = document.getElementById("selectImageBtn");

    if (btn && input) {
        btn.addEventListener("click", () => input.click());
    }

    if (input) {
        input.addEventListener("change", function () {
            const file = this.files[0];
            if (file) {
                const reader = new FileReader();
                reader.onload = function (e) {
                    preview.src = e.target.result;
                    preview.style.display = "block";
                    placeholder.style.display = "none";
                };
                reader.readAsDataURL(file);
            }
        });
    }

    const autoFillBtn = document.getElementById("autoFillBtn");
    const titleInput = document.getElementById("bookTitleInput");

    if (autoFillBtn && titleInput) {
        autoFillBtn.addEventListener("click", function () {
            titleInput.value = "Example Book";
        });
    }
    if (window.jQuery) {
        $('.js-select2').select2();
    }

});