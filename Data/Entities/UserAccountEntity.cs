using Microsoft.AspNetCore.Identity;

namespace Data.Entities;

public class UserAccountEntity : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Biography { get; set; }
    public string? ProfileImage { get; set; } = "profile-placeholder.svg";
    public bool IsExternalAccount { get; set; } = false;

    public string? AddressId { get; set; }
    public UserAddressEntity? Address { get; set; }
}
