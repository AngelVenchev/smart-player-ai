$(document).ready(function () {
    attachEventListeners();
    isUserLoggedIn();    
    sessionStorage.setItem("playedSongs", JSON.stringify([]));
});

var baseUrl = "http://localhost";

function attachEventListeners() {
    $("#rateSongBtn").click(rateSong);
    $("#nextSongButton").click(playNextSong);
    $("audio").on("ended", playNextSong);

    configureTypeahead();
};

function configureTypeahead() {
    var songs = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: baseUrl + "/api/Music/SearchSong",
            replace: function (url, query) {
                return url + "?s=" + query;
            },
            filter: function (x) {
                return $.map(x, function (item) {
                    return {
                        id: item.Id,
                        value: item.SongName
                    };
                });
            },
            wildcard: '%QUERY%'
        }
    });

    songs.initialize();

    $('.typeahead').typeahead({
        hints: false
    }, {
        limit: 20,
        name: "songs",
        displayKey: "value",
        minLength:1,
        source: songs.ttAdapter()
    });

    $('.typeahead').bind('typeahead:select', function (ev, suggestion) {
        playSelectedSong(suggestion.id);
    });
}

function getAuthHeaders() {
    var token = sessionStorage.getItem('accessToken');
    if(token) {
        var headers = {
            Authorization: 'Bearer ' + token
        }
    }
    return headers;
}

function isUserLoggedIn() {
    var url = baseUrl + "/api/Account/IsUserAuthenticated";
    $.ajax({
        type: "GET",
        url: url,
        headers: getAuthHeaders(),
        success: function (userIsAuthenticated) {
            if (userIsAuthenticated)
            {
                $('body').addClass("loggedUser");
                $('.accountInfo .userName').text(sessionStorage.userName);
            } else {
                logout();
            }
        },
        error: function (err) {
            console.log("Error!" + err);
        }
    });
}

// Call GetSong with selected song Id
function playSelectedSong(songId) {
    var url = baseUrl + "/api/Music/GetSong/?songId=" + songId;
    $.ajax({
        type: "GET",
        url: url,
        headers: getAuthHeaders(),
        success: playSong,
        error: function (err) {
            console.log("Eror: ");
        }
    });
}

function playSong(playerSong) {
    $('.typeahead').typeahead('val', '');
    var audioSource = $("audio source");
    audioSource.attr("src", playerSong.Url);

    var player = $("audio");
    player[0].load();
    player[0].play();

    $('p.songName').text(playerSong.Name);

    sessionStorage.setItem('currentSongId', playerSong.Id);
    sessionStorage.setItem('currentSongName', playerSong.Name);
    if (playerSong.CurrentUserVote) {
        $('#ratingDropDown').val(playerSong.CurrentUserVote);
    }
    var playedSongs = JSON.parse(sessionStorage.getItem("playedSongs"));
    playedSongs.push(playerSong.Id);
    sessionStorage.setItem("playedSongs", JSON.stringify(playedSongs));
}

function playNextSong() {
    var url =  baseUrl + "/api/Music/Next";
    var currentSongId = sessionStorage.getItem("currentSongId");
    var playedSongs = JSON.parse(sessionStorage.getItem("playedSongs"))
    var data = {
        PlayedSongIds: playedSongs, // get from local storage
        CurrentSongId: currentSongId
    };
    $.ajax({
        type: "POST",
        url: url,
        headers: getAuthHeaders(),
        data: data,
        success: playSong,
        error: function (e) {
            console.log("Error: ");
        }
    });
}

function handleFiles(files) {
    var file = files[0];
    if (file.type != "audio/mp3") {
        console.log("Invalid data type");
        $('.uploadFiles .errorMessage').css('display', 'block');
        $('.uploadFiles .errorMessage').text("The type of the file that you are trying to upload is invalid. Please select mp3 file!")
        $('#ajaxUploadButton').prop('disabled', true);
    } else {
        $('.uploadFiles .errorMessage').css('display', 'none');
        $('#ajaxUploadButton').prop('disabled', false);
        
        $("#fileUploadForm").ajaxForm({
            success: function () {
                $('.progress').addClass("hidden");
                $('.done').removeClass("hidden");
                $(".done").fadeOut(1500, function () {
                    $('.done').addClass("hidden");
                });
            }
        });
        $("#fileUploadForm").submit();
        $('.progress').removeClass("hidden");
    }
}

function rateSong() {
    console.log("rateSong");
    var rating = $("#ratingDropDown").val();
    var url = baseUrl + "/api/Music/Rate";

    var data = {
        Rating: rating,
        SongId: sessionStorage.getItem('currentSongId')
    }

    $.ajax({
        type: "POST",
        url: url,
        headers: getAuthHeaders(),
        data: data,
        success: function () {
            console.log("Successfull Rating");
        }
    });
}
