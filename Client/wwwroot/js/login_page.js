document.addEventListener("DOMContentLoaded", async function () {
    var user = await getUser();
    if (user) {
        console.log(user.role)
        if (user.role == 'User') {
            window.location.href = "/Chat/HomeChat"
        } else {
            window.location.href = "/Admin/Index"
        }
        
    }

    var btnLoginGoogle = document.querySelector("#login-google");
    btnLoginGoogle.addEventListener("click", function () {
        window.location.href = `${BASE_URL}api/User/signin-google?returnUrl=https://localhost:7005?login-google=true`;
    });
    const urlParams = new URLSearchParams(window.location.search);
    const loginGoogle = urlParams.get('login-google');

    if (loginGoogle === 'true') {
        var user = await getUser();
        if (user) {
            console.log(user.role)
            if (user.role == 'User') {
                window.location.href = "/Chat/HomeChat"
            } else {
                window.location.href = "/Admin/Index"
            }
        }
    }
    document.getElementById('loginLink').addEventListener('click', async function (event) {
        event.preventDefault();

        const username = document.getElementById('username').value;
        const password = document.getElementById('password').value;

        const loginData = {
            username: username,
            password: password
        };

        try {
            const response = await fetch(BASE_URL + 'api/user/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(loginData)
            });

            const errorMessageElement = document.getElementById('error-message');

            if (response.ok) {
                const data = await response.json();

                if (data.status) {
                    var date = new Date();
                    date.setTime(date.getTime() + (24 * 60 * 60 * 1000));
                    var expires = "expires=" + date.toUTCString();
                    document.cookie = 'bearer_token=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;';
                    document.cookie = `bearer_token=${data.message}; ${expires}; path=/; secure; samesite=strict`;
                    errorMessageElement.textContent = "";
                    var user = await getUser();
                    if (user) {
                        console.log(user.role)
                        if (user.role == 'User') {
                            window.location.href = "/Chat/HomeChat"
                        } else {
                            window.location.href = "/Admin/Index"
                        }
                    }
                } else {
                    console.log("Error");
                    errorMessageElement.textContent = data.message;
                }
            } else {
                console.error('Login failed:', response.statusText);
                errorMessageElement.textContent = "Login failed: " + response.statusText;
            }
        } catch (error) {
            console.error('Error:', error);
            document.getElementById('error-message').textContent = "Network error. Please try again later.";
        }
    });
   

});