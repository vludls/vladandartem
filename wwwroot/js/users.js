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
        users: []
    },
    mounted: function () {
        axios
            .post('/AdminMenu/GetUsers')
            .then(response => {
                this.users = response.data;
                
            });
      },
})
