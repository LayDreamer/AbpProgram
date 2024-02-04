using YaSha.DataManager.ListProcessing;
using OfficeOpenXml;
using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing.Base
{
    public abstract class ListProcessingBuildBase
    {
        internal int order;

        internal string rulePath;

        internal abstract string TableName { get; set; }

        internal string sheetName;

        protected SheetConfig config;

        protected ExcelWorksheets allsheets;

        public ListProcessingBuildBase(BuildFactory factory)
        {
            this.rulePath = factory.rulePath;
            sheetName = System.IO.Path.GetFileNameWithoutExtension(factory.sourcePath);
            config = factory.outSheetTemplate[TableName];
        }

        internal abstract ExcelWorksheet ProcessExcel(ExcelWorksheets sheets);
    }
}
