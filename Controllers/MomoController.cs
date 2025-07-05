using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text;
using ThienAnFuni.Models.Momo;
using ThienAnFuni.Services.Momo;
using ThienAnFuni.Models;
using ThienAnFuni.Extensions;
using System.Web;

namespace ThienAnFuni.Controllers
{
    public class MomoController : Controller
    {
        private readonly IMomoService _momoService;
        private readonly MomoSettings _momoSettings;
        // private readonly ApplicationDbContext _context; // Nếu dùng EF Core

        public MomoController(IMomoService momoService, IOptions<MomoSettings> momoSettings /*, ApplicationDbContext context */)
        {
            _momoService = momoService;
            _momoSettings = momoSettings.Value;
            // _context = context; // Nếu dùng EF Core
        }

        [HttpGet]
        public IActionResult CreatePayment()
        {
            // View này sẽ có form để người dùng nhập thông tin đơn hàng (ví dụ)
            // Hoặc bạn có thể lấy thông tin đơn hàng từ nơi khác (giỏ hàng, database)
            var cart = HttpContext.Session.GetObjectFromJson<Dictionary<int, CartDetail>>("cart") ?? new Dictionary<int, CartDetail>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreatePayment(long amount, string orderInfo) // Nhận amount và orderInfo từ form
        {
            if (amount <= 0)
            {
                TempData["MomoError"] = "Số tiền không hợp lệ.";
                return RedirectToAction("PaymentResult");
            }

            var orderId = Guid.NewGuid().ToString(); // Tạo OrderId duy nhất cho mỗi giao dịch
            var extraData = ""; // "merchantName=YourShopName;merchantId=YourShopId" (Base64 encoded) - tùy chọn

            try
            {
                var response = await _momoService.CreatePaymentAsync(amount, orderId, orderInfo, extraData);

                if (response != null && response.ResultCode == 0 && !string.IsNullOrEmpty(response.PayUrl))
                {
                    // Lưu thông tin cần thiết vào Session hoặc TempData để đối chiếu khi Momo redirect về
                    HttpContext.Session.SetString("MomoOrderId", orderId);
                    HttpContext.Session.SetString("MomoRequestId", response.RequestId); // Lưu RequestId từ response của Momo

                    // Log thông tin QR để debug
                    Console.WriteLine($"QR Code URL: {response.QrCodeUrl}");
                    Console.WriteLine($"Pay URL: {response.PayUrl}");
                    Console.WriteLine($"Deep Link: {response.Deeplink}");

                    // Chuyển hướng người dùng đến trang thanh toán của Momo
                    return Redirect(response.PayUrl);
                }
                else
                {
                    TempData["MomoError"] = $"Lỗi từ Momo: {response?.Message} (Code: {response?.ResultCode})";
                    return RedirectToAction("PaymentResult");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi (sử dụng một thư viện logging như Serilog, NLog, ...)
                Console.WriteLine($"Exception during MoMo payment creation: {ex.Message}");
                TempData["MomoError"] = "Có lỗi xảy ra trong quá trình tạo thanh toán. Vui lòng thử lại.";
                return RedirectToAction("PaymentResult");
            }
        }


        [HttpGet]
        public IActionResult PaymentReturn()
        {
            var queryString = HttpContext.Request.QueryString.Value;
            Console.WriteLine($"Raw Query String: {queryString}");

            var queryParams = System.Web.HttpUtility.ParseQueryString(queryString ?? "");
            var partnerCode = queryParams["partnerCode"];
            var orderId = queryParams["orderId"];
            var requestId = queryParams["requestId"];
            var amount = queryParams["amount"];
            var orderInfo = queryParams["orderInfo"]; // Decode URL
            var orderType = queryParams["orderType"];
            var transId = queryParams["transId"];
            var resultCode = queryParams["resultCode"];
            var message = queryParams["message"]; // Decode URL
            var payType = queryParams["payType"];
            var responseTime = queryParams["responseTime"];
            var extraData = queryParams["extraData"];
            var signature = queryParams["signature"];

            Console.WriteLine($"Decoded orderInfo: {orderInfo}");
            Console.WriteLine($"Decoded message: {message}");

            var sessionOrderId = HttpContext.Session.GetString("MomoOrderId");
            var sessionRequestId = HttpContext.Session.GetString("MomoRequestId");

            if (string.IsNullOrEmpty(sessionOrderId) || string.IsNullOrEmpty(sessionRequestId) ||
                sessionOrderId != orderId || sessionRequestId != requestId)
            {
                ViewBag.Message = "Thông tin giao dịch không khớp hoặc đã hết hạn.";
                ViewBag.IsSuccess = false;
                return View("PaymentResult");
            }

            HttpContext.Session.Remove("MomoOrderId");
            HttpContext.Session.Remove("MomoRequestId");

            var responseData = new Dictionary<string, string>
                {
                    { "partnerCode", partnerCode ?? "" },
                    { "orderId", orderId ?? "" },
                    { "requestId", requestId ?? "" },
                    { "amount", amount ?? "" },
                    { "orderInfo", orderInfo ?? "" }, // Sử dụng giá trị đã decode
                    { "orderType", orderType ?? "" },
                    { "transId", transId ?? "" },
                    { "resultCode", resultCode ?? "" },
                    { "message", message ?? "" }, // Sử dụng giá trị đã decode
                    { "payType", payType ?? "" },
                    { "responseTime", responseTime ?? "" },
                    { "extraData", extraData ?? "" }
                };

            bool isValidSignature = _momoService.ValidateSignature(responseData, _momoSettings.SecretKey, signature);

            if (isValidSignature)
            {
                if (resultCode == "0")
                {
                    // Xử lý thanh toán thành công
                    ProcessSuccessfulPayment(orderId, amount, orderInfo, transId);

                    ViewBag.Message = $"Thanh toán thành công cho đơn hàng {orderId}! Mã giao dịch Momo: {transId}. {message}";
                    ViewBag.IsSuccess = true;
                }
                else
                {
                    // Xử lý thanh toán thất bại
                    ProcessCancelledPayment(orderId, amount, orderInfo);

                    ViewBag.Message = $"Thanh toán thất bại: {message} (Mã lỗi: {resultCode})";
                    ViewBag.IsSuccess = false;
                }
            }
            else
            {
                ViewBag.Message = "Chữ ký không hợp lệ! Giao dịch có thể đã bị thay đổi.";
                ViewBag.IsSuccess = false;
            }

            return View("PaymentResult");
        }

        //[HttpGet]
        //public IActionResult PaymentReturn() // Momo sẽ redirect về URL này
        //{
        //    // Lấy chuỗi query thô
        //    var queryString = HttpContext.Request.QueryString.Value;
        //    Console.WriteLine($"Raw Query String: {queryString}");



        //    // Lấy các tham số từ query string mà Momo gửi về
        //    var queryParams = System.Web.HttpUtility.ParseQueryString(queryString ?? "");

        //    var partnerCode = HttpContext.Request.Query["partnerCode"].ToString();
        //    var orderId = HttpContext.Request.Query["orderId"].ToString();
        //    var requestId = HttpContext.Request.Query["requestId"].ToString();
        //    var amount = HttpContext.Request.Query["amount"].ToString();
        //    var orderInfo = HttpContext.Request.Query["orderInfo"].ToString();
        //    var orderType = HttpContext.Request.Query["orderType"].ToString();
        //    var transId = HttpContext.Request.Query["transId"].ToString(); // Mã giao dịch của Momo
        //    var resultCode = HttpContext.Request.Query["resultCode"].ToString();
        //    var message = HttpContext.Request.Query["message"].ToString();
        //    var payType = HttpContext.Request.Query["payType"].ToString();
        //    var responseTime = HttpContext.Request.Query["responseTime"].ToString();
        //    var extraData = HttpContext.Request.Query["extraData"].ToString(); // Dữ liệu bạn đã gửi đi (nếu có)
        //    var signature = HttpContext.Request.Query["signature"].ToString(); // Chữ ký Momo gửi về

        //    // Lấy OrderId và RequestId đã lưu từ Session để kiểm tra
        //    var sessionOrderId = HttpContext.Session.GetString("MomoOrderId");
        //    var sessionRequestId = HttpContext.Session.GetString("MomoRequestId");

        //    if (string.IsNullOrEmpty(sessionOrderId) || string.IsNullOrEmpty(sessionRequestId) ||
        //        sessionOrderId != orderId || sessionRequestId != requestId)
        //    {
        //        ViewBag.Message = "Thông tin giao dịch không khớp hoặc đã hết hạn.";
        //        ViewBag.IsSuccess = false;
        //        return View("PaymentResult");
        //    }

        //    // Xóa session sau khi đã sử dụng
        //    HttpContext.Session.Remove("MomoOrderId");
        //    HttpContext.Session.Remove("MomoRequestId");

        //    // Tạo dictionary chứa các tham số nhận được (trừ signature) để kiểm tra chữ ký
        //    var responseData = new Dictionary<string, string>
        //    {
        //        { "partnerCode", partnerCode },
        //        { "orderId", orderId },
        //        { "requestId", requestId },
        //        { "amount", amount },
        //        { "orderInfo", orderInfo },
        //        { "orderType", orderType },
        //        { "transId", transId },
        //        { "resultCode", resultCode },
        //        { "message", message },
        //        { "payType", payType },
        //        { "responseTime", responseTime },
        //        { "extraData", extraData }
        //        // Thêm các trường khác mà Momo gửi về trong Return URL (trừ signature)
        //    };

        //    // Debug để kiểm tra raw data
        //    var rawData = string.Join("&", responseData.OrderBy(x => x.Key).Select(kvp => $"{kvp.Key}={kvp.Value}"));
        //    Console.WriteLine($"Raw Data: {rawData}");
        //    Console.WriteLine($"Received Signature: {signature}");

        //    bool isValidSignature = _momoService.ValidateSignature(responseData, _momoSettings.SecretKey, signature);

        //    if (isValidSignature)
        //    {
        //        if (resultCode == "0") // Mã 0 là thành công
        //        {
        //            ViewBag.Message = $"Thanh toán thành công cho đơn hàng {orderId}! Mã giao dịch Momo: {transId}. {message}";
        //            ViewBag.IsSuccess = true;
        //            // TODO: Cập nhật trạng thái đơn hàng trong database của bạn là đã thanh toán
        //            // TODO: Lưu thông tin giao dịch MomoTransaction vào database (nếu cần)
        //            // Ví dụ:
        //            // var transaction = new MomoTransaction { ... };
        //            // _context.MomoTransactions.Add(transaction);
        //            // await _context.SaveChangesAsync();
        //        }
        //        else
        //        {
        //            ViewBag.Message = $"Thanh toán thất bại: {message} (Mã lỗi: {resultCode})";
        //            ViewBag.IsSuccess = false;
        //        }
        //    }
        //    else
        //    {
        //        ViewBag.Message = "Chữ ký không hợp lệ! Giao dịch có thể đã bị thay đổi.";
        //        ViewBag.IsSuccess = false;
        //        // TODO: Log trường hợp chữ ký không hợp lệ này lại
        //    }

        //    return View("PaymentResult");
        //}

        [HttpPost]
        public async Task<IActionResult> PaymentNotify() // Momo sẽ POST dữ liệu IPN về URL này
        {
            // Đọc raw body của request
            string rawRequestBody;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                rawRequestBody = await reader.ReadToEndAsync();
            }

            // Log raw IPN data for debugging
            Console.WriteLine($"MoMo IPN Received: {rawRequestBody}");

            try
            {
                var ipnData = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(rawRequestBody);
                if (ipnData == null)
                {
                    // Log error or return bad request
                    return BadRequest("Invalid IPN data format.");
                }

                // Chuyển đổi JsonElement sang string cho việc kiểm tra chữ ký
                var ipnDataStrings = ipnData.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.ValueKind == JsonValueKind.String ? kvp.Value.GetString() : kvp.Value.GetRawText()
                );

                var signature = ipnDataStrings.ContainsKey("signature") ? ipnDataStrings["signature"] : null;
                if (string.IsNullOrEmpty(signature))
                {
                    return BadRequest("Signature missing in IPN.");
                }



                bool isValidSignature = _momoService.ValidateSignature(ipnDataStrings, _momoSettings.SecretKey, signature);

                if (isValidSignature)
                {
                    var resultCode = ipnDataStrings.ContainsKey("resultCode") ? ipnDataStrings["resultCode"] : "-1";
                    var orderId = ipnDataStrings.ContainsKey("orderId") ? ipnDataStrings["orderId"] : null;
                    var message = ipnDataStrings.ContainsKey("message") ? ipnDataStrings["message"] : "N/A";
                    var transId = ipnDataStrings.ContainsKey("transId") ? ipnDataStrings["transId"] : "N/A";

                    if (resultCode == "0")
                    {
                        // TODO: Xử lý logic khi thanh toán thành công qua IPN
                        // - Kiểm tra xem orderId đã được xử lý chưa để tránh xử lý trùng lặp.
                        // - Cập nhật trạng thái đơn hàng trong database.
                        // - Lưu thông tin giao dịch.
                        Console.WriteLine($"IPN: Payment success for OrderId {orderId}, TransId {transId}. Message: {message}");
                    }
                    else
                    {
                        // TODO: Xử lý logic khi thanh toán thất bại qua IPN
                        Console.WriteLine($"IPN: Payment failed for OrderId {orderId}. ResultCode: {resultCode}, Message: {message}");
                    }

                    // Phản hồi cho Momo (theo tài liệu của Momo, thường là JSON)
                    // Ví dụ: {"partnerCode":"YOUR_PARTNER_CODE","requestId":"UNIQUE_REQUEST_ID","orderId":"ORDER_ID","resultCode":0,"message":"Confirm success"}
                    // Quan trọng: Phải phản hồi đúng định dạng Momo yêu cầu để họ không gửi lại IPN.
                    var ackResponse = new
                    {
                        partnerCode = _momoSettings.PartnerCode,
                        requestId = ipnDataStrings.ContainsKey("requestId") ? ipnDataStrings["requestId"] : Guid.NewGuid().ToString(),
                        orderId = orderId,
                        resultCode = 0, // Báo cho Momo là đã nhận và xử lý thành công (không phải là resultCode của giao dịch)
                        message = "Successfully processed IPN."
                    };
                    return Ok(ackResponse);
                }
                else
                {
                    // Log IPN signature validation failed
                    Console.WriteLine("IPN: Invalid signature.");
                    // Phản hồi lỗi cho Momo nếu cần, hoặc chỉ log và không xử lý
                    var errorResponse = new
                    {
                        partnerCode = _momoSettings.PartnerCode,
                        requestId = ipnDataStrings.ContainsKey("requestId") ? ipnDataStrings["requestId"] : Guid.NewGuid().ToString(),
                        orderId = ipnDataStrings.ContainsKey("orderId") ? ipnDataStrings["orderId"] : "N/A",
                        resultCode = 99, // Mã lỗi tùy ý cho việc chữ ký không hợp lệ
                        message = "Invalid signature."
                    };
                    return BadRequest(errorResponse); // Hoặc Ok với mã lỗi để Momo không gửi lại
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"IPN Deserialization Error: {jsonEx.Message}");
                return BadRequest("Error processing IPN data.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IPN General Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Internal server error processing IPN.");
            }
        }

        // View để hiển thị kết quả chung
        public IActionResult PaymentResult()
        {
            ViewBag.Message = TempData["MomoMessage"] ?? ViewBag.Message;
            ViewBag.IsSuccess = TempData["MomoIsSuccess"] ?? ViewBag.IsSuccess;
            ViewBag.Error = TempData["MomoError"]; // Hiển thị lỗi nếu có
            return View();
        }

        // Action để test simulate thanh toán thành công (chỉ dùng cho development)
        [HttpGet]
        public IActionResult SimulateSuccess(string? orderId = null, string amount = "10000")
        {
            var mockOrderId = orderId ?? Guid.NewGuid().ToString();
            var mockRequestId = Guid.NewGuid().ToString();
            var mockTransId = new Random().Next(1000000, 9999999).ToString();

            // Simulate successful payment return
            ViewBag.Message = $"Thanh toán thành công cho đơn hàng {mockOrderId}! Mã giao dịch Momo: {mockTransId}. (SIMULATION)";
            ViewBag.IsSuccess = true;

            return View("PaymentResult");
        }

        // Action để test simulate thanh toán thất bại (chỉ dùng cho development)
        [HttpGet]
        public IActionResult SimulateFailure(string? orderId = null)
        {
            var mockOrderId = orderId ?? Guid.NewGuid().ToString();

            ViewBag.Message = $"Thanh toán thất bại cho đơn hàng {mockOrderId}: Người dùng hủy giao dịch (Mã lỗi: 1006) (SIMULATION)";
            ViewBag.IsSuccess = false;

            return View("PaymentResult");
        }

        [HttpPost]
        public IActionResult CreatePaymentMock(long amount, string orderInfo) // Test thanh toán mock
        {
            if (amount <= 0)
            {
                TempData["MomoError"] = "Số tiền không hợp lệ.";
                return RedirectToAction("PaymentResult");
            }

            var orderId = Guid.NewGuid().ToString();
            var requestId = Guid.NewGuid().ToString();

            // Lưu thông tin để simulate callback
            HttpContext.Session.SetString("MomoOrderId", orderId);
            HttpContext.Session.SetString("MomoRequestId", requestId);
            HttpContext.Session.SetString("MomoAmount", amount.ToString());
            HttpContext.Session.SetString("MomoOrderInfo", orderInfo);

            // Redirect đến trang mock payment
            return RedirectToAction("MockPaymentPage", new { orderId, amount, orderInfo });
        }

        [HttpGet]
        public IActionResult MockPaymentPage(string orderId, long amount, string orderInfo)
        {
            ViewBag.OrderId = orderId;
            ViewBag.Amount = amount;
            ViewBag.OrderInfo = orderInfo;
            return View();
        }

        [HttpPost]
        public IActionResult ProcessMockPayment(string orderId, string action) // action = "success" hoặc "cancel"
        {
            var sessionOrderId = HttpContext.Session.GetString("MomoOrderId");
            var sessionRequestId = HttpContext.Session.GetString("MomoRequestId");
            var sessionAmount = HttpContext.Session.GetString("MomoAmount");
            var sessionOrderInfo = HttpContext.Session.GetString("MomoOrderInfo");

            if (sessionOrderId != orderId)
            {
                ViewBag.Message = "Thông tin giao dịch không khớp.";
                ViewBag.IsSuccess = false;
                return View("PaymentResult");
            }

            // Clear session
            HttpContext.Session.Remove("MomoOrderId");
            HttpContext.Session.Remove("MomoRequestId");
            HttpContext.Session.Remove("MomoAmount");
            HttpContext.Session.Remove("MomoOrderInfo");

            if (action == "success")
            {
                var mockTransId = new Random().Next(1000000, 9999999).ToString();

                // TODO: Xử lý đơn hàng thành công ở đây
                // - Cập nhật trạng thái đơn hàng
                // - Trừ số lượng sản phẩm
                // - Gửi email xác nhận
                // - Lưu thông tin giao dịch
                ProcessSuccessfulPayment(orderId, sessionAmount, sessionOrderInfo, mockTransId);

                ViewBag.Message = $"Thanh toán thành công cho đơn hàng {orderId}! Mã giao dịch Mock: {mockTransId}. (MOCK PAYMENT)";
                ViewBag.IsSuccess = true;
            }
            else
            {
                // TODO: Xử lý hủy thanh toán ở đây
                ProcessCancelledPayment(orderId, sessionAmount, sessionOrderInfo);

                ViewBag.Message = $"Thanh toán bị hủy cho đơn hàng {orderId}. (MOCK PAYMENT)";
                ViewBag.IsSuccess = false;
            }

            return View("PaymentResult");
        }

        private void ProcessSuccessfulPayment(string orderId, string? amount, string? orderInfo, string transId)
        {
            // TODO: Implement logic xử lý thanh toán thành công
            Console.WriteLine($"Processing successful payment - OrderId: {orderId}, Amount: {amount}, TransId: {transId}");

            // Ví dụ logic thực tế:
            try
            {
                // 1. Cập nhật trạng thái đơn hàng trong database
                // using var context = new TAF_DbContext(); // Inject DbContext nếu cần
                // var order = context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                // if (order != null)
                // {
                //     order.Status = "Paid";
                //     order.PaymentMethod = "Momo";
                //     order.TransactionId = transId;
                //     order.PaymentDate = DateTime.Now;
                //     context.SaveChanges();
                // }

                // 2. Trừ số lượng sản phẩm trong kho
                // UpdateProductQuantity(orderId);

                // 3. Gửi email xác nhận đến khách hàng
                // await _emailService.SendOrderConfirmationEmail(order);

                // 4. Lưu thông tin giao dịch vào bảng PaymentTransactions
                // var transaction = new PaymentTransaction
                // {
                //     OrderId = orderId,
                //     TransactionId = transId,
                //     Amount = decimal.Parse(amount ?? "0"),
                //     PaymentMethod = "Momo",
                //     Status = "Success",
                //     CreatedDate = DateTime.Now
                // };
                // context.PaymentTransactions.Add(transaction);
                // context.SaveChanges();

                // 5. Clear giỏ hàng (nếu có)
                HttpContext.Session.Remove("cart");

                Console.WriteLine($"Successfully processed payment for order {orderId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing successful payment: {ex.Message}");
                // Log error but don't throw - payment was successful on Momo side
            }
        }

        private void ProcessCancelledPayment(string orderId, string? amount, string? orderInfo)
        {
            // TODO: Implement logic xử lý hủy thanh toán
            Console.WriteLine($"Processing cancelled payment - OrderId: {orderId}, Amount: {amount}");

            try
            {
                // 1. Cập nhật trạng thái đơn hàng là đã hủy (nếu cần)
                // using var context = new TAF_DbContext();
                // var order = context.Orders.FirstOrDefault(o => o.OrderId == orderId);
                // if (order != null)
                // {
                //     order.Status = "Cancelled";
                //     order.CancelledDate = DateTime.Now;
                //     context.SaveChanges();
                // }

                // 2. Log thông tin hủy thanh toán
                // var cancelLog = new PaymentCancelLog
                // {
                //     OrderId = orderId,
                //     Amount = decimal.Parse(amount ?? "0"),
                //     Reason = "User cancelled payment",
                //     CreatedDate = DateTime.Now
                // };
                // context.PaymentCancelLogs.Add(cancelLog);
                // context.SaveChanges();

                Console.WriteLine($"Payment cancelled for order {orderId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing cancelled payment: {ex.Message}");
            }
        }
    }
}
