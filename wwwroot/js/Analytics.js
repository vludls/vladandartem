$(document).ready(function () {
    let date = new Date()
    let offset = -date.getTimezoneOffset() / 60;

    $("#first-input-hidden").val(offset);

    $('.analytics > form').submit(function (e) {
        //if ($('.analytics > form #select-product-name').val() == 0) {
          
            
        var genForm = $(this);
        $.ajax({
            url: "/AdminMenu/LoadGeneralAnalytics",
            type: "POST",
            data: genForm.serialize(),
            success: function (data) {
                $('.result > ul').empty();        
                var genResult = JSON.parse(data);
                $('.test').text(data);
                //alert(genResult["Sales"]);
                
                
                $('.result > ul').append('<li class="products-in-analytics"><h5 class="name-in-analitics">Общая статистика</h5><h5 class="overall">В сумме продано: ' + genResult["Sales"] + ', выручка: ' + genResult["Revenue"] + '</h5><ul class="months"></ul></li>');
                for (var j = 0; j < genResult["MonthsState"].length; j++) {
                    $('li.products-in-analytics:eq(0)').children('.months').append('<li><h5><span>' + months[genResult["MonthsState"][j]["Month"].substr(5, 2)] + '</span><span> ' + genResult["MonthsState"][j]["Month"].substr(0, 4) + '</span></h5></li>');
                    $('li.products-in-analytics:eq(0)').children('.months').children('li:eq(' + j + ')').append('<table><tr><th>День</th><th>Продано</th><th>Выручка</th></tr></table>');
                    for (var k = 0; k < genResult["MonthsState"][j]["Days"].length; k++) {
                        $('li.products-in-analytics:eq(0)').children('.months').children('li:eq(' + j + ')').children('table').append('<tr><td>' + genResult["MonthsState"][j]["Days"][k]["Day"].substr(8, 2) + '</td><td>' + genResult["MonthsState"][j]["Days"][k]["Sales"] + '</td><td>' + genResult["MonthsState"][j]["Days"][k]["Revenue"] + '</td></tr>');
                    }
                }; 
            }
        });
        var form = $(this);
        //$(form).attr('asp-action', 'LoadAnalytics');
        //$(form).attr('action', '/AdminMenu/LoadAnalytics');
        $.ajax({
            url: "/AdminMenu/LoadAnalytics",
            type: "POST",
            data: form.serialize(),
            success: function (data) {              
                var result = JSON.parse(data);
                $('.test').text(data);
                $('.jason').text(result[0]["MonthsState"][0]["Days"][0]["Day"].substr(8, 2));
                //$('.result > ul').children().remove();
                for (var i = 0; i < result.length; i++) {
                    $('.result > ul').append('<li class="products-in-analytics"><h5 class="name-in-analitics">' + result[i]["Product"]["Name"] +'</h5><h5 class="overall">В сумме продано: ' + result[i]["Sales"] + ', выручка: ' + result[i]["Revenue"] + '</h5><ul class="months"></ul></li>');
                    for (var j = 0; j < result[i]["MonthsState"].length; j++) {
                        $('li.products-in-analytics:last-child').children('.months').append('<li><h5><span>' + months[result[i]["MonthsState"][j]["Month"].substr(5, 2)] + '</span><span> ' + result[i]["MonthsState"][j]["Month"].substr(0, 4) + '</span></h5></li>');
                        $('li.products-in-analytics:last-child').children('.months').children('li:eq(' + j + ')').append('<table><tr><th>День</th><th>Продано</th><th>Выручка</th></tr></table>');
                        for (var k = 0; k < result[i]["MonthsState"][j]["Days"].length; k++) {
                            $('li.products-in-analytics:last-child').children('.months').children('li:eq(' + j + ')').children('table').append('<tr><td>' + result[i]["MonthsState"][j]["Days"][k]["Day"].substr(8, 2) + '</td><td>' + result[i]["MonthsState"][j]["Days"][k]["Sales"] + '</td><td>' + result[i]["MonthsState"][j]["Days"][k]["Revenue"] + '</td></tr>');
                        }
                    }
                };  
            }
        });
        /*} else {
            var form = $(this);
            $(form).attr('asp-action', 'LoadAnalytics');
            $(form).attr('action', '/AdminMenu/LoadAnalytics');
            $.ajax({
                url: "/AdminMenu/LoadAnalytics",
                type: "POST",
                data: form.serialize(),
                success: function (data) {              
                    var result = JSON.parse(data);
                    $('.test').text(data);
                    $('.jason').text(result[0]["MonthsState"][0]["Days"][0]["Day"].substr(8, 2));
                    //$('.result > ul').children().remove();
                    $('.result > ul').children().remove();
                    for (var i = 0; i < result.length; i++) {
                        $('.result > ul').append('<li class="products-in-analytics"><h5 class="name-in-analitics">' + result[i]["Product"]["Name"] +'</h5><h5 class="overall">В сумме продано: ' + result[i]["Sales"] + ', выручка: ' + result[i]["Revenue"] + '</h5><ul class="months"></ul></li>');
                        for (var j = 0; j < result[i]["MonthsState"].length; j++) {
                            $('li.products-in-analytics:last-child').children('.months').append('<li><h5><span>' + months[result[i]["MonthsState"][j]["Month"].substr(5, 2)] + '</span><span> ' + result[i]["MonthsState"][j]["Month"].substr(0, 4) + '</span></h5></li>');
                            $('li.products-in-analytics:last-child').children('.months').children('li:eq(' + j + ')').append('<table><tr><th>День</th><th>Продано</th><th>Выручка</th></tr></table>');
                            for (var k = 0; k < result[i]["MonthsState"][j]["Days"].length; k++) {
                                $('li.products-in-analytics:last-child').children('.months').children('li:eq(' + j + ')').children('table').append('<tr><td>' + result[i]["MonthsState"][j]["Days"][k]["Day"].substr(8, 2) + '</td><td>' + result[i]["MonthsState"][j]["Days"][k]["Sales"] + '</td><td>' + result[i]["MonthsState"][j]["Days"][k]["Revenue"] + '</td></tr>');
                            }
                        }
                    };  
                }
            });
        }*/
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

function simpleAnalytic () {
    $.ajax({
        url: "/AdminMenu/LoadAnalytics",
        type: "POST",
        data: form.serialize(),
        success: function (data) {              
            var result = JSON.parse(data);
            $('.test').text(data);
            $('.jason').text(result[0]["MonthsState"][0]["Days"][0]["Day"].substr(8, 2));
            $('.result > ul').children().remove();
            for (var i = 0; i < result.length; i++) {
                $('.result > ul').append('<li class="products-in-analytics"><h5 class="name-in-analitics">' + result[i]["Product"]["Name"] +'</h5><h5 class="overall">В сумме продано: ' + result[i]["Sales"] + ', выручка: ' + result[i]["Revenue"] + '</h5><ul class="months"></ul></li>');
                for (var j = 0; j < result[i]["MonthsState"].length; j++) {
                    $('li.products-in-analytics:eq(' + i +')').children('.months').append('<li><h5><span>' + months[result[i]["MonthsState"][j]["Month"].substr(5, 2)] + '</span><span> ' + result[i]["MonthsState"][j]["Month"].substr(0, 4) + '</span></h5></li>');
                    $('li.products-in-analytics:eq(' + i +')').children('.months').children('li:eq(' + j + ')').append('<table><tr><th>День</th><th>Продано</th><th>Выручка</th></tr></table>');
                    for (var k = 0; k < result[i]["MonthsState"][j]["Days"].length; k++) {
                        $('li.products-in-analytics:eq(' + i +')').children('.months').children('li:eq(' + j + ')').children('table').append('<tr><td>' + result[i]["MonthsState"][j]["Days"][k]["Day"].substr(8, 2) + '</td><td>' + result[i]["MonthsState"][j]["Days"][k]["Sales"] + '</td><td>' + result[i]["MonthsState"][j]["Days"][k]["Revenue"] + '</td></tr>');
                    }
                }
            };
            
        }
    });
    e.preventDefault();
};

function generalAnalytics () {
    
}

var months = {'01': 'Январь', '02': 'Февраль', '03': 'Март', '04': 'Апрель', '05': 'Май', '06': 'Июнь', '07': 'Июль', '08': 'Август', '09': 'Сентябрь', '10': 'Октябрь', '11': 'Ноябрь', '12': 'Декабрь'}
