using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ThienAnFuni.Models.VNPay;
using ThienAnFuni.Services.VNPay;

namespace ThienAnFuni.Controllers
{
    public class VNPayController : Controller
    {
        private readonly IVNPayService _vnPayService;
        private readonly VNPaySettings _vnPaySettings;

        public VNPayController(IVNPayService vnPayService, IOptions<VNPaySettings> vnPaySettings)
        {
            _vnPayService = vnPayService;
            _vnPaySettings = vnPaySettings.Value;
        }

        [HttpGet]
        public IActionResult CreatePayment()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreatePayment(long amount, string orderInfo)
        {
            if (amount <= 0)
            {
                TempData["VNPayError"] = "Số tiền không hợp lệ.";
                return RedirectToAction("PaymentResult");
            }

            // Lấy IP của client
            string ipAddr = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

            // Tạo URL thanh toán
            string paymentUrl = _vnPayService.CreatePaymentUrl(amount, orderInfo, ipAddr);

            // Lưu thông tin vào session để kiểm tra callback
            HttpContext.Session.SetString("VNPayAmount", amount.ToString());
            HttpContext.Session.SetString("VNPayOrderInfo", orderInfo);

            // Redirect đến VNPay
            return Redirect(paymentUrl);
        }

        [HttpGet]
        public IActionResult PaymentReturn()
        {
            var queryParams = HttpContext.Request.Query;

            // Validate signature
            bool isValidSignature = _vnPayService.ValidateSignature(queryParams, _vnPaySettings.HashSecret);

            if (isValidSignature)
            {
                string vnp_ResponseCode = queryParams["vnp_ResponseCode"];
                string vnp_TxnRef = queryParams["vnp_TxnRef"];
                string vnp_Amount = queryParams["vnp_Amount"];
                string vnp_OrderInfo = queryParams["vnp_OrderInfo"];
                string vnp_TransactionNo = queryParams["vnp_TransactionNo"];
                string vnp_BankCode = queryParams["vnp_BankCode"];
                string vnp_PayDate = queryParams["vnp_PayDate"];

                // Convert amount back from cents
                long amount = long.Parse(vnp_Amount) / 100;

                if (vnp_ResponseCode == "00") // Success
                {
                    // Process successful payment
                    ProcessSuccessfulPayment(vnp_TxnRef, amount.ToString(), vnp_OrderInfo, vnp_TransactionNo);

                    ViewBag.Message = $"Thanh toán thành công! Mã giao dịch: {vnp_TransactionNo}, Ngân hàng: {vnp_BankCode}";
                    ViewBag.IsSuccess = true;
                }
                else
                {
                    // Process failed payment
                    ProcessFailedPayment(vnp_TxnRef, amount.ToString(), vnp_OrderInfo);

                    ViewBag.Message = GetVNPayResponseMessage(vnp_ResponseCode);
                    ViewBag.IsSuccess = false;
                }
            }
            else
            {
                ViewBag.Message = "Chữ ký không hợp lệ!";
                ViewBag.IsSuccess = false;
            }

            // Clear session
            HttpContext.Session.Remove("VNPayAmount");
            HttpContext.Session.Remove("VNPayOrderInfo");

            return View("PaymentResult");
        }

        private void ProcessSuccessfulPayment(string txnRef, string amount, string orderInfo, string transactionNo)
        {
            // TODO: Implement logic xử lý thanh toán thành công
            Console.WriteLine($"VNPay: Successful payment - TxnRef: {txnRef}, Amount: {amount}, TransactionNo: {transactionNo}");

            // Clear cart
            HttpContext.Session.Remove("cart");

            // Add your logic here:
            // - Update order status in database
            // - Update product quantities
            // - Send confirmation email
            // - Save transaction info
        }

        private void ProcessFailedPayment(string txnRef, string amount, string orderInfo)
        {
            // TODO: Implement logic xử lý thanh toán thất bại
            Console.WriteLine($"VNPay: Failed payment - TxnRef: {txnRef}, Amount: {amount}");

            // Add your logic here:
            // - Log failed payment
            // - Update order status if needed
        }

        private string GetVNPayResponseMessage(string responseCode)
        {
            return responseCode switch
            {
                "00" => "Giao dịch thành công",
                "07" => "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, giao dịch bất thường).",
                "09" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking tại ngân hàng.",
                "10" => "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần",
                "11" => "Giao dịch không thành công do: Đã hết hạn chờ thanh toán. Xin quý khách vui lòng thực hiện lại giao dịch.",
                "12" => "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa.",
                "13" => "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP).",
                "24" => "Giao dịch không thành công do: Khách hàng hủy giao dịch",
                "51" => "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch.",
                "65" => "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày.",
                "75" => "Ngân hàng thanh toán đang bảo trì.",
                "79" => "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định.",
                _ => $"Lỗi không xác định. Mã lỗi: {responseCode}"
            };
        }

        public IActionResult PaymentResult()
        {
            ViewBag.Message = TempData["VNPayMessage"] ?? ViewBag.Message;
            ViewBag.IsSuccess = TempData["VNPayIsSuccess"] ?? ViewBag.IsSuccess;
            ViewBag.Error = TempData["VNPayError"];
            return View();
        }
    }
}
