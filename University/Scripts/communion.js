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
        if ($('#noMessages').length > 0) {
            $('#noMessages').remove();
        }
    });
}