using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using ThienAnFuni.Models.Momo;
using Microsoft.Extensions.Options;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Text.Json;

namespace ThienAnFuni.Services.Momo
{
    public interface IMomoService
    {
        Task<MomoPaymentResponse> CreatePaymentAsync(long amount, string orderId, string orderInfo, string extraData = "");
        string GenerateSignature(string data, string secretKey);
        bool ValidateSignature(Dictionary<string, string> responseData, string secretKey, string expectedSignature);
    }

    public class MomoService : IMomoService
    {
        private readonly HttpClient _httpClient;
        private readonly MomoSettings _momoSettings;

        public MomoService(IHttpClientFactory httpClientFactory, IOptions<MomoSettings> momoSettings)
        {
            _httpClient = httpClientFactory.CreateClient("MomoClient");
            _momoSettings = momoSettings.Value;
        }
        public async Task<MomoPaymentResponse> CreatePaymentAsync(long amount, string orderId, string orderInfo, string extraData = "")
        {
            var requestId = Guid.NewGuid().ToString();
            var requestType = "captureWallet";

            // Tạo rawHash theo đúng thứ tự yêu cầu của Momo
            var rawHash = $"accessKey={_momoSettings.AccessKey}" +
                          $"&amount={amount}" +
                          $"&extraData={extraData}" +
                          $"&ipnUrl={_momoSettings.NotifyUrl}" +
                          $"&orderId={orderId}" +
                          $"&orderInfo={orderInfo}" +
                          $"&partnerCode={_momoSettings.PartnerCode}" +
                          $"&redirectUrl={_momoSettings.ReturnUrl}" +
                          $"&requestId={requestId}" +
                          $"&requestType={requestType}";

            Console.WriteLine("=== Momo Create Payment Debug ===");
            Console.WriteLine($"Raw Data for Create Payment: {rawHash}");

            var signature = GenerateSignature(rawHash, _momoSettings.SecretKey);
            Console.WriteLine($"Generated Signature: {signature}");

            var requestPayload = new MomoPaymentRequest
            {
                PartnerCode = _momoSettings.PartnerCode,
                AccessKey = _momoSettings.AccessKey,
                RequestId = requestId,
                Amount = amount,
                OrderId = orderId,
                OrderInfo = orderInfo,
                RedirectUrl = _momoSettings.ReturnUrl,
                IpnUrl = _momoSettings.NotifyUrl,
                RequestType = requestType,
                ExtraData = extraData,
                Lang = "vi",
                Signature = signature,
                PartnerName = "Test Partner",
                StoreId = "TestStore"
            };

            Console.WriteLine($"Request Payload JSON: {JsonSerializer.Serialize(requestPayload, new JsonSerializerOptions { WriteIndented = true })}");

            try
            {
                var response = await _httpClient.PostAsJsonAsync(_momoSettings.Endpoint, requestPayload);
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine($"Momo Response Status: {response.StatusCode}");
                Console.WriteLine($"Momo Response Content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new ApplicationException($"HTTP Error: {response.StatusCode} - {responseContent}");
                }

                var momoResponse = JsonSerializer.Deserialize<MomoPaymentResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return momoResponse ?? throw new ApplicationException("Failed to deserialize MoMo response.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling Momo API: {ex.Message}");
                throw new ApplicationException($"Error calling MoMo API: {ex.Message}", ex);
            }
        }

        //public async Task<MomoPaymentResponse> CreatePaymentAsync(long amount, string orderId, string orderInfo, string extraData = "")
        //{
        //    var requestId = Guid.NewGuid().ToString();
        //    var requestType = "captureWallet"; // Hoặc "payWithATM" tùy nhu cầu

        //    // Chuỗi dữ liệu để tạo chữ ký
        //    var rawHash = $"accessKey={_momoSettings.AccessKey}" +
        //                  $"&amount={amount}" +
        //                  $"&extraData={extraData}" +
        //                  $"&ipnUrl={_momoSettings.NotifyUrl}" +
        //                  $"&orderId={orderId}" +
        //                  $"&orderInfo={orderInfo}" +
        //                  $"&partnerCode={_momoSettings.PartnerCode}" +
        //                  $"&redirectUrl={_momoSettings.ReturnUrl}" +
        //                  $"&requestId={requestId}" +
        //                  $"&requestType={requestType}";

        //    var signature = GenerateSignature(rawHash, _momoSettings.SecretKey);

        //    var requestPayload = new MomoPaymentRequest
        //    {
        //        PartnerCode = _momoSettings.PartnerCode,
        //        RequestId = requestId,
        //        Amount = amount,
        //        OrderId = orderId,
        //        OrderInfo = orderInfo,
        //        RedirectUrl = _momoSettings.ReturnUrl,
        //        IpnUrl = _momoSettings.NotifyUrl,
        //        RequestType = requestType,
        //        ExtraData = extraData, // Nếu có dữ liệu, cần Base64 Encode
        //        Lang = "vi",
        //        Signature = signature,
        //        PartnerName = "Test Partner", // Tên đối tác của bạn
        //        StoreId = "TestStore" // ID cửa hàng của bạn
        //    };

        //    try
        //    {
        //        var response = await _httpClient.PostAsJsonAsync(_momoSettings.Endpoint, requestPayload);
        //        response.EnsureSuccessStatusCode();

        //        var momoResponse = await response.Content.ReadFromJsonAsync<MomoPaymentResponse>();
        //        if (momoResponse == null)
        //        {
        //            throw new ApplicationException("Failed to deserialize MoMo response.");
        //        }
        //        return momoResponse;
        //    }
        //    catch (HttpRequestException httpEx)
        //    {
        //        throw new ApplicationException($"Error calling MoMo API: {httpEx.Message}", httpEx);
        //    }
        //    catch (JsonException jsonEx)
        //    {
        //        Console.WriteLine($"JSON deserialization error: {jsonEx.Message}");
        //        throw new ApplicationException($"Error deserializing MoMo response: {jsonEx.Message}", jsonEx);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Generic error: {ex.Message}");
        //        throw new ApplicationException($"An unexpected error occurred: {ex.Message}", ex);
        //    }
        //}

        public string GenerateSignature(string data, string secretKey)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        // Hàm kiểm tra chữ ký cho IPN hoặc Return URL (nếu Momo có gửi signature về)
        public bool ValidateSignature(Dictionary<string, string> responseData, string secretKey, string expectedSignature)
        {
            // Tạo rawHash theo đúng thứ tự yêu cầu của Momo cho Return URL
            var rawHash = $"accessKey={_momoSettings.AccessKey}" +
                          $"&amount={responseData.GetValueOrDefault("amount", "")}" +
                          $"&extraData={responseData.GetValueOrDefault("extraData", "")}" +
                          $"&message={responseData.GetValueOrDefault("message", "")}" +
                          $"&orderId={responseData.GetValueOrDefault("orderId", "")}" +
                          $"&orderInfo={responseData.GetValueOrDefault("orderInfo", "")}" +
                          $"&orderType={responseData.GetValueOrDefault("orderType", "")}" +
                          $"&partnerCode={responseData.GetValueOrDefault("partnerCode", "")}" +
                          $"&payType={responseData.GetValueOrDefault("payType", "")}" +
                          $"&requestId={responseData.GetValueOrDefault("requestId", "")}" +
                          $"&responseTime={responseData.GetValueOrDefault("responseTime", "")}" +
                          $"&resultCode={responseData.GetValueOrDefault("resultCode", "")}" +
                          $"&transId={responseData.GetValueOrDefault("transId", "")}";

            var calculatedSignature = GenerateSignature(rawHash, secretKey);

            // ✅ Ghi log debug
            Console.WriteLine("=== Momo Signature Debug ===");
            Console.WriteLine("RawHash:");
            Console.WriteLine(rawHash);
            Console.WriteLine("Expected Signature:");
            Console.WriteLine(expectedSignature);
            Console.WriteLine("Calculated Signature:");
            Console.WriteLine(calculatedSignature);
            Console.WriteLine("Is Valid: " + (calculatedSignature == expectedSignature));

            return calculatedSignature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);
        }

        // public bool ValidateSignature(Dictionary<string, string> responseData, string secretKey, string expectedSignature)
        // {
        //     // Sắp xếp các tham số theo thứ tự alphabet của key
        //     // Loại bỏ tham số 'signature' ra khỏi chuỗi tạo hash
        //     var checkData = responseData.Where(kv => kv.Key != "signature")
        //                                 .OrderBy(kv => kv.Key)
        //                                 .Select(kv => $"{kv.Key}={kv.Value}")
        //                                 .ToList();
        //     var rawHash = string.Join("&", checkData);

        //     var calculatedSignature = GenerateSignature(rawHash, secretKey);
        //     return calculatedSignature.Equals(expectedSignature, StringComparison.OrdinalIgnoreCase);
        // }
    }
}
