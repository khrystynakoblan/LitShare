document.addEventListener("DOMContentLoaded", function () {

    const btn = document.getElementById("autoFillBtn");

    if (!btn) return;

    btn.addEventListener("click", async function () {

        const title = document.getElementById("bookTitleInput")?.value?.trim();
        const msg = document.getElementById("apiMessage");

        if (!title) {
            if (msg) {
                msg.textContent = "Введіть назву книги";
                msg.className = "text-danger small d-block";
            }
            return;
        }

        const old = btn.innerHTML;
        btn.disabled = true;
        btn.innerHTML = "Завантаження...";

        try {
            const res = await fetch(`/Post/AutoFillBookData?title=${encodeURIComponent(title)}`);

            if (!res.ok) throw new Error();

            const data = await res.json();

            if (data.author) document.getElementById("Author").value = data.author;
            if (data.description) document.getElementById("Description").value = data.description;

            if (msg) {
                msg.textContent = "Готово";
                msg.className = "text-success small d-block";
            }

        } catch {
            if (msg) {
                msg.textContent = "Помилка завантаження";
                msg.className = "text-danger small d-block";
            }
        }

        btn.innerHTML = old;
        btn.disabled = false;
    });

});