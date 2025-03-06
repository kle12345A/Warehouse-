$(document).ready(function () {
    // Gọi toggleDeleteButton khi trang tải để kiểm tra trạng thái ban đầu
    toggleDeleteButton();

    // Xử lý checkbox "Chọn tất cả"
    $('#selectAll').on('change', function () {
        $('.categoryCheckbox').prop('checked', $(this).prop('checked'));
        toggleDeleteButton();
    });

    // Xử lý khi checkbox danh mục thay đổi
    $('.categoryCheckbox').on('change', function () {
        $('#selectAll').prop('checked', $('.categoryCheckbox:checked').length === $('.categoryCheckbox').length);
        toggleDeleteButton();
    });

    // Hàm hiển thị/ẩn button Xóa
    function toggleDeleteButton() {
        if ($('.categoryCheckbox:checked').length > 0) {
            $('#deleteSelectedBtn').show();
        } else {
            $('#deleteSelectedBtn').hide();
        }
    }
});

// Hàm xóa các danh mục đã chọn
function deleteSelectedCategories() {
    if (confirm('Bạn có chắc chắn muốn xóa các danh mục đã chọn?')) {
        var selectedIds = $('.categoryCheckbox:checked').map(function () {
            return $(this).val();
        }).get();

        $.ajax({
            url: window.deleteMultipleUrl, // URL được truyền từ view
            type: 'POST',
            data: { ids: selectedIds },
            traditional: true,
            success: function (response) {
                if (response.success) {
                    alert('Xóa danh mục thành công!');
                    location.reload(); // Tải lại trang để cập nhật danh sách
                } else {
                    alert('Xóa danh mục thất bại: ' + response.message);
                }
            },
            error: function () {
                alert('Đã xảy ra lỗi khi xóa danh mục.');
            }
        });
    }
}