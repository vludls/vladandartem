$(document).ready(function (){
    $('.add-field').submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/ProductDetailsFieldAdd",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                //alert(data);
                $('.add-new-field').append('<option value="' + data + '">' + $('#DetailFieldName').val() + '</option>');
                //$('.list-detail-fields:last:child').clone().appendTo($('.list-of-fields'));
                //$('.list-of-fields:last-child').children('.field-name').text($('#DetailFieldName').val());
                $('.list-of-fields').append('<li><p class="field-name">' + $('#DetailFieldName').val() + '</p><form asp-action="ProductDetailsDefinitionDelete" method="post"><div class="select-value"><select name="DefinitionId" class="form-control"><option value="0">Выберите значение</option>@foreach(var definition in detailField.Definitions){<option value="@definition.Id">@definition.Name</option>}</select></div><div class="last-column"><button type="submit" class="btn btn-danger">Удалить</button></div></form></li>');
            }
        });
        e.preventDefault();
    })
})