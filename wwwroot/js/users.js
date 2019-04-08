$(document).ready(function () {
    $('.centering > button').each(function () {
        $(this).click(function () {
            $('.modal fade').attr('id', $(this).attr('data-target').substr(1));
        })
    })
})