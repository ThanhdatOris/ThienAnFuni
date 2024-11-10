$(document).on('click', '.add-to-cart', function (e) {
    e.preventDefault();
    var productId = $(this).data('id');
    // Lấy `quantity` từ input trong cùng `.product__details__quantity` gần với nút bấm
    var quantity = $('.product__details__quantity .pro-qty input').val();
    console.log("Selected quantity:", quantity);

    $.ajax({
        url: `/cart/AddToCart/`,
        method: 'POST',
        data: {
            //_token: $('meta[name="csrf-token"]').attr('content'), // CSRF token
            id: productId, 
            quantity: quantity // Gửi số lượng lên server
        },
        success: function (response) {
            alert('Sản phẩm đã được thêm vào giỏ hàng!');
            // Cập nhật tổng tiền và số lượng giỏ hàng
            $('.header__cart__price span').text(response.total);
            $('.span__quantity_cart').text(response.totalQuantity);
        },
        error: function (xhr) {
            if (xhr.status === 400 && xhr.responseJSON && xhr.responseJSON.error) {
                alert(xhr.responseJSON.error);
            } else {
                alert('Đã xảy ra lỗi. Vui lòng thử lại!');
            }
        }
    });
});
