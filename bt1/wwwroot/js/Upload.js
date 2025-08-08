document.getElementById("buttonUploadProduct").addEventListener("click", function () {
    document.getElementById("overlay").style.display = "block";
    document.getElementById("PopupUpload").style.display = "block";
});

function displayFileName(input) {
    const fileName = input.files.length > 0 ? input.files[0].name : "Chưa chọn tệp nào";
    document.getElementById("fileNameDisplay").innerText = fileName;
}

document.getElementById("FormUpload").addEventListener("submit", async function (e) {
    e.preventDefault();

    const formData = new FormData(this);
    const response = await fetch('/MasterProduct/UploadExcel', {
        method: 'POST',
        body: formData
    });

    const result = await response.json();

    if (result.success) {
        alert("Tải file thành công!");
        hidePopup("PopupUpload");
        location.reload();
    } else {
        alert("Đã xảy ra lỗi:\n" + result.errors.join("\n"));
    }
});


