document.querySelectorAll(".btnEdit").forEach(function (item) {
    item.addEventListener("click", function () {
        const form = document.getElementById("Form");

        form.Id.value = item.dataset.id;
        form.ProductCode.value = item.dataset.code;
        form.ProductName.value = item.dataset.name;
        form.Unit.value = item.dataset.unit;
        form.Specification.value = item.dataset.spec;
        form.QuantityPerBox.value = item.dataset.quantity;
        form.ProductWeight.value = item.dataset.weight;

        form.ProductCode.readOnly = true;
        form.ProductName.readOnly = true;
        form.action = '/MasterProduct/Update';
        document.getElementById("submitButton").textContent = "Sửa";

        document.getElementById("overlay").style.display = "block";
        document.getElementById("Popup").style.display = "block";
    });
});