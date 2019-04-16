$(document).ready(function (){
    $('.add-field').submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/ProductDetailsFieldAdd",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                //alert(data);
                $('.add-new-field').append('<option value="' + data + '" id="o' + data + '">' + $('#DetailFieldName').val() + '</option>');
                $('.add-new-field-1').append('<option value="' + data + '" id="odel' + data + '">' + $('#DetailFieldName').val() + '</option>');
                //$('.list-detail-fields:last:child').clone().appendTo($('.list-of-fields'));
                //$('.list-of-fields:last-child').children('.field-name').text($('#DetailFieldName').val());
                $('.list-of-fields').append('<li id="li' + data + '"><p class="field-name">' + $('#DetailFieldName').val() + '</p><form asp-action="ProductDetailsDefinitionDelete" method="post"><div class="select-value"><select name="DefinitionId" class="form-control" id="d' + data + '"><option value="0">Выберите значение</option></select></div><div class="last-column"><button type="button" class="btn btn-danger" data-toggle="modal" data-target="#m' + data + '" id="id' + data + '">Удалить</button></div></form></li>');
                $('#id' + data).click(function() {
                    //alert($(this).attr('data-target'));
                    //alert('!');
                    
                    var definsub = $(this);
                    $('.for-del-def').attr('id', $(this).attr('data-target').substr(1));
                    $('.delete-def').click(function() {
                        $(definsub).closest('form').submit(function (e) {
                            var deldef = $(this);
                            $.ajax({
                                url: "/AdminMenu/ProductDetailsDefinitionDelete",
                                type: "POST",
                                data: deldef.serialize(),
                                success: function (data) {
                                    //$(deldef + ' button').attr('data-target', '#' + data);
                                    $('#def' + data).remove();  
                                }
                            });
                            e.preventDefault();
                        });
                        $(definsub).closest('form').trigger('submit');
                        $(this).prev().trigger('click');
                    })
                });
                $('#DetailFieldName').val('');
            }
        });
        e.preventDefault();
    });
    $('.definition-add').submit(function (e) {
        var defform = $(this);
        $.ajax({
            url: "/AdminMenu/ProductDetailsDefinitionAdd",
            type: "POST",
            data: defform.serialize(),
            success: function (data) {
                //alert(data);
                var ids = JSON.parse(data);
                $('#d' + ids["DetailFieldId"]).append('<option value="' + ids["DefinitionId"] + '" id="def' + ids["DefinitionId"] + '">' + ids["DefinitionName"] + '</option>');
                $('#DefinitionName').val('');
            }
        });
        e.preventDefault();
    });
    $('.del-field').submit(function (e) {
        var delfield = $(this);
        $.ajax({
            url: "/AdminMenu/ProductDetailsFieldDelete",
            type: "POST",
            data: delfield.serialize(),
            success: function (data) {
                //alert(data);
                $('#o' + data).remove();
                $('#odel' + data).remove();
                $('#li' + data).remove();
            }
        });
        e.preventDefault();
    });
    $('.del-field > button').click(function() {
        var sub = $(this);
        $(this).attr('data-target', '#del-field');
        $('.for-del-field').attr('id', $(this).attr('data-target').substr(1));
        $('.delete-field').click(function() {
            $('.del-field').trigger('submit');
            $(this).prev().trigger('click');
        })
    });

    $('.delete-defininion').each(function() {
        $(this).submit(function (e) {
            var deldef = $(this);
            $.ajax({
                url: "/AdminMenu/ProductDetailsDefinitionDelete",
                type: "POST",
                data: deldef.serialize(),
                success: function (data) {
                    //$(deldef + ' button').attr('data-target', '#' + data);
                    $('#def' + data).remove();  
                }
            });
            e.preventDefault();
        });
    })
    

    $('.last-column > button').each(function () {
        $(this).click(function() {
            var defsub = $(this);
            //$(this).attr('data-target', '#del-def');
            $('.for-del-def').attr('id', $(this).attr('data-target').substr(1));
            $('.delete-def').click(function() {
                $(defsub).closest('form').trigger('submit');
                $(this).prev().trigger('click');
            })
        });
    });
})