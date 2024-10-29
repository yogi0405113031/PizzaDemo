function applyCoupon() {
    const couponCode = document.getElementById('couponCode').value;
    const discountAmount = document.getElementById('discountAmount');
    const couponMessage = document.getElementById('couponMessage');
    const orderTotal = document.getElementById('orderTotal');

    if (couponCode.toUpperCase() === 'SAVE100') {
        discountAmount.textContent = '-$100';
        couponMessage.textContent = '折扣碼套用成功！';
        couponMessage.className = 'text-success';

        let currentTotal = parseFloat(orderTotal.textContent.replace('$', '').replace(',', ''));
        let newTotal = currentTotal - 100;
        orderTotal.textContent = '$' + newTotal.toFixed(2);

        document.getElementById('couponCode').disabled = true;
        document.querySelector('button').disabled = true;
    } else {
        discountAmount.textContent = '$0';
        couponMessage.textContent = '無效的折扣碼';
        couponMessage.className = 'text-danger';
    }
}