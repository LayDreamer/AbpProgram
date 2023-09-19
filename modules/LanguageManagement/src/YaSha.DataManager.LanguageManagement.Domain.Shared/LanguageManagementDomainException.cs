using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace YaSha.DataManager.LanguageManagement;

public class LanguageManagementDomainException : BusinessException
{
    public LanguageManagementDomainException(
        string code = null,
        string message = null,
        string details = null,
        Exception innerException = null,
        LogLevel logLevel = LogLevel.Warning)
        : base(code, message, details, innerException, logLevel)
    {
    }
}