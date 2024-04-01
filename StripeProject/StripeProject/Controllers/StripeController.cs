namespace StripeProject.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using StripeProject.Dtos;

[Route("api/[controller]")]
[ApiController]
public class StripeController : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> AddInvoiceAsync([FromBody] AddInvoiceDto addInvoiceDto)
    {
        var customerId = await CustomerHanleAsync(addInvoiceDto);
        var invoice = await CreateInvoiceAsync(addInvoiceDto, customerId);

        var paymentService = new PaymentIntentService();
        var payment = await paymentService.GetAsync(invoice.PaymentIntentId);

        return Ok(payment.PaymentMethodOptions.Link);

    }

    private static async Task<Invoice> CreateInvoiceAsync(AddInvoiceDto addInvoiceDto, string customerId)
    {
        var invoiceOption = new InvoiceCreateOptions
        {
            Customer = customerId,
            Description = $"invocie number {addInvoiceDto.InvoiceNumber}"
        };

        var invoiceService = new InvoiceService();
        var invoice = await invoiceService.CreateAsync(invoiceOption);

        invoice.CustomerId = customerId;
        invoice.AccountCountry = addInvoiceDto.CurrencyCode;
        invoice.AmountPaid = addInvoiceDto.InvoiceAmount;
        invoice.AccountName = addInvoiceDto.ServiceName;

        return invoice;
    }

    private static async Task<string> CustomerHanleAsync(AddInvoiceDto addInvoiceDto)
    {
        var searchCustomer = new CustomerSearchOptions
        {
            Query = $"email : \"{addInvoiceDto.CustomerEmail}\"",
        };

        var customerService = new CustomerService();
        var customer = await customerService.SearchAsync(searchCustomer);

        if (customer is null)
        {
            var createCustomer = new CustomerCreateOptions
            {
                Name = $"{addInvoiceDto.CustomerName}",
                Email = $"{addInvoiceDto.CustomerEmail}",
                Phone = $"{addInvoiceDto.CustomerPhone}"
            };

            await customerService.CreateAsync(createCustomer);
        }

        return customer!.First().Id;
    }
}
