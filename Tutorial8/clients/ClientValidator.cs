using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tutorial8.clients;

public class ClientValidator
{
    private DatabaseHelper _databaseHelper;

    public ClientValidator(DatabaseHelper databaseHelper)
    {
        _databaseHelper = databaseHelper;
    }


    public async Task Validate(ClientCreateDTO createDto)
    {
        var peselExists = await _databaseHelper.GetScalar<int>(
            "SELECT 1 FROM Client WHERE Client.Pesel = @pesel", 
            new Dictionary<string, object> { { "@pesel", createDto.Pesel } });
        if (peselExists == 1)
        {
            throw new ValidationException("Pesel already exists");
        }
        
        var emailExists = await _databaseHelper.GetScalar<int>(
            "SELECT 1 FROM Client WHERE Client.Email = @email", 
            new Dictionary<string, object> { { "@email", createDto.Email } });
        if (emailExists == 1)
        {
            throw new ValidationException("Email already exists");
        }
        
        var phoneExists = await _databaseHelper.GetScalar<int>(
            "SELECT 1 FROM Client WHERE Client.Telephone = @telephone", 
            new Dictionary<string, object> { { "@telephone", createDto.Telephone } });
        if (phoneExists == 1)
        {
            throw new ValidationException("Telephone already exists");
        }
    }
}