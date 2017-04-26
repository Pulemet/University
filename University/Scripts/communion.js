function OpenDialog(id) {
    $.get('/Communion/GetDialog',
        { id: id },
        function(data) {
            $('#currentDialog').replaceWith(data);
        });
}

function SendMessage() {
    event.preventDefault();
    var data = $('#sendMessage').serialize();
    var url = $('#sendMessage').attr('action');
    $.post(url, data, function (responce) {
        $('#messageInput').val('');
        $('#formMessages').append(responce);
        $('#buttonSendMessage').prop('disabled', true);
        var div = document.getElementById('formMessages');
        div.scrollTop = div.scrollHeight - div.clientHeight;
        if ($('#noMessages').length > 0) {
            $('#noMessages').remove();
        }
    });
}

function NewDialog() {
    $("#dialogsBar").hide();
    $("#newDialogBar").show();
}

function CreateDialog() {
    $("#newDialogBar").hide();
    $("#dialogsBar").show();
}

function ShowDialogs() {
    $("#newDialogBar").hide();
    $("#dialogsBar").show();
}