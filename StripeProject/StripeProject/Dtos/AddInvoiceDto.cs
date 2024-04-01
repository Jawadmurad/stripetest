namespace StripeProject.Dtos;

public class AddInvoiceDto
{
    public string InvoiceNumber { get; set; } = default!;

    public string CustomerName { get; set; } = default!;

    public string CustomerEmail { get; set; } = default!;

    public string CustomerPhone { get; set; } = default!;

    public long InvoiceAmount { get; set; } = default!;

    public string CurrencyCode { get; set; } = default!;

    public string ServiceName { get; set; } = default!;
}
