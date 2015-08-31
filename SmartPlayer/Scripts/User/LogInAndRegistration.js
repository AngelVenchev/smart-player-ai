function register() {
    console.log("Register");

    var email = $('.registrationForm .emailInput').val();
    var password = $('.registrationForm .passwordInput').val();
    var confirmPassword = $('.registrationForm .confirmPasswordInput').val();
    var data = {
        "Email": email,
        "Password": password,
        "ConfirmPassword": confirmPassword
    }
    var url = "http://localhost/api/Account/Register";

    $.ajax({
        type: "POST",
        url: url,
        data: data,
        success: function () {
            console.log("Successfull Registration")
        },
        dataType: "application/json"
    });
}


function getToken() {
    console.log("Get Token");
    var email = $('.logInForm .emailInput').val();
    var password = $('.logInForm .passwordInput').val();
    var data = {
        "Username": email,
        "Password": password,
        "grant_type": "password"
    }
    var url = "http://localhost/token";
    $.ajax({
        type: "POST",
        ContentType: "application/x-www-form-urlencoded",
        url: url,
        data: data,
        Accept: "application/json",
        success: function (data) {
            console.log("Data:", data);
            sessionStorage.accessToken = data.access_token;
            sessionStorage.userName = data.userName;
            $('body').addClass("loggedUser");
            $('.accountInfo .userName').text(sessionStorage.userName);
        }
    });
}