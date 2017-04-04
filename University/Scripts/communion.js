function AddMaterialClick() {
    var formData = new FormData();
    var file = document.getElementById("loadFile").files[0];

    var subjectId = $("#SubjectId").val();
    var name = $("#Name").val();
    var typeLesson = $("#TypeLesson").val();

    formData.append("loadFile", file);
    formData.append("SubjectId", subjectId);
    formData.append("Name", name);
    formData.append("TypeLesson", typeLesson);

    $.ajax({
        type: "POST",
        url: '/Subjects/PartialViewMaterials',
        data: formData,
        dataType: 'html',
        contentType: false,
        processData: false,
        success: function (responce) {
            $('#listMaterials').replaceWith(responce);
        },
        error: function () {
            window.location.reload();
            alert("Error");
        }
    });
}