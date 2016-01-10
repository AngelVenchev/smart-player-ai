function logout() {
    sessionStorage.removeItem("accessToken");
    sessionStorage.removeItem("userName");
    sessionStorage.removeItem("currentSongId");
    sessionStorage.removeItem("currentSongName");
    $('body').removeClass("loggedUser");
}
