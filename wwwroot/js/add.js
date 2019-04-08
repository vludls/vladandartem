$(document).ready(function () {
    alert('!');
    $('#aimg').on('change', function () {
        $('#add-img-path').val($('#aimg').val());
    });
});