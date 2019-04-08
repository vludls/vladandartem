
$(document).ready(function () {
    //Скрыть кнопку оплатить в корзине
    $(document).ready(function () {
        if ($('h1').is('.empty')) {
            $('.continue').hide();
        }
    })
    //Скрыть кнопку оплатить в корзине

    //Цена в корзине
    var p = [];
    for (var i = 0; i < $('.list ul').children('li').length; i++) {
        p[i] = $('.list ul li:nth-child(' + (i + 1) + ') .price-in-cart span').text(); //Массив цен всех продуктов в корзине
    }
    var s = 0;
    $('.price-in-cart > span').each(function () {
        $(this).html($(this).html() * $(this).parent().prev().children('input').val());
        s += (+$(this).html());
    });
    
    $('.total-price').html(s);

    

    $('.q').each(function () {
        
        s = 0;
        $(this).on('input', function () {
            s = 0;

            $(this).parent().next().children().html($(this).val() * p[$(this).closest('li').index()]);
            $('.price-in-cart > span').each(function () {
                s += (+$(this).html());
            });
            $('.total-price').html(s)
        });

        //ajax
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
    //Цена в корзине
});
