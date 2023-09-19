namespace YaSha.DataManager.Settings
{
    public class DataManagerSettingDefinitionProvider : SettingDefinitionProvider
    {
        public override void Define(ISettingDefinitionContext context)
        {
            ConfigEmail(context);
        }

       private static void ConfigEmail(ISettingDefinitionContext context)
        {
            context.GetOrNull(EmailSettingNames.Smtp.Host)
                .WithProperty(DataManagerSettings.Group.Default,
                    DataManagerSettings.Group.EmailManagement)
                .WithProperty(DataManagerSettings.ControlType.Default,
                    DataManagerSettings.ControlType.TypeText);

            context.GetOrNull(EmailSettingNames.Smtp.Port)
                .WithProperty(DataManagerSettings.Group.Default,
                    DataManagerSettings.Group.EmailManagement)
                .WithProperty(DataManagerSettings.ControlType.Default,
                    DataManagerSettings.ControlType.Number);

            context.GetOrNull(EmailSettingNames.Smtp.UserName)
                .WithProperty(DataManagerSettings.Group.Default,
                    DataManagerSettings.Group.EmailManagement)
                .WithProperty(DataManagerSettings.ControlType.Default,
                    DataManagerSettings.ControlType.TypeText);

            context.GetOrNull(EmailSettingNames.Smtp.Password)
                .WithProperty(DataManagerSettings.Group.Default,
                    DataManagerSettings.Group.EmailManagement)
                .WithProperty(DataManagerSettings.ControlType.Default,
                    DataManagerSettings.ControlType.TypeText);
            

            context.GetOrNull(EmailSettingNames.Smtp.EnableSsl)
                .WithProperty(DataManagerSettings.Group.Default,
                    DataManagerSettings.Group.EmailManagement)
                .WithProperty(DataManagerSettings.ControlType.Default,
                    DataManagerSettings.ControlType.TypeCheckBox);

            context.GetOrNull(EmailSettingNames.Smtp.UseDefaultCredentials)
                .WithProperty(DataManagerSettings.Group.Default,
                    DataManagerSettings.Group.EmailManagement)
                .WithProperty(DataManagerSettings.ControlType.Default,
                    DataManagerSettings.ControlType.TypeCheckBox);

            context.GetOrNull(EmailSettingNames.DefaultFromAddress)
                .WithProperty(DataManagerSettings.Group.Default,
                    DataManagerSettings.Group.EmailManagement)
                .WithProperty(DataManagerSettings.ControlType.Default,
                    DataManagerSettings.ControlType.TypeText);
            
            context.GetOrNull(EmailSettingNames.DefaultFromDisplayName)
                .WithProperty(DataManagerSettings.Group.Default,
                    DataManagerSettings.Group.EmailManagement)
                .WithProperty(DataManagerSettings.ControlType.Default,
                    DataManagerSettings.ControlType.TypeText);
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<DataManagerResource>(name);
        }
    }
}