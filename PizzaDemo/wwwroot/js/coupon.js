function applyCoupon() {
    var code = $('#couponCode').val().trim();

    console.log('Coupon Code:', code);

    if (!code) {
        $('#couponMessage').text('請輸入折扣碼');
        return;
    }

    $.ajax({
        url: '/Customer/Cart/ApplyCoupon',
        type: 'POST',
        data: {
            code: code
        },
        success: function (response) {
            console.log('Response:', response);

            if (response.success) {
                $('#discountAmount').text('-$' + response.discount);
                $('#orderTotal').text('$' + response.newTotal.toFixed(2));
                $('#couponMessage').removeClass('text-danger').addClass('text-success').text(response.message);
                $('#couponCode').prop('disabled', true);
                $('.btn-primary').prop('disabled', true);
            } else {
                $('#couponMessage').removeClass('text-success').addClass('text-danger').text(response.message);
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', error);
            $('#couponMessage').text('發生錯誤，請稍後再試');
        }
    });
}