$(document).ready(function () {
    let date = new Date()
    let offset = -date.getTimezoneOffset() / 60;

    $("#first-input-hidden").val(offset);
    $('.result > ul').hide();
    $('.analytics > form').submit(function (e) {
        var form = $(this);
        if ($('.analytics > form #select-product-name').val() == 0) {
        $.ajax({
            url: "/AdminMenu/LoadGeneralAnalytics",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                $('.result > ul').empty();        
                var genResult = JSON.parse(data);            
                $('.result > ul').append('<li class="products-in-analytics"><div class="name-in-analitics"><h4>Общая статистика</h4><div class="open-order"><i class="fas fa-chevron-circle-down"></i></div></div><div class="toggle"><h5 class="overall">В сумме продано: ' + genResult["Sales"] + ', выручка: ' + genResult["Revenue"] + '</h5><ul class="months"></ul></div></li>');
                for (var j = 0; j < genResult["MonthsState"].length; j++) {
                    $('li.products-in-analytics:eq(0)').children('.toggle').children('.months').append('<li><h5><span>' + months[genResult["MonthsState"][j]["Month"].substr(5, 2)] + '</span><span> ' + genResult["MonthsState"][j]["Month"].substr(0, 4) + '</span></h5></li>');
                    $('li.products-in-analytics:eq(0)').children('.toggle').children('.months').children('li:eq(' + j + ')').append('<table><tr><th>День</th><th>Продано</th><th>Выручка</th></tr></table>');
                    for (var k = 0; k < genResult["MonthsState"][j]["Days"].length; k++) {
                        $('li.products-in-analytics:eq(0)').children('.toggle').children('.months').children('li:eq(' + j + ')').children('table').append('<tr><td>' + genResult["MonthsState"][j]["Days"][k]["Day"].substr(8, 2) + '</td><td>' + genResult["MonthsState"][j]["Days"][k]["Sales"] + '</td><td>' + genResult["MonthsState"][j]["Days"][k]["Revenue"] + '</td></tr>');
                    }
                };
                analitic (form);
            }
        });
        
        } else {
            $('.result > ul').empty();
            analitic (form);
        };
        e.preventDefault();
    });

    $("form input[type='checkbox']").on("change", function () {
        $(this).val($(this).prop('checked') ? 1 : 0);
    });

    

});

var months = {'01': 'Январь', '02': 'Февраль', '03': 'Март', '04': 'Апрель', '05': 'Май', '06': 'Июнь', '07': 'Июль', '08': 'Август', '09': 'Сентябрь', '10': 'Октябрь', '11': 'Ноябрь', '12': 'Декабрь'};

function analitic (form) {
    $(form).attr('asp-action', 'LoadAnalytics');
    $(form).attr('action', '/AdminMenu/LoadAnalytics');
    $.ajax({
        url: "/AdminMenu/LoadAnalytics",
        type: "POST",
        data: form.serialize(),
        success: function (data) {
            $('.result > ul').show();
            var result = JSON.parse(data);
            for (var i = 0; i < result.length; i++) {
                $('.result > ul').append('<li class="products-in-analytics"><div class="name-in-analitics"><h4>' + result[i]["Product"]["Name"] +'</h4><div class="open-order"><i class="fas fa-chevron-circle-down"></i></div></div><div class="toggle"><h5 class="overall">В сумме продано: ' + result[i]["Sales"] + ', выручка: ' + result[i]["Revenue"] + '</h5><ul class="months"></ul></div></li>');
                for (var j = 0; j < result[i]["MonthsState"].length; j++) {
                    $('li.products-in-analytics:last-child').children('.toggle').children('.months').append('<li><h5><span>' + months[result[i]["MonthsState"][j]["Month"].substr(5, 2)] + '</span><span> ' + result[i]["MonthsState"][j]["Month"].substr(0, 4) + '</span></h5></li>');
                    $('li.products-in-analytics:last-child').children('.toggle').children('.months').children('li:eq(' + j + ')').append('<table><tr><th>День</th><th>Продано</th><th>Выручка</th></tr></table>');
                    for (var k = 0; k < result[i]["MonthsState"][j]["Days"].length; k++) {
                        $('li.products-in-analytics:last-child').children('.toggle').children('.months').children('li:eq(' + j + ')').children('table').append('<tr><td>' + result[i]["MonthsState"][j]["Days"][k]["Day"].substr(8, 2) + '</td><td>' + result[i]["MonthsState"][j]["Days"][k]["Sales"] + '</td><td>' + result[i]["MonthsState"][j]["Days"][k]["Revenue"] + '</td></tr>');
                    }
                }
            };
            
            if (document.documentElement.scrollTop > 500) {
                $('#second-input-hidden').val($('#second-input-hidden').val() + 10);
                analitic (form)
            };
            $('.toggle').hide();
            $('.open-order').each(function () {
                $(this).on('click', function () {
                    $(this).children().toggleClass('fa-chevron-circle-down');
                    $(this).children().toggleClass('fa-chevron-circle-up');
                    $(this).parent().next().slideToggle();
                })
            })
        }
    });
}