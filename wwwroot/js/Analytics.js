$(document).ready(function () {
    let date = new Date()
    let offset = -date.getTimezoneOffset() / 60;

    $("form input[type='hidden']").val(offset);

    $('form').submit(function(e){
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/LoadAnalytics",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                var result = JSON.parse(data);
                
                //$('.result').text(result);
                $('.result').text(result[0]["Product"]["Name"]);
                var ul = $('.result > ul');
                for (var i = 0; i < result.length; i++) {
                    
                }
            }
        });
        e.preventDefault();
    });

    $("form input[type='checkbox']").on("change", function () {
        $(this).val($(this).prop('checked') ? 1 : 0);
    });

    $('.months').hide();
    $('.name-in-analitics').each(function(){
        $(this).click(function(){
            $(this).next().slideToggle();
        });
    });

    

});