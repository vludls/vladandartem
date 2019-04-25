new Vue ({
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
            .post('/AdminMenu/GetUsers')
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
            const data = new FormData(document.querySelector('.user-delete'));
            data.append('id', this.userId);
            axios
            .post('/AdminMenu/DeleteUser', data 
            )
            .then( response => {
                //alert(response.data.UserId);
                if (response.data.UserId != 0) {
                    this.users.splice(this.index, 1)
                }
            });
            event.preventDefault();
        },
        closeModal: function() {
            $('.close-modal').trigger('click');
        }
    }
})
