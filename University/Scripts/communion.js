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
    $("#newDialogBar").show();
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

function ClearValuesInPageNewDialog(inputId) {
    $("input[type=checkbox]").prop('checked', false);
    $(inputId).val('');
    $("#buttonCreateDialog").prop('disabled', true);
}

function ShowDialogs() {
    $("#newDialogBar").hide();
    $("#dialogsBar").show();
}

var openDialog;

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

    // Открываем соединение
    $.connection.hub.start().done(function () {

        $('#sendMessage').submit(function (event) {
            event.preventDefault();
            var data = $('#sendMessage').serialize();
            var url = $('#sendMessage').attr('action');
            $.post(url, data, function (responce) {
                ShowViewSendMessage();
                chat.server.send($('#dialogId').val(), responce);
            });
        });

        $('#ListDialogs').on('click', '.current-dialog', (function () {
            var id = $(this).attr('id');
            if ($('#dialogId').val() !== "") {
                chat.server.onDisconnected($('#dialogId').val());
            }
            $.get('/Communion/GetDialog',
                { id: id },
                function(data) {
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
            var arr = $('input:checkbox:checked').map(function() { return this.value; }).get();
            if (arr.length !== 0) {
                if (arr.length === 1 && $(inputId).val() === '') {
                    var id = arr[0];
                    $.get('/Communion/GetViewDialogByUser',
                        { id: id },
                        function(data) {
                            if (data.IsNewDialog) {
                                $('#ListDialogs').append(GetPartialDialogHtml(data.Dialog.Id, data.Dialog.Name));
                            }
                            $('#formMessages').replaceWith(GetDialogHtml(data.Dialog));
                            $('#dialogId').val(data.Dialog.Id);
                            ScrollingDialog();
                            chat.server.connect(data.Dialog.Id);
                        });
                } else {
                    var name = $("#NameConversation").val();
                    var url = '/Communion/NewConversation';
                    $.post(url, { listUsersId: arr, nameConversation: name }, function(data) {
                        $('#ListDialogs').append(GetPartialDialogHtml(data.Id, data.Name));
                        $('#formMessages').replaceWith(GetDialogHtml(data));
                        $('#dialogId').val(data.Id);
                        chat.server.connect(data.Id);
                    });
                }
                ShowViewSendMessage();
                ShowDialogs();
            }
            ClearValuesInPageNewDialog(inputId);
        });

    });
});