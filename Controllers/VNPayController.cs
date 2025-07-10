using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using ThienAnFuni.Helpers;
using ThienAnFuni.Models.VNPay;
using ThienAnFuni.Services.VNPay;
using ThienAnFuni.Models; // Add this if CartDetail is in ThienAnFuni.Models namespace
namespace ThienAnFuni.Controllers
{
    // Extension method for ISession to get object from JSON
    public static class SessionExtensions
    {
        public static T GetObjectFromJson<T>(this ISession session, string key)
        {
            var value = session.GetString(key);
            return value == null ? default(T) : System.Text.Json.JsonSerializer.Deserialize<T>(value);
        }
    }

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
        public IActionResult CreatePayment(string address, int paymentMethod, string? note)
        {

            // Lưu tham số vào session address, ,paymentMethod, note
            HttpContext.Session.SetString("address", address);
            HttpContext.Session.SetInt32("paymentMethod", paymentMethod);
            HttpContext.Session.SetString("note", note ?? string.Empty);

            // Get cart localstorage
            var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart");
            if (cart == null || cart.Count <= 0)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống. Vui lòng thêm sản phẩm trước khi thanh toán.";
                return RedirectToAction("Index", "Cart");
            }
            if (string.IsNullOrWhiteSpace(address))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập địa chỉ giao hàng.";
                return RedirectToAction("CheckOutPro", "Cart");
            }
            if (!Enum.IsDefined(typeof(ConstHelper.PaymentMethod), paymentMethod))
            {
                TempData["ErrorMessage"] = "Phương thức thanh toán không hợp lệ.";
                return RedirectToAction("CheckOutPro", "Cart");
            }

            long amount = cart.Sum(item => (long)(item.Value.Price * item.Value.Quantity));

            string orderInfo = $"User:{User.Identity.Name}|Time:{DateTime.Now:yyyyMMddHHmmss}";


            // ===================


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
                string vnp_ResponseCode = queryParams["vnp_ResponseCode"].ToString() ?? "";
                string vnp_TxnRef = queryParams["vnp_TxnRef"].ToString() ?? "";
                string vnp_Amount = queryParams["vnp_Amount"].ToString() ?? "0";
                string vnp_OrderInfo = queryParams["vnp_OrderInfo"].ToString() ?? "";
                string vnp_TransactionNo = queryParams["vnp_TransactionNo"].ToString() ?? "";
                string vnp_BankCode = queryParams["vnp_BankCode"].ToString() ?? "";
                string vnp_PayDate = queryParams["vnp_PayDate"].ToString() ?? "";

                // Convert amount back from cents
                long amount = long.Parse(vnp_Amount) / 100;

                if (vnp_ResponseCode == "00") // Success
                {
                    // Process successful payment - return view thay vì gọi method
                    Console.WriteLine($"VNPay: Successful payment - TxnRef: {vnp_TxnRef}, Amount: {amount}, TransactionNo: {vnp_TransactionNo}");

                    // Save payment info to session for CheckOutSV to use
                    HttpContext.Session.SetString("VNPayTxnRef", vnp_TxnRef);
                    HttpContext.Session.SetString("VNPayTransactionNo", vnp_TransactionNo);

                    // Return view với form auto-post để thực hiện POST thực sự
                    var model = new PostToCartModel
                    {
                        TxnRef = vnp_TxnRef,
                        Amount = amount.ToString(),
                        OrderInfo = vnp_OrderInfo,
                        TransactionNo = vnp_TransactionNo,
                        BankCode = vnp_BankCode,
                        PayDate = vnp_PayDate
                    };

                    return View("PostToCart", model);
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

        private IActionResult ProcessSuccessfulPayment(string txnRef, string amount, string orderInfo, string transactionNo)
        {
            // TODO: Implement logic xử lý thanh toán thành công
            Console.WriteLine($"VNPay: Successful payment - TxnRef: {txnRef}, Amount: {amount}, TransactionNo: {transactionNo}");

            // DON'T Clear cart here - let CheckOutSV handle it
            // HttpContext.Session.Remove("cart");

            // Save payment info to session for CheckOutSV to use
            HttpContext.Session.SetString("VNPayTxnRef", txnRef);
            HttpContext.Session.SetString("VNPayTransactionNo", transactionNo);

            // Return view với form auto-post để thực hiện POST thực sự
            var model = new PostToCartModel
            {
                TxnRef = txnRef,
                Amount = amount,
                OrderInfo = orderInfo,
                TransactionNo = transactionNo,
                BankCode = "", // Có thể thêm từ queryParams nếu cần
                PayDate = DateTime.Now.ToString("yyyyMMddHHmmss")
            };

            return View("PostToCart", model);
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
