$(document).ready(function () {
    // Gọi toggleDeleteButton khi trang tải để kiểm tra trạng thái ban đầu
    toggleDeleteButton();

    // Xử lý checkbox "Chọn tất cả"
    $('#selectAll').on('change', function () {
        $('.productCheckbox').prop('checked', $(this).prop('checked'));
        toggleDeleteButton();
    });

    // Xử lý khi checkbox sản phẩm thay đổi
    $('.productCheckbox').on('change', function () {
        $('#selectAll').prop('checked', $('.productCheckbox:checked').length === $('.productCheckbox').length);
        toggleDeleteButton();
    });

    // Hàm hiển thị/ẩn button Xóa
    function toggleDeleteButton() {
        if ($('.productCheckbox:checked').length > 0) {
            $('#deleteSelectedBtn').show();
        } else {
            $('#deleteSelectedBtn').hide();
        }
    }
});

// Hàm xóa các sản phẩm đã chọn
function deleteSelectedProducts() {
    if (confirm('Bạn có chắc chắn muốn xóa các sản phẩm đã chọn?')) {
        var selectedIds = $('.productCheckbox:checked').map(function () {
            return $(this).val();
        }).get();

        $.ajax({
            url: '@Url.Action("DeleteMultiple", "Product")', // URL sẽ được thay thế trong view
            type: 'POST',
            data: { ids: selectedIds },
            traditional: true,
            success: function (response) {
                if (response.success) {
                    alert('Xóa thành công!');
                    location.reload(); // Tải lại trang để cập nhật danh sách
                } else {
                    alert('Xóa thất bại: ' + response.message);
                }
            },
            error: function () {
                alert('Đã xảy ra lỗi khi xóa sản phẩm.');
            }
        });
    }
}