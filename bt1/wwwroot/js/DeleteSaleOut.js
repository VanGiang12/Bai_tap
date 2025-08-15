document.querySelectorAll(".btnDelete").forEach(function (item) {
    item.addEventListener("click", function () {
        const form = document.getElementById("FormDelete");

        form.Id.value = item.dataset.id;
        document.getElementById("overlay").style.display = "block";
        document.getElementById("PopupDelete").style.display = "block";
    });
});