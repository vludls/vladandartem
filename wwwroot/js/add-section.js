/*$('.add-new-category').each(function() {
    $(this).submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/SectionAdd",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                var NameOfSection = JSON.parse(data);
                $('.users').append('<li id="s' + NameOfSection["Id"] + '"><p>' + NameOfSection["Name"] + '</p><div class="centering"><button type="submit" class="btn btn-danger delete-user" data-toggle="modal" data-route-id="' + NameOfSection["Id"] + '" data-target="#a'+ NameOfSection["Id"] +'">Удалить</button></div></li>');
                $('.add-category').val('');
                $('.centering > button').each(forAddCategory); //Для модалки
            }
        });
        e.preventDefault();
    });
});

$('.section-delete').submit(function (e) {
    var del = $(this);
    $.ajax({
        url: "/AdminMenu/SectionDelete",
        type: "POST",
        data: del.serialize(),
        success: function (data) {
            var DelSection = data;
            $('#s' + DelSection).remove();
            $('.close-modal').trigger('click');
        }
    });
    e.preventDefault();
})*/
new Vue ({
    el: '#sections',
    data: {
        sections: []
    },
    mounted: function () {
        axios
            .post('/AdminMenu/GetSections')
            .then(response => {
                this.sections = response.data;
            });
    },
})
