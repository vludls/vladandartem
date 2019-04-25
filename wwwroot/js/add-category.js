/*$('.add-new-category').each(function() {
    $(this).submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/AddCategory",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                var NameOfCategory = JSON.parse(data);
                $('.users').append('<li><p>' + NameOfCategory["Name"] + '</p><div class="centering"><button type="submit" class="btn btn-danger delete-user" data-toggle="modal" data-route-id="' + NameOfCategory["Id"] + '" data-target="#a'+ NameOfCategory["Id"] +'">Удалить</button></div></li>');
                $('.add-category').val('');
                $('.centering > button').each(forAddCategory); //Для модалки
            }
        });
        e.preventDefault();
    });
})*/
new Vue ({
    el: '#categories',
    data: {

    },
    mounted: function () {
        axios
            .post('/AdminMenu/Categories/Api/Get')
            .then(response => {
                this.sections = response.data;
            });
    },
})
