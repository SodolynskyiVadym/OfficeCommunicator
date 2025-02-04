﻿using System.Text.Json.Serialization;

namespace OfficeCommunicatorAPI.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string UniqueName  { get; set; }

    public string ZoomUrl { get; set; }
    public string AzureToken { get; set; }
    public string AzureIdentity { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }

    [JsonIgnore]
    public IEnumerable<Group> Groups { get; set; }

    [JsonIgnore]
    public IEnumerable<Contact> Contacts { get; set; }

    public void HideSensitiveData()
    {
        PasswordHash = new byte[0];
        PasswordSalt = new byte[0];
    }

    public void HideUnnecessaryData()
    {
        AzureToken = string.Empty;
        AzureIdentity = string.Empty;
        PasswordHash = new byte[0];
        PasswordSalt = new byte[0];
    }
}