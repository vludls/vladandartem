$('.add-new-category').each(function() {
    $(this).submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/SectionAdd",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                //var NameOfSection = JSON.parse(data);
                alert(data);
                /*$('.users').append('<li><p>' + NameOfSection["Name"] + '</p><div class="centering"><button type="submit" class="btn btn-danger delete-user" data-toggle="modal" data-route-id="' + NameOfSection["Id"] + '" data-target="#a'+ NameOfSection["Id"] +'">Удалить</button></div></li>');
                $('#add-category').val('');
                $('.centering > button').each(forAddCategory);*/ //Для модалки
            }
        });
        e.preventDefault();
    });
})