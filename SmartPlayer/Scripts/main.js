$("#autocomplete").autocomplete({
    source: ["c++", "java", "php", "coldfusion", "javascript", "asp", "ruby", "python"]
});

function handleFiles(event) {
    console.log("Files:", event);

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