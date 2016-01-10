function logout() {
    sessionStorage.removeItem("accessToken");
    sessionStorage.removeItem("userName");
    $('body').removeClass("loggedUser");
}
