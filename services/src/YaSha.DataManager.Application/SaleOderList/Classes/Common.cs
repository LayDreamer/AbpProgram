using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace YaSha.DataManager.SaleOderList.Classes
{
    public static class Common
    {
        public static string Gateway = "https://bds.chinayasha.com/bdsfileservice/";
        public static string ServerLocalProductListPath = @"E:\ServerData\FileManagement\ProductList";
        public static string ServerWebProductListPathHttps = $"{Gateway}ProductList/";


        /// <summary>
        /// 默认读取第一个sheet表，
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="rowStart"></param>
        /// <returns></returns>
        public static DataTable CommonWorksheetToDb(string filePath,int rowStart)
        {
            if (!File.Exists(filePath)) return new DataTable();
            FileInfo codeFileInfo = new FileInfo(filePath);
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage(codeFileInfo))
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                //从指定行开始读取数据，行数据为空结束
                DataTable dt = Common.WorksheetToTable(worksheet, worksheet.Dimension.End.Column, rowStart);
                return dt;
            }
        }


        /// <summary>
        /// Worksheet To DataTable 
        /// </summary>
        /// <param name="worksheet"></param>
        /// <param name="rowStart">读取内容数据起始行</param>
        /// <param name="ColEnd"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static DataTable WorksheetToTable(ExcelWorksheet worksheet, int ColEnd, int rowStart, int ColStart = 1)
        {
            int rows = worksheet.Dimension.End.Row;
            //int cols = worksheet.Dimension.End.Column;
            DataTable dt = new DataTable(worksheet.Name);
            DataRow dr = null;
            bool isStop = false;
            //记录表格数据连续为空的个数,当连续十行为空时，就不再读表
            int emptyCount = 0;
            ///行
            for (int i = 1; i <= rows; i++)
            {
                if (emptyCount == 10)
                {
                    //将多加的几行空列移除
                    for (int m = 0; m < emptyCount; m++)
                    {
                        dt.Rows.RemoveAt(dt.Rows.Count - 1);
                    }
                    break;
                }
                if (i >= rowStart && !isStop)
                {
                    dr = dt.Rows.Add();
                }
                ///列
                int colCount = ColStart;
                //判断当前行是否有数据
                string totalString = "";
                for (int j = colCount; j <= ColEnd; j++)
                {
                    if (i == rowStart - 1)
                    {
                        //string value = GetString(worksheet.Cells[i, j].Value);
                        string value = GetMegerValue(worksheet, i, j).Trim();
                        if (!dt.Columns.Contains(value))
                        {
                            dt.Columns.Add(value);
                        }
                    }
                    else
                    {
                        if (i < rowStart || j > ColEnd || j < ColStart)
                        {
                            continue;
                        }
                        //string value = GetString(worksheet.Cells[i, j].Value);
                        string value = GetMegerValue(worksheet, i, j);
                        if (j == 3 && string.IsNullOrEmpty(value))
                        {
                            var excelPicture = worksheet.Drawings.Where(o => o.From.Row == i && o.From.Column == 2).FirstOrDefault() as OfficeOpenXml.Drawing.ExcelPicture;
                            //var excelGroupShape = worksheet.Drawings.Where(o => o.From.Row == i && o.From.Column == j).FirstOrDefault() as OfficeOpenXml.Drawing.ExcelGroupShape;
                            //var filepath = Path.Combine(pathurl, string.Format("{0}.jpg", DateTime.Now.ToFileTime()));//指定文件名
                            //excelPicture.Image.Save(filepath, System.Drawing.Imaging.ImageFormat.Jpeg);//保存图片
                            if (excelPicture != null)
                            {
                                var bytes = excelPicture.Image.ImageBytes;
                                var random = new Random().Next(0, int.MaxValue);
                                string fileName = $"{i}{j}{random}.png";
                                bool isCreate = ByteToFile(bytes, ServerLocalProductListPath, fileName);
                                if (isCreate)
                                {
                                    value = Path.Combine(ServerWebProductListPathHttps, fileName);
                                }
                            }
                        }
                        totalString = string.IsNullOrEmpty(totalString) ? value : totalString;

                        if (j == ColEnd && string.IsNullOrEmpty(totalString))
                        {
                            emptyCount++;
                        }
                        if (j == ColEnd && !string.IsNullOrEmpty(totalString) && emptyCount > 0)
                        {
                            emptyCount = 0;
                        }
                        ///处理列名重复情况
                        var currentColumn = GetMegerValue(worksheet, rowStart - 1, j);
                        if (j > colCount)
                        {
                            var previousColumn = GetMegerValue(worksheet, rowStart - 1, j - 1);
                            if (currentColumn == previousColumn)
                            {
                                continue;
                            }
                            colCount++;
                        }
                        dr[colCount - ColStart] = value;
                    }
                }
            }
            return dt;
        }
        /// <summary>
        /// 读取合并单元格内的数据
        /// </summary>
        /// <param name="wSheet"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public static string GetMegerValue(ExcelWorksheet wSheet, int row, int column)
        {
            string range = wSheet.MergedCells[row, column];
            if (range == null)
                if (wSheet.Cells[row, column].Value != null)
                    return wSheet.Cells[row, column].Value.ToString();
                else
                    return "";
            object value =
                wSheet.Cells[(new ExcelAddress(range)).Start.Row, (new ExcelAddress(range)).Start.Column].Value;
            if (value != null)
                return value.ToString();
            else
                return "";
        }
        public static bool ByteToFile(byte[] byteArray, string dir, string fileName)
        {
            bool result = false;
            try
            {
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                string path = Path.Combine(dir, fileName);
                using (FileStream fs = new(path, FileMode.OpenOrCreate, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    result = true;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// 判断文件名后缀
        /// </summary>
        public static bool JudgeFileNameExtension(string fileName, List<string> permittedExtensions)
        {
            bool isLegal = true;
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
            {
                isLegal = false;
            }
            return isLegal;
        }

        /// <summary>
        /// Datable To List<T>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToModelList<T>(this DataTable dt) where T : new()
        {
            // 定义集合    
            List<T> ts = new List<T>();

            // 获得此模型的类型   
            Type type = typeof(T);
            string tempName = null, tempDescription = null;

            foreach (DataRow dr in dt.Rows)
            {
                bool isAdd = true;
                T t = new T();
                // 获得此模型的公共属性      
                PropertyInfo[] propertys = t.GetType().GetProperties();
                foreach (PropertyInfo pi in propertys)
                {
                    // 检查DataTable是否包含此列    
                    tempName = pi.Name;
                    tempDescription = pi == null ? null : ((DescriptionAttribute)Attribute.GetCustomAttribute(pi, typeof(DescriptionAttribute)))?.Description;
                    string column = tempDescription ?? tempName;
                    if (dt.Columns.Contains(column))
                    {
                        // 判断此属性是否有Setter      
                        if (!pi.CanWrite)
                            continue;
                        object value = dr[column];
                        try
                        {
                            if (value != DBNull.Value)
                            {
                                if (pi.PropertyType.ToString().Contains("System.Nullable"))
                                    value = Convert.ChangeType(value, Nullable.GetUnderlyingType(pi.PropertyType));
                                else
                                    value = Convert.ChangeType(value, pi.PropertyType);
                                pi.SetValue(t, value, null);
                            }
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                }
                if (isAdd)
                {
                    ts.Add(t);
                }
            }
            return ts;
        }



    }
}
