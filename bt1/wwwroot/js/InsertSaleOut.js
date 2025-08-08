const form = document.getElementById("FormSaleOutInsert");
const submitButton = document.getElementById("submitButton");
const overlay = document.getElementById("overlay");
const orderDateInput = document.getElementById("OrderDate");
const quantityInput = document.getElementById("Quantity");
const quantityPerBoxInput = document.getElementById("QuantityPerBox");
const boxQuantityInput = document.getElementById("BoxQuantity");
const customerPoNoInput = document.getElementById("CustomerPoNo");

function onProductCodeChange() {
    const selectedCode = document.getElementById("ProductCode").value;
    const product = allProducts.find(p => p.ProductCode === selectedCode);

    if (product) {
        document.getElementById("ProductId").value = product.Id;
        document.getElementById("ProductName").value = product.ProductName || '';
        document.getElementById("Unit").value = product.Unit || '';
        document.getElementById("QuantityPerBox").value = product.QuantityPerBox || '';
    } else {
        document.getElementById("ProductId").value = '';
        document.getElementById("ProductName").value = '';
        document.getElementById("Unit").value = '';
        document.getElementById("QuantityPerBox").value = '';
    }
    updateBoxQuantity();
}

function updateBoxQuantity() {
    const quantity = parseFloat(quantityInput.value);
    const quantityPerBox = parseFloat(quantityPerBoxInput.value);
    if (!isNaN(quantity) && !isNaN(quantityPerBox) && quantityPerBox !== 0) {
        const boxQty = Math.ceil(quantity / quantityPerBox);
        boxQuantityInput.value = boxQty;
    } else {
        boxQuantityInput.value = '';
    }
}

document.getElementById("buttonAdd").addEventListener("click", function () {
    form.reset();
    form.action = '/SaleOut/Insert';
    showPopup('PopupInsert');
});

form.addEventListener("submit", function (e) {
    e.preventDefault();

    if (!form.checkValidity()) {
        alert("Vui lòng điền đầy đủ các trường bắt buộc.");
        return;
    }

    const customerPoNo = customerPoNoInput.value.trim();
    const productCode = document.getElementById("ProductCode").value.trim();
    const isDuplicate = allSaleOuts.some(s => s.CustomerPoNo === customerPoNo && s.Product.ProductCode === productCode);
    if (isDuplicate) {
        alert(`Mã PO ${customerPoNo} và Mã sản phẩm ${productCode} đã tồn tại.`);
        return;
    }

    const formData = {
        CustomerPoNo: customerPoNo,
        OrderDate: parseInt(orderDateInput.value.replace(/-/g, '')),
        CustomerName: document.getElementById("CustomerName").value,
        Quantity: parseFloat(quantityInput.value),
        QuantityPerBox: parseFloat(quantityPerBoxInput.value),
        BoxQuantity: parseFloat(boxQuantityInput.value),
        ProductId: document.getElementById("ProductId").value
    };
    console.log(formData);

    fetch(form.action, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(formData)
    })
    .then(response => {
        if (!response.ok) {
            return response.json().then(err => {
                console.error('Lỗi chi tiết từ API:', err);
                throw new Error(err.message || 'Lỗi không xác định.');
            });
        }
        return response.json();
    })
    .then(data => {
        alert('Thêm bản ghi thành công!');
        hidePopup('PopupInsert');
        window.location.reload();
    })
    .catch(error => {
        console.error('Lỗi:', error);
        alert(`Có lỗi xảy ra: ${error.message}. Vui lòng thử lại.`);
    });
});