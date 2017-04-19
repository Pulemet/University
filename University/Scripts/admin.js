function DeleteUser(userId) {
    $.post("/Account/DeleteUser", { id: userId }, function (responce) {
        alert(responce);
        if (document.location.href.indexOf("/Account/ConfirmRegistration") > -1) {
            $("#" + userId).remove();
        } else {
            document.location.href = "/Account/ConfirmRegistration";
        }
    });
}

function SubmitUser(userId) {
    $.post("/Account/SubmitUser", { id: userId }, function (responce) {
        alert(responce);
        document.location.href = "/Account/ConfirmRegistration";
    });
}