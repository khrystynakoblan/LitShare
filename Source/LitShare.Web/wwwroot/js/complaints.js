function confirmApprove() {
    if (confirm("Ви впевнені, що хочете ПІДТВЕРДИТИ скаргу? Оголошення буде видалено з сайту.")) {
        const form = document.getElementById('approveForm');
        if (form) form.submit();
    }
}

function confirmReject() {
    if (confirm("Ви впевнені, що хочете ВІДХИЛИТИ цю скаргу?")) {
        const form = document.getElementById('rejectForm');
        if (form) form.submit();
    }
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