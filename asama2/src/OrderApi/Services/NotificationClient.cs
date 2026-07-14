using System.Net.Http.Json;
using OrderApi.Models;

namespace OrderApi.Services;

public interface INotificationClient
{
    Task NotifyOrderCreatedAsync(Order order, CancellationToken cancellationToken = default);
}

public class NotificationClient : INotificationClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationClient> _logger;

    public NotificationClient(HttpClient httpClient, ILogger<NotificationClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task NotifyOrderCreatedAsync(
        Order order,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var payload = new
            {
                order.OrderNumber,
                order.CustomerName,
                order.TotalPrice
            };

            using var response = await _httpClient.PostAsJsonAsync(
                "api/notify",
                payload,
                cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Bildirim servisi {StatusCode} durum kodu döndürdü. Sipariş: {OrderNumber}",
                    (int)response.StatusCode,
                    order.OrderNumber);
            }
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "Bildirim servisi zaman aşımına uğradı. Sipariş: {OrderNumber}",
                order.OrderNumber);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogWarning(
                exception,
                "Bildirim servisine ulaşılamadı. Sipariş: {OrderNumber}",
                order.OrderNumber);
        }
    }
}
