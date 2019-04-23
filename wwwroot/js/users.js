/*$(document).ready(modalForDelete);
function modalForDelete() {
    $('.centering > button').each(forAddCategory);
};
$('.user-delete').submit(function (e) {
    var delUser = $(this);
    $.ajax({
        url: "/AdminMenu/DeleteUser",
        type: "POST",
        data: delUser.serialize(),
        success: function (data) {
            var DelUser = data;
            $('#u' + DelUser).remove();
            $('.close-modal').trigger('click');
        }
    });
    e.preventDefault();
})*/
new Vue ({
    el: '#users',
    data: {
        users: [],
        editUser: 'EditUser?id=',
        modalId: 'id',
        id: '',
        user: '',
        UserId: ''
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
        activateModal: function (userId, userName) {
            this.modalId = 'id';
            this.modalId += userId;
            this.user = '';
            this.user += userName;
            this.id = '';
            this.id += userId
        },
        deleteUser: function (event) {
            const data = new FormData(document.querySelector('.user-delete'));
            data.append('id', this.id);
            axios
            .post('/AdminMenu/DeleteUser', data 
            )
            .then(response => {
                //this.users = response.data;
                this.UserId = response.data;
                
            });
            event.preventDefault();
        }
    }
})
