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
            }
        });
        e.preventDefault();
    });
});