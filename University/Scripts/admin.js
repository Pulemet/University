function DeleteUser(userId) {
    $.post("/Account/DeleteUser", { id: userId }, function () {
        $("#" + userId).remove();
    });
}

function SubmitUser(userId) {
    $.post("/Account/SubmitUser", { id: userId }, function (responce) {
        alert(responce);
        document.location.href = "/Account/ConfirmRegistration";
    });
}