namespace YaSha.DataManager.BasicManagement.OrganizationUnits.Dto;

public class CreateOrganizationUnitInput : IValidatableObject
{
    public string DisplayName { get; set; }

    public Guid? ParentId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var localization = validationContext.GetRequiredService<IStringLocalizer<DataManagerLocalizationResource>>();
        if (DisplayName.IsNullOrWhiteSpace())
        {
            yield return new ValidationResult(
                localization[DataManagerLocalizationErrorCodes.ErrorCode100003, nameof(DisplayName)],
                new[] {  nameof(DisplayName) }
            );
        }
    }
}