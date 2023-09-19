namespace YaSha.DataManager.DataDictionaryManagement.DataDictionaries.Dtos
{
    public class CreateDataDictinaryDetailInput : IValidatableObject
    {
        public Guid Id { get; set; }

        public string Code { get; set; }

        public string DisplayText { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localization = validationContext.GetRequiredService<IStringLocalizer<DataManagerLocalizationResource>>();
            if (Code.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult(
                    localization[DataManagerLocalizationErrorCodes.ErrorCode100003, nameof(Code)],
                    new[] { nameof(Code) }
                );
            }

            if (DisplayText.IsNullOrWhiteSpace())
            {
                yield return new ValidationResult(
                    localization[DataManagerLocalizationErrorCodes.ErrorCode100003, nameof(DisplayText)],
                    new[] { nameof(DisplayText) }
                );
            }
        }
    }
}