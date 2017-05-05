function OpenDialog(id) {
    $.get('/Communion/GetDialog',
        { id: id },
        function(data) {
            $('#formMessages').replaceWith(GetDialogHtml(data));
            $('#dialogId').val(data.Id);
            ScrollingDialog();
            ShowViewSendMessage();
        });
}

function SendMessage() {
    event.preventDefault();
    var data = $('#sendMessage').serialize();
    var url = $('#sendMessage').attr('action');
    $.post(url, data, function (responce) {
        ShowViewSendMessage();
        $('#formMessages').append(GetMessageHtml(responce));
        ScrollingDialog();
        if ($('#noMessages').length > 0) {
            $('#noMessages').remove();
        }
    });
}

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

function CreateDialog() {
    var inputId = "#NameConversation";
    var arr = $('input:checkbox:checked').map(function () { return this.value; }).get();
    if (arr.length !== 0) {
        if (arr.length === 1 && $(inputId).val() === '') {
            var id = arr[0];
            $.get('/Communion/GetViewDialogByUser',
                { id: id },
                function (data) {
                    if (data.IsNewDialog) {
                        $('#ListDialogs').append(GetPartialDialogHtml(data.Dialog.Id, data.Dialog.Name));
                    }
                    $('#formMessages').replaceWith(GetDialogHtml(data.Dialog));
                    $('#dialogId').val(data.Dialog.Id);
            });
        } else {
            var name = $("#NameConversation").val();
            var url = '/Communion/NewConversation';
            $.post(url, { listUsersId: arr, nameConversation: name }, function (data) {
                $('#ListDialogs').append(GetPartialDialogHtml(data.Id, data.Name));
                $('#formMessages').replaceWith(GetDialogHtml(data));
                $('#dialogId').val(data.Id);
            });
        }
        ScrollingDialog();
        ShowViewSendMessage();
        ShowDialogs();
    }
    ClearValuesInPageNewDialog(inputId);
}

function GetPartialDialogHtml(id, name) {
    var result = '<div id="' + id + '" ' + 'onclick="OpenDialog(' +
        id + ')"><a href="#" class="list-group-item">' +
        '<span class="glyphicon glyphicon-star-empty"></span>' +
        '<span class="name" style="min-width: 120px; display: inline-block;">' +
        name + '</span></a></div>';
    return result;
}

function GetDialogHtml(dialog) {
    $('#dialogName').text(dialog.Name);
    var messagesHtml = "";
    if (dialog.Messages.length === 0) {
        messagesHtml = '<div id="noMessages"><p>Список сообщений пуст</p></div>';
    } else {
        for (var index = 0, len = dialog.Messages.length; index < len; ++index) {
            messagesHtml += GetMessageHtml(dialog.Messages[index]);
        }
    }
    var result = '<div id="formMessages">' + messagesHtml + '</div>';
    return result;
}

function GetMessageHtml(message) {
    return '<div class="msg-wrap"><div class="media msg"><div class="media-body">' +
        '<small class="pull-right time"><i class="fa fa-clock-o">' + message.DateSend +
        '</i></small><h5 class="media-heading">' + message.SurName + ' ' + message.FirstName +
        '</h5><small>' + message.Text + '</small></div></div></div>';
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