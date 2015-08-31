function logout() {
    console.log("logout");

    var token = sessionStorage.accessToken;
    var headers = {
        Authorization : 'Bearer ' + token        
    }
    var url = "http://localhost/api/Account/Logout";

    $.ajax({
        type: "POST",
        url: url,
        headers: headers,
        success: function () {
            console.log("Successfull");
            sessionStorage.removeItem("accessToken");
            sessionStorage.removeItem("userName");
        }
    });

}