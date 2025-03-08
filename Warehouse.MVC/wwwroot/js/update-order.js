$(document).ready(function () {
    $("#addProductForm").on("submit", function (e) {
        e.preventDefault(); // Ngăn submit thông thường

        var formData = $(this).serialize(); // Lấy dữ liệu form
        console.log("Dữ liệu gửi đi: ", formData);

        $.ajax({
            url: $(this).attr("action"), // URL từ action của form
            type: "POST",
            data: formData,
            success: function (response) {
                if (response.success) {
                    alert(response.message); // Hiển thị thông báo thành công
                    $("#productModal").modal("hide"); // Đóng modal
                    location.reload(); // Tải lại trang để cập nhật danh sách
                } else {
                    alert(response.message); // Hiển thị lỗi
                }
            },
            error: function (xhr, status, error) {
                console.log("Lỗi: ", error);
                alert("Đã xảy ra lỗi khi thêm sản phẩm.");
            }
        });
    });

    // Ẩn số lượng mặc định, chỉ hiện khi checkbox được chọn
    $(".product-quantity").hide();
    $(".product-checkbox").on("change", function () {
        var quantityInput = $(this).closest("tr").find(".product-quantity");
        quantityInput.toggle(this.checked);
        if (!this.checked) quantityInput.val(1); // Reset số lượng khi bỏ chọn
    });
});