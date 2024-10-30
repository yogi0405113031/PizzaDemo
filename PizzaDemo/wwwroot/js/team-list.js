$(document).ready(function () {
    // 圖片載入錯誤處理
    $('img').on('error', function () {
        $(this).attr('src', '/images/team/default-team-member.jpg');
    });

    // 初始化工具提示
    $('[data-bs-toggle="tooltip"]').tooltip();

    // 限制介紹文字顯示長度
    $('.card-text').each(function () {
        var maxLength = 100;
        var text = $(this).text().trim();
        if (text.length > maxLength) {
            $(this).text(text.substring(0, maxLength) + '...');
            $(this).attr('title', text);
            $(this).tooltip();
        }
    });
});

// 刪除團隊成員
function deleteTeamMember(id) {
    Swal.fire({
        title: '確定要刪除嗎？',
        text: "此操作無法復原！",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: '是的，刪除！',
        cancelButtonText: '取消'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Admin/TeamMember/Delete/' + id,  // 注意這裡加上了 Area 路徑
                type: 'DELETE',
                success: function (data) {
                    if (data.success) {
                        Swal.fire(
                            '已刪除！',
                            data.message,
                            'success'
                        ).then(() => {
                            location.reload();
                        });
                    } else {
                        Swal.fire(
                            '錯誤！',
                            data.message,
                            'error'
                        );
                    }
                },
                error: function () {
                    Swal.fire(
                        '錯誤！',
                        '刪除過程中發生錯誤',
                        'error'
                    );
                }
            });
        }
    });
}

// 圖片載入動畫
function addImageLoadingEffect() {
    $('.card-img-top').each(function () {
        $(this).addClass('img-loading');
        $(this).on('load', function () {
            $(this).removeClass('img-loading');
        });
    });
}