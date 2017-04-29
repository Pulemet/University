function OpenDialog(id) {
    $.get('/Communion/GetDialog',
        { id: id },
        function(data) {
            $('#currentDialog').replaceWith(data);
            ScrollingDialog();
        });
}

function SendMessage() {
    event.preventDefault();
    var data = $('#sendMessage').serialize();
    var url = $('#sendMessage').attr('action');
    $.post(url, data, function (responce) {
        $('#messageInput').val('');
        $('#formMessages').append(responce);
        ScrollingDialog();
        $('#buttonSendMessage').prop('disabled', true);
        var div = document.getElementById('formMessages');
        div.scrollTop = div.scrollHeight - div.clientHeight;
        if ($('#noMessages').length > 0) {
            $('#noMessages').remove();
        }
    });
}

function ScrollingDialog() {
    var div = document.getElementById('formMessages');
    div.scrollTop = div.scrollHeight - div.clientHeight;
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
                function(data) {
                    $('#currentDialog').replaceWith(data);
                    ScrollingDialog();
            });
        } else {
            var name = $("#NameConversation").val();
            var url = '/Communion/NewConversation';
            $.post(url, { listUsersId: arr, nameConversation: name }, function (responce) {
                $('#ListDialogs').append(responce);
                var dialogId = $('#ListDialogs tr:last-child').attr('id');
                OpenDialog(dialogId);
            });
        }
        ShowDialogs();
    }
    ClearValuesInPageNewDialog(inputId);
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