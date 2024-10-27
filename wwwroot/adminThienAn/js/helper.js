function deleteRow(r) {
    var i = r.parentNode.parentNode.rowIndex;
    document.getElementById("myTable").deleteRow(i);
}

// jQuery(function () {
//     jQuery(".trash").click(function () {
//         swal({
//             title: "Cảnh báo",

//             text: "Bạn có chắc chắn là muốn xóa nhân viên này?",
//             buttons: ["Hủy bỏ", "Đồng ý"],
//         })
//             .then((willDelete) => {
//                 if (willDelete) {
//                     swal("Đã xóa thành công.!", {

//                     });
//                 }
//             });
//     });
// });

oTable = $('#sampleTable').dataTable();
$('#all').click(function (e) {
    $('#sampleTable tbody :checkbox').prop('checked', $(this).is(':checked'));
    e.stopImmediatePropagation();
});

oTable = $('#sampleTableSub').dataTable();
$('#all').click(function (e) {
    $('#sampleTableSub tbody :checkbox').prop('checked', $(this).is(':checked'));
    e.stopImmediatePropagation();
});
//Thời Gian
//function time() {
//    var today = new Date();
//    var weekday = new Array(7);
//    weekday[0] = "Chủ Nhật";
//    weekday[1] = "Thứ Hai";
//    weekday[2] = "Thứ Ba";
//    weekday[3] = "Thứ Tư";
//    weekday[4] = "Thứ Năm";
//    weekday[5] = "Thứ Sáu";
//    weekday[6] = "Thứ Bảy";
//    var day = weekday[today.getDay()];
//    var dd = today.getDate();
//    var mm = today.getMonth() + 1;
//    var yyyy = today.getFullYear();
//    var h = today.getHours();
//    var m = today.getMinutes();
//    var s = today.getSeconds();
//    m = checkTime(m);
//    s = checkTime(s);
//    nowTime = h + " giờ " + m + " phút " + s + " giây";
//    if (dd < 10) {
//        dd = '0' + dd
//    }
//    if (mm < 10) {
//        mm = '0' + mm
//    }
//    today = day + ', ' + dd + '/' + mm + '/' + yyyy;
//    tmp = '<span class="date"> ' + today + ' - ' + nowTime +
//        '</span>';
//    document.getElementById("clock")?.innerHTML = tmp;
//    clocktime = setTimeout("time()", "1000", "Javascript");

//    function checkTime(i) {
//        if (i < 10) {
//            i = "0" + i;
//        }
//        return i;
//    }
//}

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
    document.getElementById("clock")?.innerHTML = tmp;

    // Thiết lập thời gian để cập nhật lại sau mỗi giây
    setTimeout(time, 1000);

    function checkTime(i) {
        return (i < 10) ? "0" + i : i;
    }
}
// Gọi hàm time khi DOM đã được tải
// document.addEventListener("DOMContentLoaded", time);

// Gọi hàm time sau 5 giây
// setTimeout(time, 5000);

//In dữ liệu
var myApp = new function () {
    this.printTable = function () {
        var tab = document.getElementById('sampleTable');
        var win = window.open('', '', 'height=700,width=700');
        win.document.write(tab.outerHTML);
        win.document.close();
        win.print();
    }
}
//     //Sao chép dữ liệu
//     var copyTextareaBtn = document.querySelector('.js-textareacopybtn');

// copyTextareaBtn.addEventListener('click', function(event) {
//   var copyTextarea = document.querySelector('.js-copytextarea');
//   copyTextarea.focus();
//   copyTextarea.select();

//   try {
//     var successful = document.execCommand('copy');
//     var msg = successful ? 'successful' : 'unsuccessful';
//     console.log('Copying text command was ' + msg);
//   } catch (err) {
//     console.log('Oops, unable to copy');
//   }
// });


//Modal
// $("#show-emp").on("click", function () {
//     $("#ModalUP").modal({ backdrop: false, keyboard: false })
// });

// $('#ModalUP').modal('show');

