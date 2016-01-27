$('.logInForm .logInBtn').click(function() {
    var email = $('.logInForm .emailInput').val();
    var password = $('.logInForm .passwordInput').val();
    getToken(email, password);
});


function register() {
    var email = $('.registrationForm .emailInput').val();
    var password = $('.registrationForm .passwordInput').val();
    var confirmPassword = $('.registrationForm .confirmPasswordInput').val();
    var data = {
        Email: email,
        Password: password,
        ConfirmPassword: confirmPassword
    }
    var url = "http://localhost/api/Account/Register";

    $.ajax({
        type: "POST",
        url: url,
        data: JSON.stringify(data),
        contentType: 'application/json; charset=utf-8',
        success: function (data) {
            getToken(email, password);
        },
        dataType: "application/json"
    });
}


function getToken(email, password) {


    var data = {
        Username: email,
        Password: password,
        grant_type: "password"
    }
    var url = "http://localhost/token";
    $.ajax({
        type: "POST",
        ContentType: "application/x-www-form-urlencoded",
        url: url,
        data: data,
        Accept: "application/json",
        success: function (data) {
            sessionStorage.accessToken = data.access_token;
            sessionStorage.userName = data.userName;
            window.location.href = 'http://localhost';
        }
    });
}