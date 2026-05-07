function confirmApprove() {
    Swal.fire({
        icon: 'warning',
        title: 'Підтвердити скаргу?',
        text: 'Оголошення буде видалено з сайту. Цю дію неможливо скасувати.',
        confirmButtonText: 'Так, видалити',
        cancelButtonText: 'Скасувати',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        cancelButtonColor: '#6c757d',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('approveForm').submit();
        }
    });
}

function confirmReject() {
    Swal.fire({
        icon: 'question',
        title: 'Відхилити скаргу?',
        text: 'Скаргу буде відхилено без жодних наслідків.',
        confirmButtonText: 'Так, відхилити',
        cancelButtonText: 'Скасувати',
        showCancelButton: true,
        confirmButtonColor: '#111',
        cancelButtonColor: '#6c757d',
        reverseButtons: true
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('rejectForm').submit();
        }
    });
}

document.addEventListener("DOMContentLoaded", function () {
    const textParams = document.getElementById("AdditionalText");
    const radios = document.querySelectorAll(".complaint-radio");

    if (textParams && radios.length > 0) {
        const toggleTextArea = () => {
            const selected = document.querySelector(".complaint-radio:checked");
            if (selected && selected.value === "Інше") {
                textParams.disabled = false;
                textParams.placeholder = "Будь ласка, опишіть проблему детально...";
            } else {
                textParams.disabled = true;
                textParams.value = "";
                textParams.placeholder = "Опишіть проблему (доступно при виборі 'Інше')";
            }
        };

        radios.forEach(r => r.addEventListener("change", toggleTextArea));
        toggleTextArea();
    }
});