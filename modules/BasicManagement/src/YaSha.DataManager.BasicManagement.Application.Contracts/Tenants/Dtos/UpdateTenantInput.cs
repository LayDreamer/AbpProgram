namespace YaSha.DataManager.BasicManagement.Tenants.Dtos
{
    public class UpdateTenantInput : IValidatableObject
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localization = validationContext.GetRequiredService<IStringLocalizer<DataManagerLocalizationResource>>();
            if (Name.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult(
                    localization[DataManagerLocalizationErrorCodes.ErrorCode100003, nameof(Name)],
                    new[] { nameof(Name) }
                );
            }
        }
    }
}