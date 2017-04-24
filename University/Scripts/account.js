var optionSpeciality = "<option value=''> Выбор специальности </option>";
var optionGroup = "<option value=''> Выбор группы </option>";

function StartSelect(nameSelect) {
    $("#" + nameSelect).attr('class', 'form-control select-default');
    $("#" + nameSelect).find('option').attr('class', 'select-elem');
    $("#" + nameSelect + " option:first").attr('class', 'select-default');
}

function ChangedColorSelect(nameSelect) {
    var id = $("#" + nameSelect).val();
    if (id === "") {
        $("#" + nameSelect).attr('class', 'form-control select-default');
    } else {
        $("#" + nameSelect).attr('class', 'form-control select-elem');
    }
}

$(function () {
    $('#faculty').change(function () {
        // получаем выбранный id
        var id = $(this).val();
        ChangedColorSelect("faculty");
        if (id !== "") {  
            $.get("/Account/GetSpecialities",
            { id: id },
            function(data) {
                // заменяем содержимое присланным частичным представлением
                $('#speciality').replaceWith(data);
                $('#speciality').prepend(optionSpeciality);
                $("#speciality option:first").attr('selected', 'selected');
                StartSelect("speciality");
                $('#speciality').change(function () {
                    var id = $(this).val();
                    ChangedColorSelect("speciality");
                    if (id !== "") {
                        $.get("/Account/GetGroups",
                            { id: id },
                            function(data) {
                                $('#group').replaceWith(data);
                                $('#group').prepend(optionGroup);
                                $("#group option:first").attr('selected', 'selected');
                                StartSelect("group");
                                $('#group').change(function () {
                                    ChangedColorSelect("group");
                                });
                            });
                    } else {
                        $('#group').find('option').remove();
                        $('#group').prepend(optionGroup);
                    }
                    ChangedColorSelect("group");
                });
        });
        } else {
            $('#speciality').find('option').remove();
            $('#speciality').prepend(optionSpeciality);
            $('#group').find('option').remove();
            $('#group').prepend(optionGroup);
        }
        ChangedColorSelect("speciality");
        ChangedColorSelect("group");
    });
});

$(function() {
    $('#role').change(function () {
        ChangedColorSelect("role");
        var id = $(this).val();
        if (id === 'student') {
            $('#studentParams').show();
            $('#teacherParams').hide();
            $('#department').val($("#department option:first").val());
            ChangedColorSelect("department");
        } else {
            $('#studentParams').hide();            
            $('#faculty').val($("#faculty option:first").val());
            ChangedColorSelect("faculty"); 
            $('#speciality').find('option').remove();
            $('#speciality').prepend(optionSpeciality);
            ChangedColorSelect("speciality");
            $('#group').find('option').remove();
            $('#group').prepend(optionGroup);
            ChangedColorSelect("group");
            if (id === 'teacher') {        
                $('#teacherParams').show();
            } else {
                $('#teacherParams').hide();
                $('#department').val($("#department option:first").val());
                ChangedColorSelect("department");
            }
        }
    });
});

var loadFile = function (event) {
    var output = document.getElementById('output');
    output.src = URL.createObjectURL(event.target.files[0]);
};

$(function() {
    $('#date').change(function () {
        if ($(this).val() !== "") {
            $('#date').attr('class', 'date-input form-control select-elem');
        } else {
            $('#date').attr('class', 'date-input form-control select-default');
        }
    });
});