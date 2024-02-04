namespace YaSha.DataManager.StandardAndPolicy.Dto;

public class StandardAndPolicyCreateAndUpdateDto
{
    public StandardAndPolicyLibDto Data { get; set; }
    
    public List<Guid> Themes { get; set; }

    public StandardAndPolicyCreateAndUpdateDto()
    {
        Themes = new List<Guid>();
    }

    public StandardAndPolicyCreateAndUpdateDto(List<Guid> themes)
    {
        Themes = themes;
    }
}