using Microsoft.AspNetCore.Http;

namespace YaSha.DataManager.ListProcessing;

public class ListProcessingBuildDto
{
    public string File { get;  }
    
    public string Config { get;  }
    
    public string Rule { get;  }
    
    public string Nest { get;  }
    
    public string Type { get; }
    
    public string Dt { get;  }
    
    public List<string> Sheets { get; }

    public bool Replace { get; }
    
    public ListProcessingBuildDto(string file, string config, string rule, string nest, string type, string dt, bool replace, List<string> sheets)
    {
        File = file;
        Config = config;
        Rule = rule;
        Nest = nest;
        Type = type;
        Dt = dt;
        Replace = replace;
        Sheets = sheets;
    }
}