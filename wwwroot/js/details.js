$(document).ready(function (){
    $('.add-field').submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/ProductDetailsFieldAdd",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                alert(data);
            }
        });
        e.preventDefault();
    })
})