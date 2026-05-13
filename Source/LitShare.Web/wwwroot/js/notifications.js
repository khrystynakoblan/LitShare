"use strict";

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/notificationHub")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveNotification", function (message) {
    const toastHtml = `
        <div class="toast" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="toast-header bg-primary text-white">
                <i class="bi bi-bell-fill me-2"></i>
                <strong class="me-auto">Сповіщення LitShare</strong>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
            <div class="toast-body">
                ${message}
            </div>
        </div>`;

    let toastContainer = document.getElementById('toastContainer');
    toastContainer.insertAdjacentHTML('beforeend', toastHtml);

    let toastElements = toastContainer.querySelectorAll('.toast');
    let latestToast = new bootstrap.Toast(toastElements[toastElements.length - 1]);
    latestToast.show();
});

connection.start().catch(err => console.error(err.toString()));