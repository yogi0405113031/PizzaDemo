$(document).ready(function () {
    // 圖片預覽功能
    $('input[name="file"]').change(function () {
        var file = this.files[0];
        if (file) {
            // 檢查文件大小
            if (file.size > 5 * 1024 * 1024) { // 5MB
                Swal.fire({
                    icon: 'error',
                    title: '檔案太大',
                    text: '圖片大小不能超過 5MB'
                });
                this.value = '';
                return;
            }

            // 檢查文件類型
            var validTypes = ['image/jpeg', 'image/png', 'image/gif'];
            if (!validTypes.includes(file.type)) {
                Swal.fire({
                    icon: 'error',
                    title: '檔案類型錯誤',
                    text: '只允許上傳 JPG、PNG 或 GIF 圖片'
                });
                this.value = '';
                return;
            }

            // 預覽圖片
            var reader = new FileReader();
            reader.onload = function (e) {
                // 更新兩個預覽圖
                $('.col-2 img').attr('src', e.target.result).show();
                $('.mt-2 img').attr('src', e.target.result);

                // 如果預覽圖不存在，則創建
                if ($('.mt-2').length === 0) {
                    var previewHtml = '<div class="mt-2"><img src="' + e.target.result + '" style="width:100px" /></div>';
                    $('input[name="file"]').after(previewHtml);
                }
            }
            reader.readAsDataURL(file);
        }
    });

    // 圖片載入錯誤處理
    $('img').on('error', function () {
        $(this).attr('src', '/images/default-team-member.jpg');
    });

    // 表單驗證
    $("form").validate({
        rules: {
            Name: {
                required: true,
                minlength: 2
            },
            Position: {
                required: true
            }
        },
        messages: {
            Name: {
                required: "請輸入姓名",
                minlength: "姓名至少需要2個字符"
            },
            Position: {
                required: "請輸入職位"
            }
        }
    });
});