namespace YaSha.DataManager.Cli;

[DependsOn(
    typeof(YaSha.DataManager.Cli.DataManagerCliCoreModule),
    typeof(AbpAutofacModule)
)]
public class DataManagerCliModule : AbpModule
{
}
