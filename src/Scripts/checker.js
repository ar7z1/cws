var levelCompleted = function() {
    $('#task').hide();
    $('#nextLevel').show();
};

var originalAlert = window.alert;
var isAlerted = false;
var isDocumentReady = false;

window.alert = function(s) {
    if (isDocumentReady) {
        levelCompleted();
    }
    else {
        isAlerted = true;
    }
    originalAlert(s);
};

$(function(){
    if (isAlerted) {
        levelCompleted();
    }
    isDocumentReady = true;
});