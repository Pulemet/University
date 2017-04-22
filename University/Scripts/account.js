var optionSpeciality = "<option value='' color='#999'> Выбор специальности </option>";
var optionGroup = "<option value='' color='#999'> Выбор группы </option>";

$(function () {
    $('#faculty').change(function () {
        // получаем выбранный id
        var id = $(this).val();
        if (id !== "") {
            $.get("/Account/GetSpecialities",
            { id: id },
            function(data) {
                // заменяем содержимое присланным частичным представлением
                $('#speciality').replaceWith(data);
                $('#speciality').change(function () {
                    var id = $(this).val();
                    if (id !== "") {
                        $.get("/Account/GetGroups",
                            { id: id },
                            function(data) {
                                $('#group').replaceWith(data);
                                $('#group').prepend(optionGroup);
                                $("#group option:first").attr('selected', 'selected').css('color', '#999');
                            });
                    } else {
                        $('#group').find('option').remove();
                        $('#group').prepend(optionGroup);
                    }
                });
                $('#speciality').prepend(optionSpeciality);
                $("#speciality option:first").attr('selected', 'selected').css('color', '#999');
        });
        } else {
            $('#speciality').find('option').remove();
            $('#speciality').prepend(optionSpeciality);
            $('#group').find('option').remove();
            $('#group').prepend(optionGroup);
        }
    });
});

$(function() {
    $('#role').change(function() {
        var id = $(this).val();
        if (id === 'Student') {
            $('#studentParams').show();     
        } else {
            $('#studentParams').hide();            
            $('#faculty').val($("#faculty option:first").val());
            $('#speciality').find('option').remove();
            $('#speciality').prepend(optionSpeciality);
            $('#group').find('option').remove();
            $('#group').prepend(optionGroup);
        }
    });
});

var loadFile = function (event) {
    var output = document.getElementById('output');
    output.src = URL.createObjectURL(event.target.files[0]);
};