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
