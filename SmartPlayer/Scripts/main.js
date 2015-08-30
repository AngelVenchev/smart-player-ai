$("#autocomplete").autocomplete({
    source: ["c++", "java", "php", "coldfusion", "javascript", "asp", "ruby", "python"]
});

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

function likeCurrentSong() {
    console.log("LikeCurrentSong");
    var currentSong = getCurrentSongName();
    like(currentSong);
}

function dislikeCurrentSong() {
    console.log("DisLikeCurrentSong");
    var currentSong = getCurrentSongName();
    dislike(currentSong);
}

function getCurrentSongName() {
    return Song;
}

function like(song) {
    console.log("Like");
}

function dislike(song) {
    console.log("Dislike");
}


function togglePlayPause() {
    console.log("TogglePlayPause");
}