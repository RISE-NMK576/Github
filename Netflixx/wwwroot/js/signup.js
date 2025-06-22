// Send OTP functionality
document.addEventListener('DOMContentLoaded', function () {
    const sendOtpBtn = document.getElementById('sendOtpBtn');
    if (sendOtpBtn) {
        sendOtpBtn.addEventListener('click', async function () {
            const email = document.getElementById('emailInput').value;
            if (!email) {
                alert('Vui lòng nhập email');
                return;
            }

            try {
                const response = await fetch('/Login/SendSignUpOtp', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({ email: email })
                });

                const result = await response.json();
                if (result.success) {
                    alert('Mã OTP đã được gửi!');
                } else {
                    alert(result.message || 'Lỗi gửi OTP');
                }
            } catch (error) {
                console.error('Error:', error);
                alert('Đã xảy ra lỗi khi gửi OTP');
            }
        });
    }

    // Password validation
    const passwordInput = document.getElementById('passwordInput');
    if (passwordInput) {
        passwordInput.addEventListener('input', function () {
            const password = this.value;
            const requirements = {
                length: password.length >= 8,
                uppercase: /[A-Z]/.test(password),
                number: /[0-9]/.test(password),
                special: /[^A-Za-z0-9]/.test(password)
            };

            updateRequirement('lengthRequirement', requirements.length);
            updateRequirement('uppercaseRequirement', requirements.uppercase);
            updateRequirement('numberRequirement', requirements.number);
            updateRequirement('specialRequirement', requirements.special);
        });

        // Trigger validation on page load
        passwordInput.dispatchEvent(new Event('input'));
    }

    function updateRequirement(elementId, isValid) {
        const element = document.getElementById(elementId);
        if (element) {
            element.classList.remove('requirement-valid', 'requirement-invalid');
            element.classList.add(isValid ? 'requirement-valid' : 'requirement-invalid');
        }
    }
});