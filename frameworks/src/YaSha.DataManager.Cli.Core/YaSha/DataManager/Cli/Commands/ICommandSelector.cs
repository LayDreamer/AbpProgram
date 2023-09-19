namespace YaSha.DataManager.Cli.Commands;

public interface ICommandSelector
{
    Type Select(CommandLineArgs commandLineArgs);
}