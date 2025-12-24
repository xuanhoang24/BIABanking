// Email verification countdown
let seconds = 30;
const countdownElement = document.getElementById('countdown');

const timer = setInterval(() => {
    seconds--;
    countdownElement.textContent = seconds;
    
    if (seconds <= 0) {
        clearInterval(timer);
        window.close();
        
        // If window.close() doesn't work (some browsers block it), redirect to login
        setTimeout(() => {
            window.location.href = '/Auth/Login';
        }, 500);
    }
}, 1000);
