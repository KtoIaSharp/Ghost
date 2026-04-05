using Newtonsoft.Json;
using System.Text;

namespace Ghost.Services;

public class CryptoCloudService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public CryptoCloudService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<string?> CreateInvoiceAsync(decimal amount, string orderId)
    {
        var apiKey = _config["CryptoCloud:ApiKey"];
        var shopId = _config["CryptoCloud:ShopId"];
        var apiUrl = _config["CryptoCloud:ApiUrl"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(shopId))
        {
            // Заглушка для разработки — возвращаем фейковую ссылку
            return $"https://cryptocloud.plus/demo/invoice/{orderId}";
        }

        var payload = new
        {
            shop_id = shopId,
            amount = amount,
            order_id = orderId,
            currency = "USD",
            lifetime = 3600 // 1 час на оплату
        };

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Thread-safe: используем HttpRequestMessage вместо мутации DefaultRequestHeaders
        var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}/invoice/create")
        {
            Content = content
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
            return result?.result?.link;
        }

        return null;
    }

    public async Task<bool> CheckPaymentStatusAsync(string invoiceId)
    {
        var apiKey = _config["CryptoCloud:ApiKey"];
        var shopId = _config["CryptoCloud:ShopId"];
        var apiUrl = _config["CryptoCloud:ApiUrl"];

        if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(shopId))
        {
            // В режиме заглушки считаем все платежи оплаченными
            return true;
        }

        // Реальная проверка статуса через CryptoCloud API
        var request = new HttpRequestMessage(HttpMethod.Post, $"{apiUrl}/invoice/info")
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(new { shop_id = shopId, invoice_id = invoiceId }),
                Encoding.UTF8,
                "application/json"
            )
        };
        request.Headers.Add("Authorization", $"Bearer {apiKey}");

        var response = await _httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
            var status = result?.result?.status?.ToString()?.ToLower();
            return status == "paid" || status == "completed";
        }

        return false;
    }
}
