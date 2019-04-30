/*$(document).ready(function () {
    let date = new Date()
    let offset = -date.getTimezoneOffset() / 60;

    $("#first-input-hidden").val(offset);
    $('.result > ul').hide();
    $('.analytics > form').submit(function (e) {
        var form = $(this);
        if ($('.analytics > form #select-product-name').val() == 0) {
        $.ajax({
            url: "/AdminMenu/Analytics/Api/GetGeneralAnalytics",
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
        url: "/AdminMenu/Analytics/Api/GetAnalytics",
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
}*/

new Vue({
    el: '#analytics',
    data: {
        categories: [],
        products: [],
        users: [],
        categoryId: 0,
        productId: 0,
        userId: 0,
        test: [],
        date: '',
        offset: '',
        DateFrom: '',
        DateTo: '',
        AllTime: 0,
        LastItemId: 0,
        productsAnalytics: [],
        genAnalytics: null,
        months: {
            '01': 'Январь',
            '02': 'Февраль',
            '03': 'Март',
            '04': 'Апрель',
            '05': 'Май',
            '06': 'Июнь',
            '07': 'Июль',
            '08': 'Август',
            '09': 'Сентябрь',
            '10': 'Октябрь',
            '11': 'Ноябрь',
            '12': 'Декабрь'
        }
    },
    mounted: function () {
        this.date = new Date();
        this.offset = -this.date.getTimezoneOffset() / 60;
        axios
        .post('/AdminMenu/Analytics/Api/GetAnalyticsFields')
        .then(response => {
            this.categories = response.data.Categories;
            this.products = response.data.Products;
            this.users = response.data.Users;
        });
    },
    methods: {
        showRelevantProducts: function () {
            axios
                .post('/AdminMenu/Analytics/Api/GetCategoryProducts', null, {
                    params: {
                        categoryId: this.categoryId
                    }
                })
                .then(response => {
                    this.products = response.data;
                });
        },
        GetAnalytics: function () {
            axios
                .post('/AdminMenu/Analytics/Api/GetAnalytics', null, {
                    params: {
                        TimeZoneOffset: this.offset,
                        CategoryId: this.categoryId,
                        ProductId: this.productId,
                        UserId: this.userId,
                        DateFrom: this.DateFrom,
                        DateTo: this.DateTo,
                        AllTime: this.AllTime,
                        LastItemId: this.LastItemId
                    }
                })
                .then(response => {
                    this.productsAnalytics = response.data;
                });
        },
        GetGeneralAnalytics: function (event) {
            axios
                .post('/AdminMenu/Analytics/Api/GetGeneralAnalytics', null, {
                    params: {
                        TimeZoneOffset: this.offset,
                        CategoryId: this.categoryId,
                        ProductId: this.productId,
                        UserId: this.userId,
                        DateFrom: this.DateFrom,
                        DateTo: this.DateTo,
                        AllTime: this.AllTime,
                        LastItemId: this.LastItemId
                    }
                })
                .then(response => {
                    this.genAnalytics = response.data;
<<<<<<< HEAD
                    this.GetAnalytics(event)
                })

            event.preventDefault();
        },
        selectAllTime: function () {
            !this.AllTime
=======
                    this.GetAnalytics();
                });
            event.preventDefault();
>>>>>>> e507da5688cf7b7b72c36cae5476b2d3394405bc
        }
    }
})