//Фиксировать навигацию при скроллинге
$(window).scroll(function () {

    if (document.documentElement.scrollTop > 105) {
        $('.fix').removeClass('begin');
        $('.fix').addClass('end');
    } else {

        $('.fix').addClass('begin');
        $('.fix').removeClass('end');
    }
});
//Фиксировать навигацию при скроллинге

$('.datepicker').datepicker({
    language: "ru",
    format: "dd.mm.yyyy"
});
function forAddCategory () {
    var button = this;
    $(this).click(function () {
        $('.fade').attr('id', $(button).attr('data-target').substr(1));
        $('.fade form input').val($(button).attr('data-route-id'));
        $('.modal-body > h6').text('Удалить категорию ' + $(this).closest('li').children('p:first-child').text() + '?');
    });
}

