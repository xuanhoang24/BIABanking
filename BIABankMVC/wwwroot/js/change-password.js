// Change password validation
$(document).ready(function() {
    // Get form elements
    var newPwd = $('#' + 'newPassword');
    var confirmPwd = $('#' + 'confirmPassword');
    var requirements = $('#' + 'passwordRequirements');

    // Update validation check display (icon and color)
    function updateCheck(element, isValid, text) {
        var icon = isValid ? 'bi-check-circle' : 'bi-x-circle';
        var color = isValid ? 'text-success' : 'text-danger';
        element.html('<i class="bi ' + icon + '"></i> ' + text)
            .removeClass('text-success text-danger')
            .addClass(color);
    }

    // Validate password requirements on input
    newPwd.on('input', function() {
        var pwd = $(this).val();
        
        if (pwd.length > 0) {
            requirements.show();
            // Check each requirement
            updateCheck($('#' + 'lengthCheck'), pwd.length >= 6, 'At least 6 characters');
            updateCheck($('#' + 'capitalCheck'), /[A-Z]/.test(pwd), 'At least 1 capital letter');
            updateCheck($('#' + 'specialCheck'), /[!@@#$%^&*(),.?"':{}|<>]/.test(pwd), 'At least 1 special character');
        } else {
            requirements.hide();
        }
        
        checkMatch();
    });

    // Check password match on confirm input
    confirmPwd.on('input', checkMatch);

    // Verify passwords match
    function checkMatch() {
        var pwd = newPwd.val();
        var confirm = confirmPwd.val();
        var feedback = $('#' + 'confirmPasswordFeedback');
        
        if (confirm.length > 0) {
            updateCheck(feedback, pwd === confirm, pwd === confirm ? 'Passwords match' : 'Passwords do not match');
        } else {
            feedback.html('');
        }
    }
});
