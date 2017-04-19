$(function () {
    $("input[type='date']")
                .datepicker({ dateFormat: 'dd/mm/yy' })
                .get(0).setAttribute("type", "text");
    $.datepicker.regional['ru'] = {
        prevText: 'Пред',
        nextText: 'След',
        monthNames: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь',
        'Июль', 'Август', 'Сентябрь', 'Октябрь', 'Ноябрь', 'Декабрь'],
        monthNamesShort: ['Янв', 'Фев', 'Мар', 'Апр', 'Май', 'Июн',
        'Июл', 'Авг', 'Сен', 'Окт', 'Ноя', 'Дек'],
        dayNames: ['воскресенье', 'понедельник', 'вторник', 'среда', 'четверг', 'пятница', 'суббота'],
        dayNamesShort: ['вск', 'пнд', 'втр', 'срд', 'чтв', 'птн', 'сбт'],
        dayNamesMin: ['Вс', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
        weekHeader: 'Не',
        dateFormat: 'dd/mm/yy',
        firstDay: 1,
        isRTL: false,
        showMonthAfterYear: false,
        yearSuffix: ''
    };
    $.datepicker.setDefaults($.datepicker.regional['ru']);
    $.validator.addMethod('date',
        function (value, element) {
            var ok = true;
            try {
                $.datepicker.parseDate('dd/mm/yy', value);
            }
            catch (err) {
                ok = false;
            }
            return ok;
        });
});

$(function () {
    $("#ListUsers").on("click", ".js-add-friend-button", function () {
        var data = $(this).attr("id");
        $.post("/FindUser/AddFriend", { id: data }, function () {
            $('#' + data).text("Добавлен");
            $('#' + data).prop('disabled', true);
        });
    });
});

$(document).ready(function () {
    $('#sendMessage').submit(function (event) {
        event.preventDefault();
        var data = $(this).serialize();
        var url = $(this).attr('action');
        $.post(url, data, function (responce) {
            $('#messageInput').val('');
            $('#formMessages').append(responce);
            $('#buttonSendMessage').prop('disabled', true);
            if ($('#noMessages').length > 0) {
                $('#noMessages').remove();
            }
        });
    });
});

$(document).ready(function () {
    $('#addComment').submit(function (event) {
        event.preventDefault();
        var data = $(this).serialize();
        var url = $(this).attr('action');
        $.post(url, data, function (responce) {
            $('#commentInput').val('');
            $('#formComments').append(responce);
            $('#buttonAddComment').prop('disabled', true);
            if ($('#noComments').length > 0) {
                $('#noComments').remove();
            }      
        });
    });
});

$(document).ready(function () {
    $('#addAnswer').submit(function (event) {
        event.preventDefault();
        var data = $(this).serialize();
        var url = $(this).attr('action');
        $.post(url, data, function (responce) {
            $('#answerInput').val('');
            $('#formAnswers').append(responce);
            $('#buttonAddAnswer').prop('disabled', true);
            if ($('#noAnswers').length > 0) {
                $('#noAnswers').remove();
            }
        });
    });
});

function LockedButton(thisInput, lockedButton) {
    if ($("#" + thisInput).val() === '') {
        $("#" + lockedButton).prop('disabled', true);
    } else {
        $("#" + lockedButton).prop('disabled', false);
    }
}

function AddMaterialClick() {
    var formData = new FormData();
    var file = document.getElementById("loadFile").files[0];

    var subjectId = $("#SubjectId").val();
    var name = $("#inputDescriptionMaterial").val();
    var typeLesson = $("#TypeLesson").val();

    formData.append("loadFile", file);
    formData.append("SubjectId", subjectId);
    formData.append("Name", name);
    formData.append("TypeLesson", typeLesson);

    $.ajax({
        type: "POST",
        url: '/Subjects/PartialViewMaterials',
        data: formData,
        dataType: 'html',
        contentType: false,
        processData: false,
        success: function (responce) {
            $('#inputDescriptionMaterial').val('');
            $('#buttonAddMaterial').prop('disabled', true);
            $('#listMaterials').replaceWith(responce);
        },
        error: function () {
            window.location.reload();
            alert("Error");
        }
    });
}