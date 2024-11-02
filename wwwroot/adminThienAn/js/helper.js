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

