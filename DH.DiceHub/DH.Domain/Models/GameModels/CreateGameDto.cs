namespace DH.Domain.Models.GameModels;

public class CreateGameDto : IValidableFields
{
    public string Name { get; set; }
    public CreateGameDto(string name)
    {
        Name = name;
    }

    public bool FieldsAreValied()
    {
        bool isValid = true;

        if (Name.Trim().Length < 1)
            isValid = false;

        return isValid;
    }

    public dynamic ValidateFields()
    {
        throw new NotImplementedException();
    }
}
