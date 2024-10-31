function readURL(input, thumbimage) {
    if (input.files && input.files[0]) { //Sử dụng  cho Firefox - chrome
        var reader = new FileReader();
        reader.onload = function (e) {
            $("#thumbimage").attr('src', e.target.result);
        }
        reader.readAsDataURL(input.files[0]);
    } else { // Sử dụng cho IE
        $("#thumbimage").attr('src', input.value);

    }
    $("#thumbimage").show();
    $('.filename').text($("#uploadfile").val());
    $('.Choicefile').css('background', '#14142B');
    $('.Choicefile').css('cursor', 'default');
    $(".removeimg").show();
    $(".Choicefile").unbind('click');

}

$(document).ready(function () {
    $(".Choicefile").bind('click', function () {
        $("#uploadfile").click();

    });
    $(".removeimg").click(function () {
        $("#thumbimage").attr('src', '').hide();
        $("#myfileupload").html('<input type="file" id="uploadfile"  onchange="readURL(this);" />');
        $(".removeimg").hide();
        $(".Choicefile").bind('click', function () {
            $("#uploadfile").click();
        });
        $('.Choicefile').css('background', '#14142B');
        $('.Choicefile').css('cursor', 'pointer');
        $(".filename").text("");
    });
})

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
