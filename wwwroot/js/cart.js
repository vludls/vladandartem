
$(document).ready(function () {
    //Скрыть кнопку оплатить в корзине
    $(document).ready(function () {
        if ($('h1').is('.empty')) {
            $('.continue').hide();
        }
    })
    //Скрыть кнопку оплатить в корзине

    //Цена в корзине
    var s = 0;
    function sum() {
        //$(this).parent().next().children().text($(this).val() * $(this).prev().val());
        $('#price' + $(this).attr('data-id')).text($(this).val() * $(this).prev().val());
        s += $(this).val() * $(this).prev().val();
        $('.total-price').text(s);
    };

    $('.q').each(sum);
    $('.q').on('input', function(){
        s = 0;
        $('.q').each(sum);
    });
    //Цена в корзине

    $('.q').each(function () {
        var q = this;
        $(this).on('change', function () {
            $.ajax({
                url: "/Home/CartChangeProductNum",
                type: "POST",
                data: ({
                    //id: "" + $(this).parent().parent().children().first().val(),
                    id: "" + $('#hidden' + $(q).attr('data-id')).val(),
                    count: $(this).val()
                }),
                success: function (data) {
                    //$(q).next().next().next().html(data);
                    $('#count' + $(q).attr('data-id')).text(data);
                }
            });
        });
    });
});
