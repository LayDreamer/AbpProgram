using OfficeOpenXml;
using YaSha.DataManager.ListProcessing.Base;
using YaSha.DataManager.ListProcessing.CommonModel;

namespace YaSha.DataManager.ListProcessing
{
    public class BuildFactory
    {
        public string sourcePath;

        public string templetePath;

        public string rulePath;

        public string nestrulePath;

        public string savePath;

        public string sheetType;

        public string dt;

        internal OutSheetTemplate outSheetTemplate = null;

        List<ListProcessingBuildBase> allBuildSheets;

        public BuildFactory(
            string sourcePath,
            string templetePath,
            string rulePath,
            string nestrulePath,
            string savePath,
            string sheetType,
            string dt,
            List<string> sheetNames
            )
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            this.sourcePath = sourcePath;
            this.templetePath = templetePath;
            this.rulePath = rulePath;
            this.nestrulePath = nestrulePath;
            this.savePath = savePath;
            string resDir = Path.GetDirectoryName(savePath);
            if (!Directory.Exists(resDir))
            {
                Directory.CreateDirectory(resDir);
            }
            this.sheetType = sheetType;
            this.dt = dt;
            this.outSheetTemplate = new OutSheetTemplate(this.templetePath);
            allBuildSheets = new List<ListProcessingBuildBase>();


            var allsheetModels = typeof(ListProcessingBuildBase).Assembly.GetTypes().
                Where(t => typeof(ListProcessingBuildBase).IsAssignableFrom(t)).
                Where(t => !t.IsAbstract && t.IsClass).
                Select(t => (ListProcessingBuildBase)Activator.CreateInstance(t, this)).Where(x => sheetNames.Contains(x.TableName)).OrderBy(x => x.order);
            allBuildSheets.AddRange(allsheetModels);

        }

        public bool Build(bool replace = false)
        {
            bool iRet = true;
            string msg = string.Empty;

            using (ExcelPackage package = new(sourcePath))
            {
                foreach (var item in allBuildSheets)
                {
                    var sheet = package.Workbook.Worksheets[item.TableName];
                    if (sheet == null)
                    {
                        try
                        {
                            var buildSheet = item.ProcessExcel(package.Workbook.Worksheets);
                            package.Workbook.Worksheets.Add(item.TableName, buildSheet);
                        }
                        catch (Exception ex)
                        {
                            msg += (ex.Message + "\n");
                            iRet = false;
                        }
                    }
                    else
                    {
                        if (replace)
                        {
                            try
                            {
                                string preName = string.Empty;

                                string currentName = string.Empty;

                                string nextName = string.Empty;

                                for (int i = 0; i < package.Workbook.Worksheets.Count; i++)
                                {
                                    if (package.Workbook.Worksheets[i].Name == item.TableName)
                                    {
                                        currentName = item.TableName;

                                        if (i > 0)
                                        {
                                            preName = package.Workbook.Worksheets[i - 1].Name;
                                        }

                                        if (i < package.Workbook.Worksheets.Count - 1)
                                        {
                                            nextName = package.Workbook.Worksheets[i + 1].Name;
                                        }

                                        break;
                                    }
                                }

                                package.Workbook.Worksheets.Delete(sheet);

                                var buildSheet = item.ProcessExcel(package.Workbook.Worksheets);

                                package.Workbook.Worksheets.Add(item.TableName, buildSheet);

                                if (preName != string.Empty)
                                {
                                    package.Workbook.Worksheets.MoveAfter(currentName, preName);
                                }
                                else
                                {
                                    if (nextName != string.Empty)
                                    {
                                        package.Workbook.Worksheets.MoveBefore(currentName, nextName);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                msg += (ex.Message + "\n");
                                iRet = false;
                            }
                        }
                        else
                        {
                            var timeName = $"{item.TableName}-{DateTime.Now:yyyy MM dd HH mm ss}";
                            try
                            {
                                var buildSheet = item.ProcessExcel(package.Workbook.Worksheets);
                                package.Workbook.Worksheets.Add(timeName, buildSheet);
                            }
                            catch (Exception ex)
                            {
                                msg += (ex.Message + "\n");
                                iRet = false;
                            }
                        }
                    }
                }
                try
                {
                    package.SaveAs(savePath);
                }
                catch
                {
                    iRet = false;
                }
            }
            if (!string.IsNullOrEmpty(msg))
            {
                throw new Exception(msg);
            }
            return iRet;
        }
    }
}
