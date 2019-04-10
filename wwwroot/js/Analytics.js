$(document).ready(function () {
    let date = new Date()
    let offset = -date.getTimezoneOffset() / 60;

    $("form input[type='hidden']").val(offset);

    $('form').submit(function(){
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/LoadAnalytics",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                document.write(data);
            }
        });
    });

    $("form input[type='checkbox']").on("change", function () {
        $(this).val($(this).prop('checked') ? 1 : 0);
    });
});