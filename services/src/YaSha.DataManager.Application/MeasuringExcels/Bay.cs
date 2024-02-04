using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager.MeasuringExcels
{
    public class Bay
    {
        public Bay()
        {
            children = new List<int>();
            dic = new Dictionary<string, string>();
        }

        public int Index { get; set; }

        public string BayName { get; set; }

        public List<int> children { get; set; }

        public Dictionary<string,string> dic { get; set; }

    }
}
