function ScrollingDialog() {
    var div = document.getElementById('formMessages');
    div.scrollTop = div.scrollHeight - div.clientHeight;
}

function ShowViewSendMessage() {
    $('#messageInput').val('');
    $('#formSendMessage').show();
    $('#buttonSendMessage').prop('disabled', true);
}

function NewDialog() {
    $("#dialogsBar").hide();
    $("#addMembersToDialogBar").hide();
    $("#newDialogBar").show();
    var id = $("#userId").val();
    if (id !== "") {
        console.log(id);
        $("#" + id).prop("checked", true);
    }
}

function AddMembersToDialogBar() {
    $("#dialogsBar").hide();
    $("#newDialogBar").hide();
    $("#addMembersToDialogBar").show();
}

function GetPartialDialogHtml(id, name) {
    var result = '<div id="' + id + '" ' + 'class="current-dialog"><a href="#" class="list-group-item">' +
        '<span class="name" style="min-width: 120px; display: inline-block; height: 30px; ">' +
        name + '</span></a></div>';
    return result;
}

function GetDialogHtml(dialog) {
    $('#dialogName').text(dialog.Name);
    var messagesHtml = "";
    if (dialog.Messages.length === 0) {
        messagesHtml = '<div id="noMessages" style="margin-top: 30%; margin-left: 30%;"><p>Список сообщений пуст</p></div>';
    } else {
        for (var index = 0, len = dialog.Messages.length; index < len; ++index) {
            messagesHtml += GetMessageHtml(dialog.Messages[index]);
        }
    }
    var result = '<div id="formMessages">' + messagesHtml + '</div>';
    return result;
}

function GetMessageHtml(message) {
    return '<div class="msg-wrap" style="border-bottom: 1px solid #4c4444; padding: 5px; background-color: #fff; border-radius: 3px"><div class="media msg">' +
            '<small class="pull-right time"><i class="fa fa-clock-o" style="color: #6E6E6E;"> ' + message.DateSend +
            '</i></small><h5 class="media-heading" style="color: #003bb3;"><b>' + message.SurName + ' ' + message.FirstName +
            '</b></h5><small>' + message.Text + '</small></div></div>';
}

function ClearValuesInPageDialog(inputId) {
    $("input[type=checkbox]", "#formBoxFriends").prop('checked', false);
    $("input[type=checkbox]", "#formBox").prop('checked', false);
    $(inputId).val('');
    $("#buttonCreateDialog").prop('disabled', true);
    $("#buttonAddMembers").prop('disabled', true);
}

function ShowDialogs() {
    $("#newDialogBar").hide();
    $("#addMembersToDialogBar").hide();
    ClearValuesInPageDialog("#NameConversation");
    $("#dialogsBar").show();
}

$(function () {
    // Ссылка на автоматически-сгенерированный прокси хаба
    var chat = $.connection.chatHub;
    // Объявление функции, которая хаб вызывает при получении сообщений
    chat.client.sendMessage = function (message) {
        // Добавление сообщений на веб-страницу 
        $('#formMessages').append(GetMessageHtml(message));
        ScrollingDialog();
        if ($('#noMessages').length > 0) {
            $('#noMessages').remove();
        }
    };

    chat.client.addMembers = function (dialog) {
        $('#membersDialog').replaceWith(GetDialogMembersHtml(dialog));
    }

    // Открываем соединение
    $.connection.hub.start().done(function () {

        $('#sendMessage').submit(function (event) {
            event.preventDefault();
            var data = $('#sendMessage').serialize();
            var url = $('#sendMessage').attr('action');
            $.post(url, data, function (data) {
                ShowViewSendMessage();
                chat.server.send($('#dialogId').val(), data);
            });
        });

        $('#ListDialogs').on('click', '.current-dialog', (function () {
            console.log('here1');
            var id = $(this).attr('id');
            if ($('#dialogId').val() !== "") {
                chat.server.onDisconnected($('#dialogId').val());
            }
            $.get('/Communion/GetDialog',
                { id: id },
                function (data) {
                    DialogMembers(data);
                    if (data.IsConversation) {
                        $('#membersDialog').replaceWith(GetDialogMembersHtml(data));
                    } else {
                        $("#formBox").html("");
                    }
                    $('#formMessages').replaceWith(GetDialogHtml(data));
                    $('#dialogId').val(data.Id);
                    ScrollingDialog();
                    ShowViewSendMessage();
                    chat.server.connect(data.Id);
                });
        }));

        $('#buttonCreateDialog').click(function () {
            if ($('#dialogId').val() !== "") {
                chat.server.onDisconnected($('#dialogId').val());
            }
            var inputId = "#NameConversation";
            var arr = $('input:checkbox:checked', "#formBoxFriends").map(function() { return this.value; }).get();
            if (arr.length !== 0) {
                if (arr.length === 1 && $(inputId).val() === '') {
                    var id = arr[0];
                    $.get('/Communion/GetViewDialogByUser',
                        { id: id },
                        function(data) {
                            if (data.IsNewDialog) {
                                $('#ListDialogs').append(GetPartialDialogHtml(data.Dialog.Id, data.Dialog.Name));
                            }
                            DialogMembers(data.Dialog);
                            $("#formBox").html("");
                            $('#formMessages').replaceWith(GetDialogHtml(data.Dialog));
                            $('#dialogId').val(data.Dialog.Id);
                            ScrollingDialog();
                            chat.server.connect(data.Dialog.Id);
                        });
                } else {
                    var name = $("#NameConversation").val();
                    var url = '/Communion/NewConversation';
                    $.post(url, { listUsersId: arr, nameConversation: name }, function (data) {
                        DialogMembers(data);
                        $('#membersDialog').replaceWith(GetDialogMembersHtml(data));
                        $('#ListDialogs').append(GetPartialDialogHtml(data.Id, data.Name));
                        $('#formMessages').replaceWith(GetDialogHtml(data));
                        $('#dialogId').val(data.Id);
                        chat.server.connect(data.Id);
                    });
                }
                ShowViewSendMessage();
            }
            ShowDialogs();
        });

        $('#buttonAddMembers').click(function () {
            var dualogId = $('#dialogId').val();
            var arr = $('input:checkbox:checked', "#formBox").map(function () { return this.value; }).get();
            if (arr.length !== 0) {
                var url = '/Communion/AddUsersToDialog';
                $.post(url, { listUsersId: arr, dialogId: dualogId }, function (data) {
                    DialogMembers(data);
                    chat.server.addMembers($('#dialogId').val(), data);
                });
            }
            ShowDialogs();
        });

    });
});

function DialogMembers(dialog) {
    if (dialog.IsConversation) {
        $("#btnShowMembersToDialog").show();
        $("#btnAddMembersToDialog").show();
        $("#buttonShowFormAddMembers").attr('onclick', 'AddMembersToDialogBar()');
        $("#userId").val("");
    } else {
        $("#btnShowMembersToDialog").hide();
        $("#btnAddMembersToDialog").show();
        $("#buttonShowFormAddMembers").attr('onclick', 'NewDialog()');
        var id = dialog.Members[0].Id === userId ? dialog.Members[1].Id : dialog.Members[0].Id;
        $("#userId").val(id);
    }
}

function GetDialogMembersHtml(dialog) {
    $('#dialogName-modal').text('Участники диалога "' + dialog.Name + '"');
    var membersHtml = "";
    var arrFriends = new Array();

    for (var index = 0, len = dialog.Members.length; index < len; ++index) {
        if (dialog.Members[index].Id !== userId) {
            membersHtml += GetDialogMemberHtml(dialog.Members[index]);
        }   
    }

    for (var i = 0, len1 = arrayListFrieds.length; i < len1; ++i) {
        var isMember = false;
        for (var j = 0, len2 = dialog.Members.length; j < len2; ++j) {
            if (arrayListFrieds[i].Id === dialog.Members[j].Id || arrayListFrieds[i].Id === userId) {
                isMember = true;
                break;
            }
        }
        if (!isMember) {
            arrFriends.push(arrayListFrieds[i]);
        }
    }

    $("#formBox").html(GetFormListFriendsInDialog(arrFriends));
    return '<div class="modal-body" id="membersDialog">' + membersHtml + '</div>';
}

function GetFormListFriendsInDialog(members) {
    var html = '<label class="cols-md-12 control-label" for="" style="color: #0094ff; margin-top: 4px;">Выберите участников:</label>';
    for (var index = 0, len = members.length; index < len; ++index) {
        html += GetFriendInDialogHtml(members[index]);
    }
    return html;
}

function GetFriendInDialogHtml(member) {
    return '<div class="col-md-12 selectedUsers"><input type="checkbox" value="' + member.Id +
        '" id="' + member.Id + '"><label for="' +
        member.Id + '">' + member.SurName + ' ' + member.FirstName + '</label><br /></div>';
}

function GetDialogMemberHtml(member) {
    return '<p><a href="/Home/UserPage/' + member.Id +
           '"> ' + member.SurName + ' ' + member.FirstName + ' </a></p>';
}

var arrayListFrieds;
var userId;

function initArrayListFriendsForDialogs(arr, id) {
    arrayListFrieds = arr;
    userId = id;
}

var IsChecked = function () {
    var n = $('input:checked', "#formBoxFriends").length;
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

var IsCheckedFormAddToDialogs = function () {
    var n = $('input:checked', "#formBox").length;
    var buttonId = "#buttonAddMembers";
    if (n !== 0) {
        $(buttonId).prop('disabled', false);
    } else {
        $(buttonId).prop('disabled', true);
    }
};

$(document).ready(function () {
    $("#formBoxFriends").on('click', 'input[type=checkbox]', IsChecked);
});

$(document).ready(function () {
    $("#formBox").on('click', 'input[type=checkbox]', IsCheckedFormAddToDialogs);
});