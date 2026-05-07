function confirmApprove() {
    Swal.fire({
        title: 'Підтвердити скаргу?',
        text: "Оголошення буде видалено",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'Так',
        cancelButtonText: 'Ні'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('approveForm').submit();
        }
    });
}

function confirmReject() {
    Swal.fire({
        title: 'Відхилити скаргу?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Так',
        cancelButtonText: 'Ні'
    }).then((result) => {
        if (result.isConfirmed) {
            document.getElementById('rejectForm').submit();
        }
    });
}   