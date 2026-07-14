var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/api/notify", (NotifyRequest request, ILogger<Program> logger) =>
{
    var errors = new Dictionary<string, string[]>();

    if (string.IsNullOrWhiteSpace(request.OrderNumber))
    {
        errors[nameof(request.OrderNumber)] = ["Sipariş numarası zorunludur."];
    }

    if (string.IsNullOrWhiteSpace(request.CustomerName))
    {
        errors[nameof(request.CustomerName)] = ["Müşteri adı zorunludur."];
    }

    if (request.TotalPrice < 0)
    {
        errors[nameof(request.TotalPrice)] = ["Toplam tutar negatif olamaz."];
    }

    if (errors.Count > 0)
    {
        return Results.ValidationProblem(errors);
    }

    logger.LogInformation(
        "[Bildirim] {OrderNumber} numaralı sipariş için {CustomerName} adlı müşteriye bildirim gönderildi. Toplam: {TotalPrice:N2} TL",
        request.OrderNumber,
        request.CustomerName.Trim(),
        request.TotalPrice);

    return Results.Ok(new { sent = true });
});

app.Run();

record NotifyRequest(string OrderNumber, string CustomerName, decimal TotalPrice);
