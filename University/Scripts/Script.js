function AddFriend(id, buttonId) {
    $.post("/Home/AddFriend", { id: id }, function () {
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
            ScrollingForm("formComments");
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
            ScrollingForm("formAnswers");
            $('#buttonAddAnswer').prop('disabled', true);
            if ($('#noAnswers').length > 0) {
                $('#noAnswers').remove();
            }
        });
    });
});

$(document).ready(function () {
    $('#addReview').submit(function (event) {
        event.preventDefault();
        var data = $(this).serialize();
        var url = $(this).attr('action');
        $.post(url, data, function (responce) {
            $('#reviewInput').val('');
            $('#formReviews').append(responce);
            ScrollingForm("formReviews");
            $('#buttonAddReview').prop('disabled', true);
            if ($('#noReviews').length > 0) {
                $('#noReviews').remove();
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
        url: '/Subjects/AddMaterial',
        data: formData,
        dataType: 'html',
        contentType: false,
        processData: false,
        success: function (responce) {
            if (typeLesson === "Lecture") {
                $("#lecture-content").append(responce);
                LectureContentShow();
                ScrollingForm("lecture-content");
            } else {
                $("#practical-content").append(responce);
                PracticalContentShow();
                ScrollingForm("practical-content");
            }
            CloseFormAddMaterial();
        },
        error: function () {
            window.location.reload();
            alert("Error");
        }
    });
}

var userNames;
var userName;
var Friends;
var Surnames;
var FirstNames;
var SubjectFullNames;
var SubjectNames;
var Subjects;

function initArraySubjects(arr) {
    Subjects = arr;
    SubjectFullNames = new Array();
    SubjectNames = new Array();
    for (var index = 0, len = Subjects.length; index < len; ++index) {
        SubjectFullNames.push(Subjects[index].NameFull.toLowerCase());
        SubjectNames.push(Subjects[index].NameAbridgment.toLowerCase());
    }
}

function initArrayInExternFile(arr) {
    userNames = arr;
    Surnames = new Array();
    FirstNames = new Array();
    for (var index = 0, len = userNames.length; index < len; ++index) {
        var temp = userNames[index].split(" ");
        Surnames.push(temp[0].toLowerCase());
        FirstNames.push(temp[1].toLowerCase());
    }
}

function initUserName(name) {
    userName = name;
}

function initArrayUsers(arr) {
    Friends = arr;
    Surnames = new Array();
    FirstNames = new Array();
    for (var index = 0, len = Friends.length; index < len; ++index) {
        Surnames.push(Friends[index].SurName.toLowerCase());
        FirstNames.push(Friends[index].FirstName.toLowerCase());
    }
}

function NotSearchResult(name) {
    return '<div style="text-align: center; color: black; padding-top: 20%; "><p> Ниодного ' + name + ' не найдено';
}

function SearchSubject() {
    var inputSearch = $("#input-search").val();
    var data = new Array();
    var dataHtml = "";
    if (inputSearch.length > 0 && inputSearch.length < 50) {
        for (var index = 0, len = Subjects.length; index < len; ++index) {
            var loverInputSearch = inputSearch.toLowerCase();
            if (SubjectFullNames[index].substring(0, inputSearch.length) === loverInputSearch
                || SubjectNames[index].substring(0, inputSearch.length) === loverInputSearch) {
                data.push(Subjects[index]);
            }
        }
        if (data.length !== 0) {
            dataHtml = GetViewSubjects(data);
            $('#listSubjects').replaceWith(dataHtml);
        } else {
            $('#listSubjects').replaceWith('<div class="col-md-7" id="listSubjects">' + NotSearchResult("предмета") + '</div>');
        }
    } else {
        dataHtml = GetViewSubjects(Subjects);
        $('#listSubjects').replaceWith(dataHtml);
    }

}

function SearchFriend() {
    var inputSearch = $("#input-search").val();
    var data = new Array();
    var dataHtml = "";
    if (inputSearch.length > 0 && inputSearch.length < 50) {
        // Делаем запрос в обработчик в котором будет происходить поиск.

        for (var index = 0, len = Friends.length; index < len; ++index) {
            var loverInputSearch = inputSearch.toLowerCase();
            if (Surnames[index].substring(0, inputSearch.length) === loverInputSearch
                || FirstNames[index].substring(0, inputSearch.length) === loverInputSearch) {
                data.push(Friends[index]);
            }
        }
        if (data.length !== 0) {
            dataHtml = GetViewFriends(data);
            $('#listUsers').replaceWith(dataHtml);
        } else {
            $('#listUsers').replaceWith('<div id="listUsers">' + NotSearchResult("друга") + '</div>');
        }
    } else {
        dataHtml = GetViewFriends(Friends);
        $('#listUsers').replaceWith(dataHtml);
    }

}

function GetViewSubjects(subjects) {
    var data = '<div class="col-md-7" id="listSubjects">';
    for (var index = 0, len = subjects.length; index < len; ++index) {
        data += GetViewSubject(subjects[index]);
    }
    data = data + '</div>';
    return data;
}

function GetViewSubject(subject) {
    var id = subject.Id;
    return '<div class="panel panel-default"><div class="panel-heading resume-heading"><p>' +
        '<a href="/Teachers/Index/' + id + '"> Преподаватели </a></p><h4> ' +
        subject.NameFull + '</h4><p><a href="/Subjects/MaterialsSubject/' + id +
        '"> Перейти к материалам </a></p>' +
        '<p><a href="/Questions/Questions/' + id + '"> Перейти к вопросам </a></p></div></div>';
}

function GetViewFriends(friends) {
    var data = '<div id="listUsers">';
    for (var index = 0, len = friends.length; index < len; ++index) {
        data += GetViewFriend(friends[index]);
    }
    data = data + '</div>';
    return data;
}

function GetViewFriend(friend) {
    var role;
    if (friend.UserRole === "student") {
        role = "Студент";
    } else {
        role = "Преподаватель";
    }
    return '<div class="panel panel-default">' +
        '<div class="panel-heading resume-heading"><div class="row">' + '<div class="col-md-4 col-lg-4" align="center">' +
        '<img src="' + friend.Photo + '" class="circular-square-users"></div>' +
        '<div class="col-sm-6 col-md-8"><h4><i class="fa fa-university fa">' +
        '<a href="/Home/UserPage/' + friend.Id + '"> ' + friend.SurName + ' ' + friend.FirstName + ' </a></i>' +
        '</h4><p><i class="glyphicon glyphicon-user"> ' + role + ' </i><br/>' +
        '<i class="glyphicon glyphicon-envelope"> ' + friend.Email + ' </i><br/></p>' +
        '<div class="btn-group"><a class="btn btn-primary" onclick="ShowFormSendMessageD("' + friend.Id +
        '", "' + friend.SurName + ' ' + friend.FirstName + '")"' +
        'id="show-button"><i class="fa fa-envelope fa"> Написать сообщение </i></a></div>' +
        '</div></div></div></div>';
}

function SearchUser() {
    var inputSearch = $("#input-search").val();
    // Проверяем поисковое значение. Если оно больше или ровняется Трём, то всё нормально и также если меньше 50 символов.
    var data = "";
    if (inputSearch.length > 0 && inputSearch.length < 50) {
        // Делаем запрос в обработчик в котором будет происходить поиск.

        for (var index = 0, len = userNames.length; index < len; ++index) {
            var loverInputSearch = inputSearch.toLowerCase();
            if (Surnames[index].substring(0, inputSearch.length) === loverInputSearch
                || FirstNames[index].substring(0, inputSearch.length) === loverInputSearch) {
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
        $.get("/Home/SearchUser", { name: userName }, function (responce) {
            $('#input-search').val('');
            $("#block-search-result").hide();
            $('#listUsers').replaceWith(responce);
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

function ShowFormSendMessage(id) {
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

$(function () {
    $('#lecture-form-link').click(function (e) {
        LectureContentShow();
        e.preventDefault();
    });
    $('#practical-form-link').click(function (e) {
        PracticalContentShow();
        e.preventDefault();
    });

});

function LectureContentShow() {
    $("#lecture-content").show();
    $("#practical-content").hide();
    $("#lecture-content").delay(100).fadeIn(100);
    $("#practical-content").fadeOut(100);
    $('#practical-form-link').removeClass('active');
    $('#lecture-form-link').addClass('active');
}

function PracticalContentShow() {
    $("#lecture-content").hide();
    $("#practical-content").show();
    $("#practical-content").delay(100).fadeIn(100);
    $("#lecture-content").fadeOut(100);
    $('#lecture-form-link').removeClass('active');
    $('#practical-form-link').addClass('active');
}

function ScrollingForm(id) {
    var div = document.getElementById(id);
    div.scrollTop = div.scrollHeight - div.clientHeight;
}

function CloseFormAddMaterial() {
    $('#buttonAddMaterial').prop('disabled', true);
    $('#inputDescriptionMaterial').val('');
    $('#loadFile').val('');
    $("#filename").val('');
}

$(document).ready(function () {
    $(".file-upload input[type=file]").change(function () {
        var filename = $(this).val().replace(/.*\\/, "");
        $("#filename").val(filename);
    });
});