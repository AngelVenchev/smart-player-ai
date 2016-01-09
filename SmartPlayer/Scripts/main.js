// call GetAll
$(document).ready(function () {
    attachEventListeners();
});

var baseUrl = "http://localhost";
localStorage.setItem("baseUrl", baseUrl);

function attachEventListeners() {

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
