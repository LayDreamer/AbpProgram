namespace YaSha.DataManager.LanguageManagement.Settings
{
    public class LanguageManagementSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            var languageManagementSettings = context.GetOrNull(LocalizationSettingNames.DefaultLanguage)
                .WithProperty(LanguageManagementSettings.Group.Default, LanguageManagementSettings.Group.SystemManagement)
                .WithProperty(DataManagerSettingConsts.ControlType.Default, DataManagerSettingConsts.ControlType.TypeText);
            languageManagementSettings.DefaultValue = "zh-Hans";
        }
    }
}