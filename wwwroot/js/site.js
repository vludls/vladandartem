$(window).scroll(function () {

    if (document.documentElement.scrollTop > 105) {
        $('.fix').removeClass('begin');
        $('.fix').addClass('end');
    } else {

        $('.fix').addClass('begin');
        $('.fix').removeClass('end');
    }
});

var price = $('.price-in-cart>span').html();
var p = [];
for (var i = 0; i < $('.list ul').children('li').length; i++) {

    p[i] = $('.list ul li:nth-child(' + (i + 1) + ') .price-in-cart span').html();


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
                    id: "" + $(this).parent().parent().children().first().val(),
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

    $('.price-in-cart > span').each(function () {
        $(this).html($(this).html() * $(this).parent().prev().children('input').val())
    });
    $('.price-in-cart > span').each(function () {
        s += (+$(this).html());
    });
    $('.total-price').html(s);
    $('.q').each(function () {
        s = 0;
        $(this).on('input', function () {
            s = 0;
            $('.price-in-cart > span').each(function () {
                s += (+$(this).html());
            });
            $('.total-price').html(s)
        })
    })
});
//Цена в корзине

$(document).ready(function () {
    $('#aimg').on('change', function () {
        $('#add-img-path').val($('#aimg').val());
    });
});

//Цена в оплаченных
$(document).ready(function () {
    $('.unit-price').each(function () {
        $(this).html($(this).html() * $(this).parent().children('p').children('.units').html());

    })
})
//Цена в оплаченных

//выкатывание продуктов
$(document).ready(function () {
    $('.order > div:not(:first-child)').hide();
    $('.open-order').each(function () {
        $(this).on('click', function () {
            $(this).children().toggleClass('fa-chevron-circle-down');
            $(this).children().toggleClass('fa-chevron-circle-up');
            $(this).parent().next().slideToggle();
        })
    })
})
//выкатывание продуктов

//Скрыть кнопку оплатить в корзине
$(document).ready(function () {
    if ($('h1').is('.empty')) {
        $('.continue').hide();
    }
})
//Скрыть кнопку оплатить в корзине

//Подсвечивать незаполненные поля красным
$(document).ready(function () {
    $('.regist span').each(function () {
        if ($(this).html() != "") {
            $(this).css('color', 'red');
            $(this).prev().css('border-color', 'red');

        }
    })
})

/*$(document).ready(function () {
    $('.delete-user').click(function () {
        $(this).attr('data-target', $(this).parent().prev().children().attr('data-route-id'));
        alert($(this).attr('data-target'));
    })
})*/

/*$(document).ready(function () {
    var id;
    $('.delete-user').each(function () {
        $(this).click(function () {
            id = 'a' + $(this).parent().prev().children().attr('data-route-id');
            $(this).attr('data-target', '#' + id);
            $(this).parent().parent().next().attr('id', id);
        });
    })
})*/

$('.datepicker').datepicker({
    language: "ru",
    format: "dd.mm.yyyy"
});
