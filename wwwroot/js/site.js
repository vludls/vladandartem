$(window).scroll(function () {

    if (document.documentElement.scrollTop > 105) {
        $('.fix').removeClass('begin');
        $('.fix').addClass('end');
    } else {

        $('.fix').addClass('begin');
        $('.fix').removeClass('end');
    }
});

var price = $('.priceincart>span').html();
var p = [];
for (var i = 0; i < $('.list ul').children('li').length; i++) {

    p[i] = $('.list ul li:nth-child(' + (i + 1) + ') .priceincart span').html();


}
$('.q').each(function () {
    $('.q').on('input', function () {

        $(this).parent().next().children().html($(this).val() * p[$(this).closest('li').index()]);
    });
}
);
var a = window.location.search;
function scr() {

    document.documentElement.scrollTop = '600';
}
if (a.length > 19) {
    scr();
    //$('#sear').val(a.substr(19));
}
if (a.substr(1, 5) == 'Page=') {
    scr();
};

//Предпросмотр изображения
function readURL(input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('.addimg').attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);
    }
}

$(".file").change(function () {
    readURL(this);
});
//Предпросмотр изображения

//slider
$(document).ready(function () {


    var width = $('#mainSlider').width();

    $('.slides>li:last-child').prependTo('.slides');
    function nextslide() {
        $('.slides').animate({
            'margin-left': -width
        }, 500, function () {
            $('.slides>li:first-child').appendTo('.slides');
            $('.slides').css('margin-left', 0)
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
    //window.onload = timer();
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
});
//slider

$(document).ready(function () {
    $('.q').each(function () {
        var q = this;
        $(this).on('change', function () {
            $.ajax({
                url: "/Home/CartChangeProductNum",
                type: "POST",
                data: ({
                    id: $(this).parent().parent().children().first().val(),
                    count: $(this).val()
                }),
                success: function (data) {
                    $(q).next().next().next().html(data);

                }
            });
        });
    });
});

//Цена в корзине
$(document).ready(function () {
    var s = 0;

    $('.priceincart > span').each(function () {
        $(this).html($(this).html() * $(this).parent().prev().children('input').val())
    });
    $('.priceincart > span').each(function () {
        s += (+$(this).html());
    });
    $('.total-price').html(s);
    $('.q').each(function () {
        s = 0;
        $(this).on('input', function () {
            s = 0;
            $('.priceincart > span').each(function () {
                s += (+$(this).html());
            });
            $('.total-price').html(s)
        })
    })
});
//Цена в корзине

//Цена в оплаченных
$(document).ready(function () {
    $('.list > li > p > span').each(function () {
        $(this).html($(this).html() * $(this).parent().prev().children('h3').children().html());

    })
})
//Цена в оплаченных





