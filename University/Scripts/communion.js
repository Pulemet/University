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
}

function ShowDialogs() {
    $("#newDialogBar").hide();
    $("#dialogsBar").show();
}