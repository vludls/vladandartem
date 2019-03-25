$(window).scroll(function () {

    if (document.documentElement.scrollTop > 35) {
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
$('.q').each(
    $('.q').on('input', function () {

        $(this).parent().next().children().html($(this).val() * p[$(this).closest('li').index()]);
    })
);
var a = window.location.search;
function scr() {

    document.documentElement.scrollTop = '580';
}
if (a.length > 19) {
    scr();
    $('#sear').val(a.substr(19));
}
if (a.substr(1, 5) == 'Page=') {
    scr();
};

function readURL(input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('#addimg').attr('src', e.target.result);
        }

        reader.readAsDataURL(input.files[0]);
    }
}

$("#file").change(function () {
    readURL(this);
});








