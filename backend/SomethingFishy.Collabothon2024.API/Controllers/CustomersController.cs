using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SomethingFishy.Collabothon2024.API.Data.Models;
using SomethingFishy.Collabothon2024.Common;

namespace SomethingFishy.Collabothon2024.API.Controllers;

[ApiController, Authorize]
[Route("/api/v1/[controller]")]
public sealed class CustomersController : ControllerBase
{
    private readonly ICommerzCustomersClient _customers;

    public CustomersController(ICommerzCustomersClient customers)
    {
        this._customers = customers;
    }

    [HttpGet, Route("@me")]
    public async Task<CustomerModel> GetCurrentCustomerAsync(CancellationToken cancellationToken = default)
    {
        if (!this.HttpContext.TryGetCommerzCredentials(out var credentials))
            return null;

        this._customers.AuthorizationToken = credentials.AuthenticationToken;
        var customer = await this._customers.GetCurrentCustomerAsync(cancellationToken);
        return new()
        {
            FirstName = customer.NaturalPerson.FirstName,
            LastName = customer.NaturalPerson.LastName,
            DateOfBirth = customer.NaturalPerson.DateOfBirth,
            Salutation = customer.NaturalPerson.Salutation,
            Title = customer.NaturalPerson.Title,
            PhoneNumbers = customer.NaturalPerson.PhoneNumbers.Select(x => new CustomerPhoneNumberModel
            {
                Number = x.PhoneNumber,
                Type = x.PhoneType switch
                {
                    Common.Models.CommerzPhoneType.Landline => PhoneNumberType.Landline,
                    Common.Models.CommerzPhoneType.Mobile => PhoneNumberType.Mobile,
                }
            }),
            EmailAddresses = customer.NaturalPerson.EmailAddresses.Select(x => x.EmailAddress),
            PostalAddresses = customer.NaturalPerson.PostalAddresses.Select(x => new CustomerPostalAddressModel
            {
                Street = x.Street,
                StreetNumber = x.StreetNumber,
                City = x.City,
                ZipCode = x.ZipCode,
                Country = x.Country,
            }),
            Nationalities = customer.NaturalPerson.Nationalities,
        };
    }
}
