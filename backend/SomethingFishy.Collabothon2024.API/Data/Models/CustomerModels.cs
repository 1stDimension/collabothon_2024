using System;
using System.Collections.Generic;

namespace SomethingFishy.Collabothon2024.API.Data.Models;

public sealed class CustomerModel
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public DateOnly DateOfBirth { get; init; }
    public string Salutation { get; init; }
    public string Title { get; init; }
    public IEnumerable<CustomerPhoneNumberModel> PhoneNumbers { get; init; }
    public IEnumerable<string> EmailAddresses { get; init; }
    public IEnumerable<CustomerPostalAddressModel> PostalAddresses { get; init; }
    public IEnumerable<string> Nationalities { get; init; }
}

public sealed class CustomerPhoneNumberModel
{
    public string Number { get; init; }
    public PhoneNumberType Type { get; init; }
}

public sealed class CustomerPostalAddressModel
{
    public string Street { get; init; }
    public string StreetNumber { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }
    public string Country { get; init; }
}

public enum PhoneNumberType
{
    Landline,
    Mobile,
}
