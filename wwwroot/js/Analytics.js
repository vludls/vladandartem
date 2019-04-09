$(document).ready(function () {
    let date = new Date()
    let offset = -date.getTimezoneOffset() / 60;

    $("form input[type='hidden']").val(offset);
});