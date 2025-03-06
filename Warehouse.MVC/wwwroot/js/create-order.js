$(document).ready(function () {
    $('.product-checkbox').change(function () {
        var productId = $(this).val();
        var quantityInput = $(`input[name="SelectedProducts[${productId}].Quantity"]`);
        if ($(this).is(':checked')) {
            quantityInput.prop('disabled', false).prop('required', true);
        } else {
            quantityInput.prop('disabled', true).prop('required', false).val(1);
        }
    });

    $('#addProductForm').submit(function (e) {
        e.preventDefault();
        var form = $(this);
        var formData = form.serializeArray();
        console.log("Dữ liệu gửi lên: ", formData);

        $.ajax({
            url: form.attr('action'),
            type: 'POST',
            data: formData,
            success: function (response) {
                if (response.success) {
                    $('#productModal').modal('hide');
                    location.reload();
                } else {
                    alert(response.message);
                }
            },
            error: function (xhr, status, error) {
                alert('Có lỗi xảy ra: ' + error);
                console.log("Lỗi chi tiết: ", xhr.responseText);
            }
        });
    });
});