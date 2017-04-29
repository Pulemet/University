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

function AddFriend(id, buttonId) {
    $.post("/FindUser/AddFriend", { id: id }, function () {
        $("#" + buttonId).text("Добавлен");
        $("#" + buttonId).prop('disabled', true);
    });
}

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

var userNames;
var userName;

function initArrayInExternFile(arr) {
    userNames = arr;
}

function initUserName(name) {
    userName = name;
}

function SearchUser() {
    var inputSearch = $("#input-search").val();
    // Проверяем поисковое значение. Если оно больше или ровняется Трём, то всё нормально и также если меньше 50 символов.
    var data = "";
    if (inputSearch.length >= 3 && inputSearch.length < 50) {
        // Делаем запрос в обработчик в котором будет происходить поиск.
        
        for (var index = 0, len = userNames.length; index < len; ++index)
        {
            if (userNames[index].toLowerCase().substring(0, inputSearch.length) === inputSearch.toLowerCase()) {
                data = data + '<li class="add-li"><div class="block-title-price" >' + '<a href="#">' + userNames[index] + "</a></div></li>";
            }
        }
    } 
    if (data !== "") {
        $("#block-search-result").show(); // Показываем блок с результатом.
        $("#list-search-result").html(data); // Добавляем в список результат поиска.
    } else {
        // Если ничего не найдено, то скрываем выпадающий список.
        $("#block-search-result").hide();
    }
}

$(document).ready(function () {
    $("#list-search-result").on("click", ".add-li", function () {
        var userName = $(this).text();
        $.get("/FindUser/SearchUser", { name: userName }, function (responce) {
            $('#input-search').val('');
            $("#block-search-result").hide();
            $('#ListUsers').replaceWith(responce);
        });
    });
});

// ---------------------------------------------------------------------------------------
// Показать полупрозрачный DIV, затеняющий всю страницу
// (а форма будет не в нем, а рядом с ним, чтобы не полупрозрачная)
function showCover() {
    var coverDiv = document.createElement('div');
    coverDiv.id = 'cover-div';
    document.body.appendChild(coverDiv);
}

function hideCover() {
    document.body.removeChild(document.getElementById('cover-div'));
}

function showPrompt(text, callback) {
    showCover();
    var form = document.getElementById('prompt-form');
    var container = document.getElementById('prompt-form-container');
    form.elements.text.value = '';

    function complete(value) {
        hideCover();
        container.style.display = 'none';
        document.onkeydown = null;
        callback(value);
    }

    form.onsubmit = function () {
        var value = form.elements.text.value;
        if (value === '') return false; // игнорировать пустой submit

        complete(value);
        return false;
    };

    form.elements.cancel.onclick = function () {
        complete(null);
    };

    document.onkeydown = function (e) {
        if (e.keyCode === 27) { // escape
            complete(null);
        }
    };

    var lastElem = form.elements[form.elements.length - 1];
    var firstElem = form.elements[0];

    lastElem.onkeydown = function (e) {
        if (e.keyCode === 9 && !e.shiftKey) {
            firstElem.focus();
            return false;
        }
    };

    firstElem.onkeydown = function (e) {
        if (e.keyCode === 9 && e.shiftKey) {
            lastElem.focus();
            return false;
        }
    };

    container.style.display = 'block';
    form.elements.text.focus();
}

function ShowFormSendMessageD(id, name) {
    userName = name;
    ShowFormSendMessage(id);
}

function ShowFormSendMessage (id) {
    showPrompt(id, function (value) {
        if (value !== null) {
            SendMessage(id, value); 
        }
    });
};

function SendMessage(id, value) {
    var url = "/Communion/SendMessage";
    $.post(url, { id: id, text: value }, function () {
        alert("Отправлено " + userName);
    });
}
// ---------------------------------------------------------------------------------------