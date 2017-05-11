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

$(document).ready(function () {
    $('#addReview').submit(function (event) {
        event.preventDefault();
        var data = $(this).serialize();
        var url = $(this).attr('action');
        $.post(url, data, function (responce) {
            $('#reviewInput').val('');
            $('#formReviews').append(responce);
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

var IsChecked = function () {
    var n = $('input:checked').length;
    var inputId = "#NameConversation";
    var buttonId = "#buttonCreateDialog";
    if (n !== 0 && $(inputId).val() !== '') {
        $(buttonId).prop('disabled', false);
    } else {
        if (n === 1) {
            $(buttonId).prop('disabled', false);
        } else {
            $(buttonId).prop('disabled', true);
        } 
    }
};

$(document).ready(function() {
    $('input[type=checkbox]').on('click', IsChecked);
});

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
            } else {
                $("#practical-content").append(responce);
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
    return '<div style="text-align: center; color: black;"><p> Ниодного ' + name + ' не найдено';
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
            $('#ListSubjects').replaceWith(dataHtml);
        } else {
            $('#ListSubjects').replaceWith('<div class="col-md-12" id="ListSubjects">' + NotSearchResult("предмета") + '</div>');
        }
    } else {
        dataHtml = GetViewSubjects(Subjects);
        $('#ListSubjects').replaceWith(dataHtml);
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
            $('#ListUsers').replaceWith(dataHtml);
        } else {
            $('#ListUsers').replaceWith('<div id="ListUsers">' + NotSearchResult("друга") + '</div>');
        }
    } else {
        dataHtml = GetViewFriends(Friends);
        $('#ListUsers').replaceWith(dataHtml);
    }
    
}

function GetViewSubjects(subjects) {
    var data = '<div class="col-md-12" id="ListSubjects">';
    for (var index = 0, len = subjects.length; index < len; ++index) {
        data += GetViewSubject(subjects[index]);
    }
    data = data + '</div>';
    return data;
}

function GetViewSubject(subject) {
    var id = subject.Id;
    return '<div class="blockquote-box clearfix col-xs-6"><p>' +
        '<a href="/Teachers/Index/' + id + '"> Преподаватели </a></p><h4> ' +
        subject.NameFull + '</h4><p><a href="/Subjects/MaterialsSubject/' + id +
        '"> Перейти к материалам </a></p>' +
        '<p><a href="/Questions/Questions/' + id + '"> Перейти к вопросам </a></p></div>';
}

function GetViewFriends(friends) {
    var data = '<div id="ListUsers">';
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
    return '<div class="col-md-10"><div class="panel panel-default">' +
        '<div class="panel-body"><div class="row"><div class="col-md-4 col-lg-4" align="center">' +
        '<img src="' + friend.Photo + '" class="circular-square-users"></div>' +
        '<div class="col-sm-6 col-md-8"><h4><i class="fa fa-university fa">' +
        '<a href="/Home/UserPage/' + friend.Id + '"> ' + friend.SurName + ' ' + friend.FirstName +' </a></i>' +
        '</h4><p><i class="glyphicon glyphicon-user"> ' + role + ' </i><br/>' +
        '<i class="glyphicon glyphicon-envelope"> ' + friend.Email + ' </i><br/></p>' +
        '<div class="btn-group"><a class="btn btn-primary" onclick="ShowFormSendMessageD("' + friend.Id +
        '", "' + friend.SurName + ' ' + friend.FirstName + '")"' +
        'id="show-button"><i class="fa fa-envelope fa"> Написать сообщение </i></a></div>' +
        '</div></div></div></div></div>';
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

$(function () {
    $('#lecture-form-link').click(function (e) {
        $("#lecture-content").show();
        $("#practical-content").hide();
        $("#lecture-content").delay(100).fadeIn(100);
        $("#practical-content").fadeOut(100);
        $('#practical-form-link').removeClass('active');
        $(this).addClass('active');
        e.preventDefault();
    });
    $('#practical-form-link').click(function (e) {
        $("#lecture-content").hide();
        $("#practical-content").show();
        $("#practical-content").delay(100).fadeIn(100);
        $("#lecture-content").fadeOut(100);
        $('#lecture-form-link').removeClass('active');
        $(this).addClass('active');
        e.preventDefault();
    });

});

function NewMaterialClick() {
    $("#formAddMaterial").show();
    $("#buttonNewMaterial").hide();
}

function CloseFormAddMaterial() {
    $('#buttonAddMaterial').prop('disabled', true);
    $('#inputDescriptionMaterial').val('');
    $('#loadFile').val('');
    $("#formAddMaterial").hide();
    $("#buttonNewMaterial").show();
}

/**
 Function for check-box. 
 */
$(function () {
    $('.list-group.checked-list-box .list-group-item').each(function () {

        // Settings
        var $widget = $(this),
            $checkbox = $('<input type="checkbox" class="hidden" />'),
            color = ($widget.data('color') ? $widget.data('color') : "primary"),
            style = ($widget.data('style') == "button" ? "btn-" : "list-group-item-"),
            settings = {
                on: {
                    icon: 'glyphicon glyphicon-check'
                },
                off: {
                    icon: 'glyphicon glyphicon-unchecked'
                }
            };

        $widget.css('cursor', 'pointer')
        $widget.append($checkbox);

        // Event Handlers
        $widget.on('click', function () {
            $checkbox.prop('checked', !$checkbox.is(':checked'));
            $checkbox.triggerHandler('change');
            updateDisplay();
        });
        $checkbox.on('change', function () {
            updateDisplay();
        });


        // Actions
        function updateDisplay() {
            var isChecked = $checkbox.is(':checked');

            // Set the button's state
            $widget.data('state', (isChecked) ? "on" : "off");

            // Set the button's icon
            $widget.find('.state-icon')
                .removeClass()
                .addClass('state-icon ' + settings[$widget.data('state')].icon);

            // Update the button's color
            if (isChecked) {
                $widget.addClass(style + color + ' active');
            } else {
                $widget.removeClass(style + color + ' active');
            }
        }

        // Initialization
        function init() {

            if ($widget.data('checked') == true) {
                $checkbox.prop('checked', !$checkbox.is(':checked'));
            }

            updateDisplay();

            // Inject the icon if applicable
            if ($widget.find('.state-icon').length == 0) {
                $widget.prepend('<span class="state-icon ' + settings[$widget.data('state')].icon + '"></span>');
            }
        }
        init();
    });

    $('#get-checked-data').on('click', function (event) {
        event.preventDefault();
        var checkedItems = {}, counter = 0;
        $("#check-list-box li.active").each(function (idx, li) {
            checkedItems[counter] = $(li).text();
            counter++;
        });
        $('#display-json').html(JSON.stringify(checkedItems, null, '\t'));
    });
});