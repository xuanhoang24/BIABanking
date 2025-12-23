// Timezone detection
(function () {
    const tz = Intl.DateTimeFormat().resolvedOptions().timeZone;
    if (tz) {
        document.cookie = "timezone=" + tz + "; path=/";
    }
})();

// Real-time updates with SignalR
const apiUrl = window.location.hostname === 'localhost'
    ? 'http://localhost:5000'
    : window.location.origin;
const connection = new signalR.HubConnectionBuilder()
    .withUrl(apiUrl + "/notificationHub")
    .configureLogging(signalR.LogLevel.None)
    .build();

// Prevent reload during form submission or navigation
let isNavigating = false;

document.addEventListener('submit', function() {
    isNavigating = true;
});

window.addEventListener('beforeunload', function() {
    isNavigating = true;
});

connection.on("ReceiveNotification", () => {
    if (!isNavigating) {
        window.location.reload();
    }
});

connection.start()
    .catch(err => console.error("SignalR error:", err));
