document.getElementById("buttonAdd").addEventListener("click", function () {
    const form = document.getElementById("Form");
    form.reset();
    form.action = '/MasterProduct/Insert';
    document.getElementById("submitButton").textContent = "Thêm";
    form.ProductCode.readOnly = false;
    form.ProductName.readOnly = false;
    document.getElementById("overlay").style.display = "block";
    document.getElementById("Popup").style.display = "block";
});
