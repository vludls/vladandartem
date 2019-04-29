new Vue({
    el: '#users',
    data: {
        users: [],
        editUser: 'EditUser?id=',
        modalId: '',
        userId: '',
        userName: '',
        index: ''
    },
    mounted: function () {
        axios
            .post('/AdminMenu/Users/Api/Get')
            .then(response => {
                this.users = response.data;
            });
    },
    methods: {
        setLink: function (userId) {
            this.editUser += userId
        },
        activateModal: function (userId, userName, index) {
            this.modalId = 'id';
            this.modalId += userId;
            this.userName = userName;
            this.userId = userId;
            this.index = index
        },
        deleteUser: function (event) {
            axios
                .post('/AdminMenu/DeleteUser', null, {
                    params: {
                        id: this.userId
                    }
                })
                .then(response => {
                    if (response.data.UserId != 0) {
                        this.users.splice(this.index, 1)
                    }
                });
            event.preventDefault();
        },
        closeModal: function () {
            $('.close-modal').trigger('click');
        }
    }
})
