$(document).ready(function () {
    let date = new Date()
    let offset = -date.getTimezoneOffset() / 60;

    $("#first-input-hidden").val(offset);

    $('.analytics > form').submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/LoadAnalytics",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                document.write(data);
                /*
                var result = JSON.parse(data);
                $('.test').text(data);
                $('.len').html('<p>Количество товаров: ' + result.length + '</p>');
                //$('.test').text(data);
                //$('.result').text(result);
                // $('.test').text(result[0]["Product"]["Name"]);
                //var ul = $('.result > ul');
                for (var i = 0; i < result.length; i++) {
                    $('.jason').append('<p>' + result[i]["Product"]["Name"] + ' Продано: ' + result[i]["Sales"] + ' Выручка: ' + result[i]["Revenue"] + '</p>');
                    $('.jason').append('<p>' + result[i]["MonthsState"].length + '</p>');
                }*/
            }
        });
        e.preventDefault();
    });

    $("form input[type='checkbox']").on("change", function () {
        $(this).val($(this).prop('checked') ? 1 : 0);
    });

    $('.months').hide();
    $('.name-in-analitics').each(function () {
        $(this).click(function () {
            $(this).next().slideToggle();
        });
    });



});