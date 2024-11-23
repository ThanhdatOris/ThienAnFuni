function readURL(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $("#thumbimage").attr('src', e.target.result).show(); // Hiển thị hình ảnh
            $(".removeimg").css('display', 'block'); // Hiển thị nút xóa
        }
        reader.readAsDataURL(input.files[0]);
    } else {
        $("#thumbimage").attr('src', input.value).show();
        $(".removeimg").css('display', 'block'); // Hiển thị nút xóa
    }

    $('.filename').text($("#uploadfile").val());
    $('.Choicefile').css('background', '#14142B').css('cursor', 'default');
}

function removeImage() {
    $("#thumbimage").attr('src', '').hide(); // Ẩn hình ảnh
    $(".removeimg").css('display', 'none'); // Ẩn nút xóa
    $("#uploadfile").val(''); // Đặt lại giá trị của input file
    $('.filename').text(""); // Xóa tên tệp
    $('.Choicefile').css('background', '#14142B').css('cursor', 'pointer'); // Thiết lập lại nút chọn tệp
}

$(document).ready(function () {
    $(".Choicefile").on('click', function () {
        $("#uploadfile").click();
    });

    $(document).on('click', '.removeimg', function () {
        removeImage(); // Gọi hàm removeImage
    });

    // Delete image server side
    $(document).on('click', '.removeimg', function () {
        var imageName = $(this).data('image-name'); // Lấy tên file từ data attribute
        if (imageName) {
            // Gửi yêu cầu đến server để xóa ảnh
            $.ajax({
                url: '/AdminProducts/DeleteImage', // Thay "YourController" bằng tên Controller của bạn
                type: 'POST',
                data: { imageName: imageName },
                success: function (response) {
                    if (response.success) {
                        $("#thumbimage").attr('src', '').hide(); // Ẩn hình ảnh
                        $(".removeimg").hide(); // Ẩn nút xóa
                        $("#uploadfile").val(''); // Đặt lại input file
                        $('.filename').text(""); // Xóa tên tệp
                        $('.Choicefile').css('background', '#14142B').css('cursor', 'pointer'); // Đặt lại nút chọn tệp
                    } else {
                        swal({
                            title: "Xóa ảnh thất bại",
                            text: response.message,
                            icon: "error",
                            button: "OK",
                        });
                    }
                },
                error: function () {
                    swal({
                        title: 'Đã xảy ra lỗi khi xóa ảnh',
                        icon: "error",
                        button: "OK",
                    });
                }
            });
        } else {
            swal({
                title: 'Không có tên file để xóa.',
                icon: "error",
                button: "OK",
            });
        }
    });
});

// function readURL(input, thumbimage) {
//     if (input.files && input.files[0]) { //Sử dụng  cho Firefox - chrome
//         var reader = new FileReader();
//         reader.onload = function (e) {
//             $("#thumbimage").attr('src', e.target.result);
//         }
//         reader.readAsDataURL(input.files[0]);
//     } else { // Sử dụng cho IE
//         $("#thumbimage").attr('src', input.value);

//     }
//     $("#thumbimage").show();
//     $('.filename').text($("#uploadfile").val());
//     $('.Choicefile').css('background', '#14142B');
//     $('.Choicefile').css('cursor', 'default');
//     $(".removeimg").show();
//     $(".Choicefile").unbind('click');

// }

// $(document).ready(function () {
//     $(".Choicefile").bind('click', function () {
//         $("#uploadfile").click();

//     });
//     $(".removeimg").click(function () {
//         $("#thumbimage").attr('src', '').hide();
//         $("#myfileupload").html('<input type="file" id="uploadfile"  onchange="readURL(this);" />');
//         $(".removeimg").hide();
//         $(".Choicefile").bind('click', function () {
//             $("#uploadfile").click();
//         });
//         $('.Choicefile').css('background', '#14142B');
//         $('.Choicefile').css('cursor', 'pointer');
//         $(".filename").text("");
//     });
// })

// function readURL(input) {
//     if (input.files && input.files[0]) { // Sử dụng cho Firefox - Chrome
//         var reader = new FileReader();
//         reader.onload = function (e) {
//             $("#thumbimage").attr('src', e.target.result).show(); // Hiển thị hình ảnh
//             $(".removeimg").show(); // Hiện nút xóa
//         }
//         reader.readAsDataURL(input.files[0]);
//     } else { // Sử dụng cho IE
//         $("#thumbimage").attr('src', input.value).show(); // Hiển thị hình ảnh
//         $(".removeimg").show(); // Hiện nút xóa
//     }

//     $('.filename').text($("#uploadfile").val());
//     $('.Choicefile').css('background', '#14142B');
//     $('.Choicefile').css('cursor', 'default');
// }

// // Định nghĩa hàm removeImage
// function removeImage() {
//     $("#thumbimage").attr('src', '').hide(); // Ẩn hình ảnh
//     $(".removeimg").hide(); // Ẩn nút xóa hiện tại
//     $("#myfileupload").html('<input type="file" id="uploadfile" onchange="readURL(this);" />'); // Đặt lại input file
//     $('.filename').text(""); // Xóa tên tệp
//     $('.Choicefile').css('background', '#14142B'); // Thiết lập lại nền cho nút chọn tệp
//     $('.Choicefile').css('cursor', 'pointer'); // Thiết lập lại con trỏ chuột cho nút
// }

// $(document).ready(function () {
//     $(".Choicefile").on('click', function () {
//         $("#uploadfile").click();
//     });

//     // Gắn sự kiện click cho removeimg
//     $(document).on('click', '.removeimg', function () {
//         removeImage(); // Gọi hàm removeImage
//     });
// });




function time() {
    const today = new Date();
    const weekday = ["Chủ Nhật", "Thứ Hai", "Thứ Ba", "Thứ Tư", "Thứ Năm", "Thứ Sáu", "Thứ Bảy"];
    const day = weekday[today.getDay()];
    let dd = today.getDate();
    let mm = today.getMonth() + 1; // Tháng bắt đầu từ 0
    const yyyy = today.getFullYear();
    let h = today.getHours();
    let m = today.getMinutes();
    let s = today.getSeconds();

    // Kiểm tra và thêm số 0 phía trước nếu số nhỏ hơn 10
    m = checkTime(m);
    s = checkTime(s);

    const nowTime = h + " giờ " + m + " phút " + s + " giây";

    // Định dạng ngày với tiền tố '0' nếu cần
    if (dd < 10) {
        dd = '0' + dd;
    }
    if (mm < 10) {
        mm = '0' + mm;
    }

    const formattedDate = day + ', ' + dd + '/' + mm + '/' + yyyy;
    const tmp = '<span class="date">' + formattedDate + ' - ' + nowTime + '</span>';

    // Cập nhật nội dung của phần tử clock nếu tồn tại
    const clockElement = document.getElementById("clock");
    if (clockElement) {
        clockElement.innerHTML = tmp;
    }

    // Thiết lập thời gian để cập nhật lại sau mỗi giây
    setTimeout(time, 1000);

    function checkTime(i) {
        return (i < 10) ? "0" + i : i;
    }
}
