document.querySelectorAll(".btnEdit").forEach(function (item) {
    item.addEventListener("click", function () {
        const form = document.getElementById("FormSaleOutUpdate");

        form.reset();

        form.Id.value = item.dataset.id;
        form.ProductId.value = item.dataset.productid;
        form.CustomerPoNo.value = item.dataset.customerpono;

        const rawDate = item.dataset.date;
        if (rawDate && rawDate.length === 8) {
            const formattedDate = `${rawDate.slice(0, 4)}-${rawDate.slice(4, 6)}-${rawDate.slice(6, 8)}`;
            form.OrderDate.value = formattedDate;
        }

        form.CustomerName.value = item.dataset.customername;
        form.ProductCode.value = item.dataset.productcode;
        form.Price.value = item.dataset.price;
        form.Unit.value = item.dataset.unit;
        form.QuantityPerBox.value = item.dataset.quantityperbox;
        form.Quantity.value = item.dataset.quantity;

        form.action = '/SaleOut/Update';

        showPopup('PopupSaleOutUpdate');
    });
});

document.getElementById("FormSaleOutUpdate").addEventListener("submit", function (e) {
    e.preventDefault();

    const form = document.getElementById("FormSaleOutUpdate");

    if (!form.checkValidity()) {
        alert("Vui lòng điền đầy đủ các trường bắt buộc.");
        return;
    }

    const quantity = parseFloat(document.getElementById("Quantity").value);
    const quantityPerBox = parseFloat(document.getElementById("QuantityPerBox").value);
    const boxQuantity = (!isNaN(quantity) && !isNaN(quantityPerBox) && quantityPerBox !== 0)
        ? Math.ceil(quantity / quantityPerBox)
        : '';

    document.getElementById("BoxQuantity").value = boxQuantity;

    const formData = {
        Id: form.Id.value,
        CustomerPoNo: form.CustomerPoNo.value,
        OrderDate: parseInt(form.OrderDate.value.replace(/-/g, '')),
        CustomerName: form.CustomerName.value,
        ProductId: form.ProductId.value,
        Quantity: form.Quantity.value,
        QuantityPerBox: form.QuantityPerBox.value,
        BoxQuantity: Math.ceil(form.Quantity.value / form.QuantityPerBox.value),
        Price: form.Price.value,
        Amount: parseFloat(form.Price.value) * parseFloat(form.Quantity.value)
    };

    console.log(formData);

    fetch(form.action, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData)
    })
    .then(async response => {
        if (!response.ok) {
            const errorText = await response.text(); 
            console.error("Lỗi phía server:", errorText);
            throw new Error("Lỗi từ server: " + errorText);
        }

        return response.json();
    })
    .then(data => {
        alert('Sửa bản ghi thành công!');
        hidePopup('PopupSaleOutUpdate');
        window.location.reload();
    })
    .catch(error => {
        console.error('Lỗi:', error);
        alert(`Có lỗi xảy ra: ${error.message}`);
    });

});
