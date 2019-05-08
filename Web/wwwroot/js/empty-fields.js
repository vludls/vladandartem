//Подсвечивать незаполненные поля красным
$(document).ready(function () {
    $('.regist span').each(function () {
        if ($(this).html() != "") {
            $(this).css('color', 'red');
            $(this).prev().css('border-color', 'red');

        }
    })
})