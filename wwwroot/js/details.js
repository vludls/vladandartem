/*$(document).ready(function () {
    $('.add-field').submit(function (e) {
        var form = $(this);
        $.ajax({
            url: "/AdminMenu/DetailField/Api/Add",
            type: "POST",
            data: form.serialize(),
            success: function (data) {
                if (data.length != 0) {
                    $('.add-new-field').append('<option value="' + data + '" id="o' + data + '">' + $('#DetailFieldName').val() + '</option>');
                    $('.add-new-field-1').append('<option value="' + data + '" id="odel' + data + '">' + $('#DetailFieldName').val() + '</option>');
                    $('.list-of-fields').append('<li id="li' + data + '"><p class="field-name">' + $('#DetailFieldName').val() + '</p><form asp-action="ProductDetailsDefinitionDelete" method="post"><div class="select-value"><select name="DefinitionId" class="form-control" id="d' + data + '"><option value="0">Выберите значение</option></select></div><div class="last-column"><button type="button" class="btn btn-danger" data-toggle="modal" data-target="#m' + data + '" id="id' + data + '">Удалить</button></div></form></li>');
                    $('#id' + data).click(function () {
                        var definsub = $(this);
                        $('.for-del-def').attr('id', $(this).attr('data-target').substr(1));
                        $('.delete-def').click(function () {
                            $(definsub).closest('form').submit(function (e) {
                                var deldef = $(this);
                                $.ajax({
                                    url: "/AdminMenu/ProductDetailsDefinitionDelete",
                                    type: "POST",
                                    data: deldef.serialize(),
                                    success: function (data) {
                                        $('#def' + data).remove();
                                    }
                                });
                                e.preventDefault();
                            });
                            $(definsub).closest('form').trigger('submit');
                            $(this).prev().trigger('click');
                        })
                    });

                };
                $('#DetailFieldName').val('');
            }
        });
        e.preventDefault();
    });
    $('.definition-add').submit(function (e) {
        var defform = $(this);
        $.ajax({
            url: "/AdminMenu/DetailField/Definition/Api/Add",
            type: "POST",
            data: defform.serialize(),
            success: function (data) {
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
            url: "/AdminMenu/DetailField/Api/Delete",
            type: "POST",
            data: delfield.serialize(),
            success: function (data) {
                $('#o' + data.DetailFieldId).remove();
                $('#odel' + data.DetailFieldId).remove();
                $('#li' + data.DetailFieldId).remove();
            }
        });
        e.preventDefault();
    });
    $('.del-field > button').click(function () {
        var sub = $(this);
        $(this).attr('data-target', '#del-field');
        $('.for-del-field').attr('id', $(this).attr('data-target').substr(1));
        $('.delete-field').click(function () {
            $('.del-field').trigger('submit');
            $(this).prev().trigger('click');
        })
    });

    $('.delete-defininion').each(function () {
        $(this).submit(function (e) {
            var deldef = $(this);
            $.ajax({
                url: "/AdminMenu/DetailField/Definition/Api/Delete",
                type: "POST",
                data: deldef.serialize(),
                success: function (data) {
                    $('#def' + data).remove();
                }
            });
            e.preventDefault();
        });
    })


    $('.last-column > button').each(function () {
        $(this).click(function () {
            var defsub = $(this);
            $('.for-del-def').attr('id', $(this).attr('data-target').substr(1));
            $('.delete-def').click(function () {
                $(defsub).closest('form').trigger('submit');
                $(this).prev().trigger('click');
            })
        });
    });
})*/

new Vue({
    el: '#detail-fields',
    data: {
        fields: [],
        fieldName: '',
        definitionName: '',
        fieldIdForAddDefin: '',
        fieldIdForDelete: ''
    },
    mounted: function () {
        axios
            .post('/AdminMenu/DetailField/GetAll')
            .then(response => {
                this.fields = response.data;
            });
    },
    methods: {
        addField: function () {
            axios
                .post('/AdminMenu/DetailField/Api/Add', null, {
                    params: {
                        detailFieldName: this.fieldName
                    }
                })
                .then(response => {
                    //alert(response.data.DetailFieldId)
                    this.fields.push(response.data)
                });
        },
        addDefinition: function () {
            axios
                .post('/AdminMenu/DetailField/Definition/Api/Add', null, {
                    params: {
                        detailFieldId: this.fieldIdForAddDefin,
                        definitionName: this.definitionName
                    }
                })
                .then(response => {
                    alert(response.data)
                });
        }
    }
})