namespace YaSha.DataManager.Cli.Utils;

public static class ReplacePackageReferenceExtensions
{
    public static string ReplacePackageReferenceCore(this string content)
    {
        return content
                .Replace("<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\frameworks\\src\\YaSha.DataManager.Core\\YaSha.DataManager.Core.csproj\"/>",
                    "<PackageReference Include=\"YaSha.DataManager.Core\"/>")
                .Replace("<ProjectReference Include=\"..\\..\\..\\..\\aspnet-core\\frameworks\\src\\YaSha.DataManager.Core\\YaSha.DataManager.Core.csproj\"/>",
                    "<PackageReference Include=\"YaSha.DataManager.Core\"/>")
                .Replace("<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\shared\\YaSha.DataManager.Shared.Hosting.Microservices\\YaSha.DataManager.Shared.Hosting.Microservices.csproj\"/>",
                    "<PackageReference Include=\"YaSha.DataManager.Shared.Hosting.Microservices\"/>")
                .Replace("<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\shared\\YaSha.DataManager.Shared.Hosting.Gateways\\YaSha.DataManager.Shared.Hosting.Gateways.csproj\"/>",
                    "<PackageReference Include=\"YaSha.DataManager.Shared.Hosting.Gateways\"/>")
            ;
    }

    public static string ReplacePackageReferenceBasicManagement(this string content)
    {
        return content
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\BasicManagement\\src\\YaSha.DataManager.BasicManagement.Application\\YaSha.DataManager.BasicManagement.Application.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.BasicManagement.Application\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\BasicManagement\\src\\YaSha.DataManager.BasicManagement.Application.Contracts\\YaSha.DataManager.BasicManagement.Application.Contracts.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.BasicManagement.Application.Contracts\"/>")
            .Replace("<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\BasicManagement\\src\\YaSha.DataManager.BasicManagement.Domain\\YaSha.DataManager.BasicManagement.Domain.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.BasicManagement.Domain\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\BasicManagement\\src\\YaSha.DataManager.BasicManagement.Domain.Shared\\YaSha.DataManager.BasicManagement.Domain.Shared.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.BasicManagement.Domain.Shared\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\BasicManagement\\src\\YaSha.DataManager.BasicManagement.EntityFrameworkCore\\YaSha.DataManager.BasicManagement.EntityFrameworkCore.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.BasicManagement.EntityFrameworkCore\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\BasicManagement\\src\\YaSha.DataManager.BasicManagement.FreeSqlRepository\\YaSha.DataManager.BasicManagement.FreeSqlRepository.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FreeSqlRepository\"/>")
            .Replace("<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\BasicManagement\\src\\YaSha.DataManager.BasicManagement.HttpApi\\YaSha.DataManager.BasicManagement.HttpApi.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.BasicManagement.HttpApi\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\BasicManagement\\src\\YaSha.DataManager.BasicManagement.HttpApi.Client\\YaSha.DataManager.BasicManagement.HttpApi.Client.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.BasicManagement.HttpApi.Client\"/>");
    }

    public static string ReplacePackageReferenceDataDictionaryManagement(this string content)
    {
        return content
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\DataDictionaryManagement\\src\\YaSha.DataManager.DataDictionaryManagement.Application\\YaSha.DataManager.DataDictionaryManagement.Application.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.DataDictionaryManagement.Application\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\DataDictionaryManagement\\src\\YaSha.DataManager.DataDictionaryManagement.Application.Contracts\\YaSha.DataManager.DataDictionaryManagement.Application.Contracts.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.DataDictionaryManagement.Application.Contracts\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\DataDictionaryManagement\\src\\YaSha.DataManager.DataDictionaryManagement.Domain\\YaSha.DataManager.DataDictionaryManagement.Domain.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.DataDictionaryManagement.Domain\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\DataDictionaryManagement\\src\\YaSha.DataManager.DataDictionaryManagement.Domain.Shared\\YaSha.DataManager.DataDictionaryManagement.Domain.Shared.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.DataDictionaryManagement.Domain.Shared\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\DataDictionaryManagement\\src\\YaSha.DataManager.DataDictionaryManagement.EntityFrameworkCore\\YaSha.DataManager.DataDictionaryManagement.EntityFrameworkCore.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.DataDictionaryManagement.EntityFrameworkCore\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\DataDictionaryManagement\\src\\YaSha.DataManager.DataDictionaryManagement.FreeSqlRepository\\YaSha.DataManager.DataDictionaryManagement.FreeSqlRepository.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FreeSqlRepository\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\DataDictionaryManagement\\src\\YaSha.DataManager.DataDictionaryManagement.HttpApi\\YaSha.DataManager.DataDictionaryManagement.HttpApi.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.DataDictionaryManagement.HttpApi\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\DataDictionaryManagement\\src\\YaSha.DataManager.DataDictionaryManagement.HttpApi.Client\\YaSha.DataManager.DataDictionaryManagement.HttpApi.Client.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.DataDictionaryManagement.HttpApi.Client\"/>");
    }

    public static string ReplacePackageReferenceFileManagement(this string content)
    {
        return content
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\FileManagement\\src\\YaSha.DataManager.FileManagement.Application\\YaSha.DataManager.FileManagement.Application.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FileManagement.Application\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\FileManagement\\src\\YaSha.DataManager.FileManagement.Application.Contracts\\YaSha.DataManager.FileManagement.Application.Contracts.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FileManagement.Application.Contracts\"/>")
            .Replace("<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\FileManagement\\src\\YaSha.DataManager.FileManagement.Domain\\YaSha.DataManager.FileManagement.Domain.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FileManagement.Domain\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\FileManagement\\src\\YaSha.DataManager.FileManagement.Domain.Shared\\YaSha.DataManager.FileManagement.Domain.Shared.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FileManagement.Domain.Shared\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\FileManagement\\src\\YaSha.DataManager.FileManagement.EntityFrameworkCore\\YaSha.DataManager.FileManagement.EntityFrameworkCore.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FileManagement.EntityFrameworkCore\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\FileManagement\\src\\YaSha.DataManager.FileManagement.FreeSqlRepository\\YaSha.DataManager.FileManagement.FreeSqlRepository.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FreeSqlRepository\"/>")
            .Replace("<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\FileManagement\\src\\YaSha.DataManager.FileManagement.HttpApi\\YaSha.DataManager.FileManagement.HttpApi.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FileManagement.HttpApi\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\FileManagement\\src\\YaSha.DataManager.FileManagement.HttpApi.Client\\YaSha.DataManager.FileManagement.HttpApi.Client.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FileManagement.HttpApi.Client\"/>");
    }

    public static string ReplacePackageReferenceLanguageManagement(this string content)
    {
        return content
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\LanguageManagement\\src\\YaSha.DataManager.LanguageManagement.Application\\YaSha.DataManager.LanguageManagement.Application.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.LanguageManagement.Application\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\LanguageManagement\\src\\YaSha.DataManager.LanguageManagement.Application.Contracts\\YaSha.DataManager.LanguageManagement.Application.Contracts.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.LanguageManagement.Application.Contracts\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\LanguageManagement\\src\\YaSha.DataManager.LanguageManagement.Domain\\YaSha.DataManager.LanguageManagement.Domain.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.LanguageManagement.Domain\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\LanguageManagement\\src\\YaSha.DataManager.LanguageManagement.Domain.Shared\\YaSha.DataManager.LanguageManagement.Domain.Shared.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.LanguageManagement.Domain.Shared\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\LanguageManagement\\src\\YaSha.DataManager.LanguageManagement.EntityFrameworkCore\\YaSha.DataManager.LanguageManagement.EntityFrameworkCore.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.LanguageManagement.EntityFrameworkCore\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\LanguageManagement\\src\\YaSha.DataManager.LanguageManagement.FreeSqlRepository\\YaSha.DataManager.LanguageManagement.FreeSqlRepository.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FreeSqlRepository\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\LanguageManagement\\src\\YaSha.DataManager.LanguageManagement.HttpApi\\YaSha.DataManager.LanguageManagement.HttpApi.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.LanguageManagement.HttpApi\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\LanguageManagement\\src\\YaSha.DataManager.LanguageManagement.HttpApi.Client\\YaSha.DataManager.LanguageManagement.HttpApi.Client.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.LanguageManagement.HttpApi.Client\"/>");
    }

    public static string ReplacePackageReferenceNotificationManagement(this string content)
    {
        return content
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\NotificationManagement\\src\\YaSha.DataManager.NotificationManagement.Application\\YaSha.DataManager.NotificationManagement.Application.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.NotificationManagement.Application\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\NotificationManagement\\src\\YaSha.DataManager.NotificationManagement.Application.Contracts\\YaSha.DataManager.NotificationManagement.Application.Contracts.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.NotificationManagement.Application.Contracts\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\NotificationManagement\\src\\YaSha.DataManager.NotificationManagement.Domain\\YaSha.DataManager.NotificationManagement.Domain.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.NotificationManagement.Domain\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\NotificationManagement\\src\\YaSha.DataManager.NotificationManagement.Domain.Shared\\YaSha.DataManager.NotificationManagement.Domain.Shared.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.NotificationManagement.Domain.Shared\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\NotificationManagement\\src\\YaSha.DataManager.NotificationManagement.EntityFrameworkCore\\YaSha.DataManager.NotificationManagement.EntityFrameworkCore.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.NotificationManagement.EntityFrameworkCore\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\NotificationManagement\\src\\YaSha.DataManager.NotificationManagement.FreeSqlRepository\\YaSha.DataManager.NotificationManagement.FreeSqlRepository.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.FreeSqlRepository\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\NotificationManagement\\src\\YaSha.DataManager.NotificationManagement.HttpApi\\YaSha.DataManager.NotificationManagement.HttpApi.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.NotificationManagement.HttpApi\"/>")
            .Replace(
                "<ProjectReference Include=\"..\\..\\..\\..\\..\\aspnet-core\\modules\\NotificationManagement\\src\\YaSha.DataManager.NotificationManagement.HttpApi.Client\\YaSha.DataManager.NotificationManagement.HttpApi.Client.csproj\"/>",
                "<PackageReference Include=\"YaSha.DataManager.NotificationManagement.HttpApi.Client\"/>");
    }

    public static string ReplaceYaShaPackageVersion(this string context, string version)
    {
        return context.Replace("MyVersion", version);
    }
}