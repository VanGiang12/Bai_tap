document.getElementById("buttonUploadSaleOut").addEventListener("click", function () {
    showPopup("PopupUploadSaleOut");
});

function displayFileName(input) {
    const fileName = input.files.length > 0 ? input.files[0].name : "Chưa chọn tệp nào";
    document.getElementById("fileNameDisplay").innerText = fileName;
}

document.getElementById("FormUploadSaleOut").addEventListener("submit", async function (e) {
    e.preventDefault();

    const formData = new FormData(this);
    const response = await fetch('/SaleOut/UploadExcel', {
        method: 'POST',
        body: formData
    });

    const result = await response.json();

    if (result.success) {
        alert("Tải file thành công!");
        hidePopup("PopupUploadSaleOut");
        location.reload();
    } else {
        alert("Đã xảy ra lỗi:\n" + result.errors.join("\n"));
    }
});


