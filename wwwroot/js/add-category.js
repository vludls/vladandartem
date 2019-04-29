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
new Vue({
    el: '#categories',
    data: {
        categories: [],
        modalId: 'id',
        categoryId: '',
        categoryName: '',
        sectionId: '',
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
            this.modalId = 'id' + categoryId;
            this.categoryName = categoryName;
            this.categoryId = categoryId;
            this.index = index;
        },
        deleteCategory: function (event) {
            axios
                .post('/AdminMenu/Categories/Api/Delete', null, {
                    params: {
                        categoryId: this.categoryId
                    }
                })
                .then(
                    this.categories.Categories.splice(this.index, 1)
                );
            event.preventDefault();
        },
        closeModal: function () {
            this.$refs.closeModal.click();
        },
        addCategory: function (event) {
            axios
                .post('/AdminMenu/Categories/Api/Add', null, {
                    params: {
                        sectionId: this.sectionId,
                        categoryName: this.addCategoryName
                    }
                })
                .then(response => {
                    this.categories.Categories.push(response.data);
                    this.addCategoryName = '';
                });
            event.preventDefault();
        }
    }
})
