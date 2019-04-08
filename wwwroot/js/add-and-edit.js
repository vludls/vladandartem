//Предпросмотр изображения
function readURL(input) {

    if (input.files && input.files[0]) {
        var reader = new FileReader();

        reader.onload = function (e) {
            $('.addimg').attr('src', e.target.result);
        };

        reader.readAsDataURL(input.files[0]);
    }
}

$(".file").change(function () {
    readURL(this);
});
//Предпросмотр изображения