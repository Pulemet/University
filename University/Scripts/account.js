var optionSpeciality = "<option value=''> Выбор специальности </option>";
var optionGroup = "<option value=''> Выбор группы </option>";
var optionSubject = "<option value=''> Выбор предмета </option>";

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

var addAvatar = function (event) {
    var output = document.getElementById('output');
    output.src = URL.createObjectURL(event.target.files[0]);
};

var changeAvatar = function (event) {
    var output = document.getElementById('output');
    output.src = URL.createObjectURL(event.target.files[0]);
    $("#buttonChangePhoto").prop('disabled', false);
    $("#buttonChangePhoto").html("<i class='fa fa-save fa'> Изменить </i>");
};

function ChangeAboutInfo() {
    var aboutInfo = $('#aboutInfoInput').val();
    var url = '/Manage/ChangeAboutInfo';
    $.post(url, { aboutInfo: aboutInfo }, function() {
        $("#buttonAddAboutInfo").prop('disabled', true);
        $("#aboutInfoInput").val("");
    });
}

$(function() {
    $('#date').change(function () {
        if ($(this).val() !== "") {
            $('#date').attr('class', 'date-input form-control select-elem');
        } else {
            $('#date').attr('class', 'date-input form-control select-default');
        }
    });
});

function SubmitAvatar() {
    var formData = new FormData();
    var file = document.getElementById("avatar").files[0];
    formData.append("loadFile", file);
    $.ajax({
        type: "POST",
        url: '/Manage/ChangeAvatar',
        data: formData,
        dataType: 'html',
        contentType: false,
        processData: false,
        success: function () {
            $('#buttonChangePhoto').prop('disabled', true);
            $("#buttonChangePhoto").html("<i class='fa fa-save fa'> Изменено </i>");
        },
        error: function () {
            window.location.reload();
            alert("Error");
        }
    });
}

$(function() {
    $('#subject').change(function() {
        ChangedColorSelect("subject");
        if ($(this).val() !== "") {
            $('#buttonAddSubject').prop('disabled', false); 
        } else {
            $('#buttonAddSubject').prop('disabled', true);
        }
    });
});

function AddSubjectToTeacher() {
    var id = $('#subject').val();
    var text = $('#subject option:selected').text();
    $.get("/Manage/AddSubject",
        { id: id },
        function(data) {
            $('#subject').replaceWith(data);
            $('#subject').prepend(optionSubject);
            $("#subject option:first").attr('selected', 'selected');
            StartSelect("subject");
            $('#buttonAddSubject').prop('disabled', true);
            var newSubject = "<p>" + text + "</p>";
            $('#listSubjects').append(newSubject);
            $('#subject').change(function() {
                ChangedColorSelect("subject");
                if ($(this).val() !== "") {
                    $('#buttonAddSubject').prop('disabled', false);
                } else {
                    $('#buttonAddSubject').prop('disabled', true);
                }
            });
        });
}