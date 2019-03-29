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
                    $(q).next().next().html(data);

                }
            });
        });
    });
});









