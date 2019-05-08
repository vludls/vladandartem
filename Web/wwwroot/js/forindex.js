$(document).ready(function () {
    //Скроллинг при поиске и переходе по ссылкам пагинации
    var addressBar = window.location.search;
    if (addressBar.length > 19 || addressBar.substr(1, 5) == 'Page=') {
        document.documentElement.scrollTop = '600';
    };
    //Скроллинг при поиске и переходе по ссылкам пагинации

    //slider
    var width = $('.mainSlider').width();

    $('.slides>li:last-child').prependTo('.slides');
    function nextslide() {
        $('.slides').animate({
            'margin-left': -width
        }, 500, function () {
            $('.slides>li:first-child').appendTo('.slides');
            $('.slides').css('margin-left', 0);
        });
    }

    function prevslide() {
        $('.slides').animate({
            'margin-left': width
        }, 500, function () {
            $('.slides>li:last-child').prependTo('.slides');
            $('.slides').css('margin-left', 0)
        });
    }

    $('.right').click(nextslide);
    $('.left').click(prevslide);
    function timer() {
        var timerId = setInterval(nextslide, 3000);
        $('.right').click(function () {
            clearInterval(timerId);
            timerId = setInterval(nextslide, 3000);
        });
        $('.left').click(function () {
            clearInterval(timerId);
            timerId = setInterval(nextslide, 3000);
        });
    };
    timer();
    //slider

    $('.delete-products').submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/Home/RemoveProduct",
            type: "POST",
            data: form.serialize(),
            success: function () {
                $(form).closest('li').remove();
            }
        });
        e.preventDefault();
    });
});
