using OfficeOpenXml;

namespace YaSha.DataManager.ListProcessing.CommonMethods
{
    internal static  class ExcelHelper
    {
        internal static int GetColumn(ExcelWorksheet sheet, int row, string name)
        {
            for (int i = 1; i <= sheet.Dimension.Columns; i++)
            {
                var value = sheet.Cells[row, i].Text;

                if (value.Equals(name))
                {
                    return i;
                }
            }

            return -1;
        }

    }
}
