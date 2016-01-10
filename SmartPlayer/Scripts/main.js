// call GetAll
$(document).ready(function () {
    attachEventListeners();
});

var baseUrl = "http://localhost";
localStorage.setItem("baseUrl", baseUrl);

function attachEventListeners() {
    $("#rateSongBtn").click(rateSong);
    $("#nextSongButton").click(playNextSong);

    var songs = new Bloodhound({
        datumTokenizer: Bloodhound.tokenizers.obj.whitespace('value'),
        queryTokenizer: Bloodhound.tokenizers.whitespace,
        remote: {
            url: baseUrl + "/api/Music/SearchSong?s=%QUERY%",
            filter: function (x) {
                return $.map(x, function (item) {
                    return { id: item.Id, value: item.SongName };
                });
            },
            wildcard: '%QUERY%'
        }
    });

    $('.typeahead').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    }, {
        display: "value",
        source: songs
    });

};

function getAllSongs() {
    console.log("GetAllSongs");

    var allSongs = {};
    var url = baseUrl + "/api/Music/GetAll";
    $.ajax({
        type: "GET",
        async: false,
        url: url,
        success: function (data) {
            console.log("Data: ", data);
            allSongs = data;
            console.log("allSongs: ", allSongs);            
        },
        error: function () {
            console.log("Error!");
        }
    });

    return allSongs;
}

$('.typeahead').bind('typeahead:select', function (ev, suggestion) {
    playSelectedSong(suggestion.id);
    $('.typeahead.tt-input').val('');

});

// Call GetSong with selected song Id
function playSelectedSong(songId) {
    var url = "http://localhost/api/Music/GetSong/?songId=" + songId;
    $.ajax({
        type: "GET",
        url: url,
        success: playSong,
        error: function (err) {
            console.log("Eror: ");
        }
    });
}

function playSong(playerSong) {
    var audioSource = $("audio source");
    audioSource.attr("src", playerSong.Url);

    var player = $("audio");
    player[0].load();
    player[0].play();

    $('p.songName').text(playerSong.Name);

    player.on("ended", playNextSong);

    sessionStorage.setItem('currentSongId', playerSong.Id);
    sessionStorage.setItem('currentSongName', playerSong.Name);
}

function playNextSong() {
    var currentSongId = sessionStorage.getItem("currentSongId");
    var url = "http://localhost/api/Music/Next";
    var data = {
        PlayedSongIds: [], // get from local storage
        CurrentSongId: currentSongId
    };
    $.ajax({
        type: "POST",
        url: url,
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
        }
    }

    function rateSong() {
        console.log("rateSong");

        var rating = $("#rationgDropDown").val();
        var token = sessionStorage.getItem('accessToken');
        var headers = {
            Authorization: 'Bearer ' + token
        }
        var url = "http://localhost/api/Music/Rate";
        var data = {
            Rating: rating,
            SongId: sessionStorage.getItem('currentSongId')
        }

        $.ajax({
            type: "POST",
            url: url,
            headers: headers,
            data: data,
            success: function () {
                console.log("Successfull Rating");
            }
        });
    }
