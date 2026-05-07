document.addEventListener("DOMContentLoaded", function () {
    const btn = document.getElementById("autoFillBtn");
    if (!btn) return;

    btn.addEventListener("click", async function () {
        const title = document.getElementById("bookTitleInput")?.value?.trim();
        const msg = document.getElementById("apiMessage");

        if (!title) {
            msg.textContent = "Введіть назву книги";
            return;
        }

        btn.disabled = true;
        try {
            const res = await fetch(`/Post/AutoFillBookData?title=${encodeURIComponent(title)}`);
            const data = await res.json();
            if (data.author) document.getElementById("Author").value = data.author;
            if (data.description) document.getElementById("Description").value = data.description;
        } catch (e) {
            console.error("Помилка завантаження даних");
        }
        btn.disabled = false;
    });
});