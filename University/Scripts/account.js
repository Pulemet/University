$(function () {
    $('#faculty').change(function () {
        // получаем выбранный id
        var id = $(this).val();
        $.get("/Account/GetSpecialities",
            { id: id },
            function(data) {
                // заменяем содержимое присланным частичным представлением
                $('#speciality').replaceWith(data);
                $('#speciality').change(function () {
                    var id = $(this).val();
                    $.get("/Account/GetGroups",
                        { id: id },
                         function (data) {
                             $('#group').replaceWith(data);
                               
                        });
                });
                $("#speciality option:first").attr('selected', 'selected');
                var selectSpecialityId = $('#speciality').val();
                $.get("/Account/GetGroups", { id: selectSpecialityId},
                    function (data) {
                        $('#group').replaceWith(data);
                        $('#speciality').change();
                        $("#group option:first").attr('selected', 'selected');
                });
        });
    });
});