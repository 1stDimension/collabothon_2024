using System;
using System.Collections.Generic;

namespace SomethingFishy.Collabothon2024.Common.Models;

// Generated using Gemini Advanced Artificial Intelligence, refined by Emzi Human Intelligence.

public sealed class CommerzCustomer
{

    /// <summary>
    /// Data of the customer
    /// </summary>
    public CommerzNaturalPerson NaturalPerson { get; set; }
}

public sealed class CommerzNaturalPerson
{

    /// <summary>
    /// The last name of the customer
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// The first name of the customer
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// The date of birth of the customer
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// The salutation for the customer
    /// </summary>
    public string Salutation { get; set; }

    /// <summary>
    /// The academic title of the customer (if existing and known)
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// A list of phone numbers of the customers. Returns all private mobile numbers and all private landline numbers known for the customer
    /// </summary>
    public IEnumerable<CommerzPhoneNumber> PhoneNumbers { get; set; }

    /// <summary>
    /// A list of all known private e-mail addresses of the customer
    /// </summary>
    public IEnumerable<CommerzEmailAddress> EmailAddresses { get; set; }

    /// <summary>
    /// A list of postal addresses of the customer. Always contains the legal address of the customer
    /// </summary>
    public IEnumerable<CommerzPostalAddress> PostalAddresses { get; set; }

    /// <summary>
    /// All known nationalities of the customer, as ISO codes
    /// </summary>
    public IEnumerable<string> Nationalities { get; set; }
}

public sealed class CommerzPostalAddress
{

    /// <summary>
    /// The street name of the address
    /// </summary>
    public string Street { get; set; }

    /// <summary>
    /// The street number / house number of the address
    /// </summary>
    public string StreetNumber { get; set; }

    /// <summary>
    /// The city name of the address
    /// </summary>
    public string City { get; set; }

    /// <summary>
    /// The postal Zone Improvement Plan-code of the address
    /// </summary>
    public string ZipCode { get; set; }

    /// <summary>
    /// The country of the address, as ISO code
    /// </summary>
    public string Country { get; set; }

    /// <summary>
    /// The type of the address, always 'legal address'
    /// </summary>
    public CommerzAddressType AddressType { get; set; }
}

public sealed class CommerzEmailAddress
{

    /// <summary>
    /// The e-mail address
    /// </summary>
    public string EmailAddress { get; set; }
}

public sealed class CommerzPhoneNumber
{

    /// <summary>
    /// The phone number of the corresponding type
    /// </summary>
    public string PhoneNumber { get; set; }

    /// <summary>
    /// Type of the phone number: mobile number ('mobile phone') or landline number ('phone')
    /// </summary>
    public CommerzPhoneType PhoneType { get; set; }
}

public enum CommerzPhoneType
{
    Mobile,
    Landline,
}

public enum CommerzAddressType
{
    Legal,
}
