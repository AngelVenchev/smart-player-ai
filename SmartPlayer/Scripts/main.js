// call GetAll
$(document).ready(function () {
    attachEventListeners();
});

var baseUrl = "http://localhost";
localStorage.setItem("baseUrl", baseUrl);

function attachEventListeners() {
    $("#rateSongBtn").click("rateSong");

    var songs = getAllSongsNames();
    $('.typeahead').typeahead({
        hint: true,
        highlight: true,
        minLength: 1
    },
    {
        name: 'songs',
        source: substringMatcher(songs)
    });
};

var substringMatcher = function (strs) {
    return function findMatches(q, cb) {
        var matches, substringRegex;

        // an array that will be populated with substring matches
        matches = [];

        // regex used to determine if a string contains the substring `q`
        substrRegex = new RegExp(q, 'i');

        // iterate through the pool of strings and for any string that
        // contains the substring `q`, add it to the `matches` array
        $.each(strs, function (i, str) {
            if (substrRegex.test(str)) {
                matches.push(str);
            }
        });

        cb(matches);
    };
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

function getAllSongsNames() {
    var songsNames = [];
    var allSongs = getAllSongs();
    for (var song in allSongs) {
        songsNames.push(allSongs[song].SongName);
    }
    console.log("allSongsNames: ", songsNames);
    return songsNames;
}

// Call GetSong with selected song Id
function playSelestedSong() {
    var selectedSong = $(".typeahead.tt-input").val();
    console.log("selectedSong", selectedSong);
    
    var songId;
    var allSongs = getAllSongs();
    for (var song in allSongs) {
        if (song.SongName == selectedSong) {
            songId = song.Id;
        }
    }

    var url = "http://localhost/api/Music/GetSong";
    var data = {
        "songId": songId
    }
    $.ajax({
        type: "GET",
        url: url,
        data: data,        
        // onsuccess play song
        success: function (data) {
            console.log("Data: ", data);
            var songId = data.Id;
            var songName = data.Name;
            var src = data.Url;
            var audio = $("audio source");
            audio.attr("src", src);
            audio[0].play();
            sessionStorage.setItem('currentSong', { "songName": songName, "songId": songId });
            console.log("currentSong:", sessionStorage.currentSong);
        },
        success: function () {
            console.log("Eror: ");
        },
        dataType: "application/json"
    });
}


// on next button call next
function playNextSong() {
    var songId;
    var allSongs = getAllSongs();
    for (var song in allSongs) {
        if (song.SongName == selectedSong) {
            songId = song.Id;
        }
    }

    var url = "http://localhost/api/Music/GetNextSong";
    var data = {
        "songId": songId
    }
    $.ajax({
        type: "GET",
        url: url,
        data: data,
        success: function (data) {
            console.log("Data: ", data);
            var songId = data.Id;
            var songName = data.Name;
            var src = data.Url;
            var audio = $("audio source");
            audio.attr("src", src);
            audio[0].play();
        },
        success: function () {
            console.log("Eror: ");
        },
        dataType: "application/json"
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
    var token = sessionStorage.accessToken;
    var headers = {
        Authorization: 'Bearer ' + token
    }
    var url = "http://localhost/api/Music/Rate";
    var data = {
        "Rating": rating,
        "SongId": "2"
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
