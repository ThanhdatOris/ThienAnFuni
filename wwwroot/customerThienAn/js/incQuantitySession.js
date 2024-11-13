$(document).ready(function () {
    // Xử lý nút tăng và giảm số lượng
    var proQty = $('.pro-qty');
    proQty.prepend('<span class="dec qtybtn">-</span>');
    proQty.append('<span class="inc qtybtn">+</span>');

    proQty.on('click', '.qtybtn', function () {
        var $button = $(this);
        var $input = $button.parent().find('input');
        var oldValue = parseFloat($input.val());

        // Kiểm tra nút được nhấn là tăng hay giảm
        if ($button.hasClass('inc')) {
            var newVal = oldValue + 1;
        } else {
            var newVal = oldValue > 1 ? oldValue - 1 : 1; // Không giảm xuống dưới 1
        }

        // Cập nhật giá trị input và kích hoạt sự kiện 'change'
        $input.val(newVal).trigger('change');
    });

    // // Khi số lượng thay đổi, gửi ajax để cập nhật giỏ hàng
    // $('.pro-qty input').on('change', function () {
    //     var id = $(this).data('id'); // Giả sử mỗi input có data-id để xác định sản phẩm
    //     var quantity = $(this).val();

    //     $.ajax({
    //         url: `{{ route('customer.cart.update') }}`, // Đường dẫn cập nhật giỏ hàng
    //         method: 'POST',
    //         data: {
    //             _token: '{{ csrf_token() }}',
    //             id: id,
    //             quantity: quantity
    //         },
    //         success: function (response) {
    //             // Cập nhật tổng giá của sản phẩm và giỏ hàng
    //             $('#product-total-' + id).text(response.productTotal);
    //             $('#cart-total').text(response.cartTotal);
    //         },
    //         error: function () {
    //             alert('Đã xảy ra lỗi khi cập nhật giỏ hàng.');
    //         }
    //     });
    // });
});
