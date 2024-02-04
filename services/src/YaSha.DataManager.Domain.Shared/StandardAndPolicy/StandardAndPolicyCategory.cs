using System.ComponentModel;

namespace YaSha.DataManager.StandardAndPolicy;

public enum StandardAndPolicyCategory
{
    /// <summary>
    /// 国家标准
    /// </summary>
    GB = 1,
    
    /// <summary>
    /// 地方标准
    /// </summary>
    JG = 2,
    
    /// <summary>
    /// 行业标准
    /// </summary>
    DB = 4,
    
    /// <summary>
    /// 团体标准
    /// </summary>
    TB = 8,
    
    
    /// <summary>
    /// 企业标准
    /// </summary>
    QB = 16,
}