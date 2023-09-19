namespace YaSha.DataManager.BasicManagement.Tenants.Dtos
{
    public class UpdateConnectionStringInput : IValidatableObject
    {
        public Guid Id { get; set; }
        public string ConnectionString { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localization = validationContext.GetRequiredService<IStringLocalizer<DataManagerLocalizationResource>>();
            if (ConnectionString.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult(
                    localization[DataManagerLocalizationErrorCodes.ErrorCode100003, nameof(ConnectionString)],
                    new[] { nameof(ConnectionString) }
                );
            }
        }
    }
}