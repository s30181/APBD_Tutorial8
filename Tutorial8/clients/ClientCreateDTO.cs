using System.ComponentModel.DataAnnotations;

namespace Tutorial8.clients;

public class ClientCreateDTO
{
    [Required, StringLength(50)]
    public string FirstName { get; set;}
    
    [Required, StringLength(50)]
    public string LastName { get; set;}
    
    [Required, EmailAddress]
    public string Email { get; set;}
    
    [Required, RegularExpression(@"^\+\d{11}$")]
    public string Telephone { get; set;}
    
    [Required, RegularExpression(@"^\d{11}$")]
    public string Pesel { get; set;}
}