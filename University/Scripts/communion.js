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
    $("#newDialogBar").hide();
    $("#dialogsBar").show();
    var arr = $('input:checkbox:checked').map(function () { return this.value; }).get();
    if (arr.length !== 0) {
        var name = $("#NameConversation").val();
        var url = '/Communion/NewConversation';
        $("input[type=checkbox]").prop('checked', false);
        $.post(url, { listUsersId: arr, nameConversation: name }, function (responce) {
            $('#ListDialogs').append(responce);
            var dialogId = $('#ListDialogs tr:last-child').attr('id');
            OpenDialog(dialogId);
            ShowDialogs();
        });
    }   
}

function ShowDialogs() {
    $("#newDialogBar").hide();
    $("#dialogsBar").show();
}