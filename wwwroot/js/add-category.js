$('.add-new-category').each(function() {
    $(this).submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/AddCategory",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                //document.write(data);
                alert('!');
            }
        });
        e.preventDefault();
    });
})
