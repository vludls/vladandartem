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
        categories: [],
        modalId: 'id',
        categoryId: '',
        categoryName: '',
        index: '',
        addCategoryName: ''
    },
    mounted: function () {
        axios
            .post('/AdminMenu/Categories/Api/Get')
            .then(response => {
                this.categories = response.data;
            });
    },
    methods: {
        activateModal: function (categoryId, categoryName, index) {
            this.modalId = 'id';
            this.modalId += categoryId;
            this.categoryName = categoryName;
            this.categoryId = categoryId;
            this.index = index
        },
        deleteCategory: function (event) {
            const data = new FormData(document.querySelector('.delete-category'));
            data.append('categoryId', this.categoryId);
            axios
            .post('/AdminMenu/Categories/Api/Delete', data
            )
            .then(
                this.categories.Categories[0].splice(this.index, 1)
            );
            event.preventDefault();
        },
        closeModal: function () {
            $('.close-modal').trigger('click');
        },
        addCategory: function (event) {
            const data = new FormData(document.querySelector('.add-new-category'));
            data.append('sectionName', this.addCategoryName);
            axios
            .post('/AdminMenu/Categories/Api/Add', data
            )
            .then(response => {
                this.categories.push(response.data);
                this.addCategoryName = ''
            });
            event.preventDefault();
        }
    }
})
