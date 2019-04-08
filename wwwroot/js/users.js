$(document).ready(function () {
    $('.centering > button').each(function () {
        var button = this;
        $(this).click(function () {
            $('.fade').attr('id', $(button).attr('data-target').substr(1));
            $('.fade form input').val($(button).attr('data-route-id'));
            $('.modal-body > h6').text($('.modal-body > h6').text() + $(this).closest('li').children('p:first-child').text() + '?');
        })
    })
})