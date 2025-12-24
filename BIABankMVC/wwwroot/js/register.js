// Register page validation
$(document).ready(function() {
    // Get form elements
    var pwd = $('#' + 'password');
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
    pwd.on('input', function() {
        var password = $(this).val();
        
        if (password.length > 0) {
            requirements.show();
            // Check each requirement
            updateCheck($('#' + 'lengthCheck'), password.length >= 6, 'At least 6 characters');
            updateCheck($('#' + 'capitalCheck'), /[A-Z]/.test(password), 'At least 1 capital letter');
            updateCheck($('#' + 'specialCheck'), /[!@@#$%^&*(),.?"':{}|<>]/.test(password), 'At least 1 special character');
        } else {
            requirements.hide();
        }
        
        checkMatch();
    });

    // Check password match on confirm input
    confirmPwd.on('input', checkMatch);

    // Verify passwords match
    function checkMatch() {
        var password = pwd.val();
        var confirm = confirmPwd.val();
        var feedback = $('#' + 'confirmPasswordFeedback');
        
        if (confirm.length > 0) {
            updateCheck(feedback, password === confirm, password === confirm ? 'Passwords match' : 'Passwords do not match');
        } else {
            feedback.html('');
        }
    }

    // Calculate age from birth date
    function calculateAge(birthDate) {
        var today = new Date();
        var age = today.getFullYear() - birthDate.getFullYear();
        var m = today.getMonth() - birthDate.getMonth();
        // Adjust age if birthday hasn't occurred this year yet
        if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
            age--;
        }
        return age;
    }
    
    // Validate date of birth on change or blur
    $('#dateOfBirth').on('change blur', function() {
        var dateValue = $(this).val();
        if (!dateValue) return;
        
        var date = new Date(dateValue);
        var $error = $(this).siblings('.text-danger');
        
        // Check if year is before 1900
        if (date.getFullYear() < 1900) {
            $error.text('Please enter a valid date of birth');
            $(this).addClass('input-validation-error');
        }
        // Check if date is in the future
        else if (date > new Date()) {
            $error.text('Date of birth cannot be in the future');
            $(this).addClass('input-validation-error');
        }
        // Check if user is at least 16 years old
        else if (calculateAge(date) < 16) {
            $error.text('You must be at least 16 years old to register');
            $(this).addClass('input-validation-error');
        }
        // Clear error if valid
        else {
            $error.text('');
            $(this).removeClass('input-validation-error');
        }
    });
});
