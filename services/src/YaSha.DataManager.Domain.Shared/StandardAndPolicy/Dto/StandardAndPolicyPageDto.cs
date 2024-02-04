namespace YaSha.DataManager.StandardAndPolicy.Dto;

public class StandardAndPolicyPageDto
{
    public string ParentName { get; set; }
    public Guid? Id { get; set; }
    public string Theme { get; set; }
    public List<StandardAndPolicyCardDto> Data { get; set; }

    public StandardAndPolicyPageDto()
    {
        Data = new List<StandardAndPolicyCardDto>();
    }
}

public class StandardAndPolicyCardDto
{
    public Guid Id { get; set; }

    public string Number { get; set; }

    public string Name { get; set; }

    public DateTime CreationTime { get; set; }
}

public class StandardAndPolicyCardDetailDto : StandardAndStandLibWithAllTreeDto
{
    public bool Collect { get; set; }

    public StandardAndPolicyCardDetailDto()
    {
    }
}

public class StandardAndStandLibWithAllTreeDto
{
    public StandardAndPolicyLibDto Lib { get; set; }
    public List<string> Themes { get; set; }
    
    public StandardAndStandLibWithAllTreeDto()
    {
        Themes = new List<string>();
    }
}