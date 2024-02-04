using System.Data;

namespace YaSha.DataManager.ListProcessing.AssembleSheet
{
    public static class MergeData
    {
        public static DataTable MergeExcelData(DataTable sources, DataTable EditExcel)
        {
            //DataTable resultTable = ReadExceFilel.CreateNewDataTable(sources);
            DataTable source = sources;
            DataTable addDT = BuildAssembleSheet.CreateNewDataTable(EditExcel);

            foreach (DataRow Edt in EditExcel.Rows)
            {
                if (Edt["赋值类型"].ToString().Contains("累加"))
                {
                    addDT.Rows.Add(Edt.ItemArray);
                }
            }
            if (addDT.Rows.Count > 0)
            {
                foreach (DataRow Edr in addDT.Rows)
                {
                    string editField = Edr["赋值字段"].ToString();  //累加字段名
                    string category = Edr["赋值类型"].ToString();  //累加字段名
                    //string field = Edr["字段"].ToString();  //字段名
                    //string compareMethod = Edr["算法"].ToString();
                    //string value = Edr["值"].ToString();   //比较的内容
                    //List<string> strs = new List<string>();  //字段包含内容（条件）

                    //List<MergeList> mergeGroup = new List<MergeList>();   //根据某字段（比如：模块名称、物料名称）相等分组
                    DataTable allDatas = BuildAssembleSheet.CreateNewDataTable(sources);


                    foreach (DataRow Sdr in source.Rows)
                    {
                        if (Sdr["序号"].ToString() == null || Sdr["序号"].ToString() == "")
                        {
                            break;
                        }
                        string field = Sdr[Edr["字段"].ToString()].ToString();   //主值字段内容
                        if (JudgeCompare(field, Edr["算法"].ToString(), Edr["值"].ToString()) == true)
                        {
                            allDatas.Rows.Add(Sdr.ItemArray);
                        }
                    }
                    #region[源数据删除符合条件的需要累加的数据]
                    //source = DelSource(source , allDatas);
                    source = DelSource(source, allDatas);
                    #endregion

                    #region[处理符合条件的数据，判断分组累加还是直接累加]
                    if (category.Contains("-"))
                    {
                        string fields = EditString(category);
                        if (fields.Contains("、"))
                        {
                            //待填充
                        }
                        else
                        {
                            string fieldValue = fields.Replace("[", "");
                            fieldValue = fieldValue.Replace("]", "");
                            IEnumerable<IGrouping<string, DataRow>> result = allDatas.Rows.Cast<DataRow>().GroupBy<DataRow, string>(dr => dr[fieldValue].ToString());
                            foreach (IGrouping<string, DataRow> res in result)
                            {
                                DataTable dt = BuildAssembleSheet.CreateNewDataTable(source);

                                foreach (DataRow item in res)
                                {
                                    dt.Rows.Add(item.ItemArray);
                                }
                                double num = 0;
                                for (int i = 0; i < dt.Rows.Count; i++)
                                {
                                    if (i > 0)
                                    {
                                        num = num + Convert.ToDouble(dt.Rows[i][editField].ToString());
                                    }
                                }
                                dt.Rows[0][editField] = (Convert.ToDouble(dt.Rows[0][editField].ToString()) + num).ToString();
                                dt.Rows[0]["图号"] = "/";
                                source.Rows.Add(dt.Rows[0].ItemArray);
                                //resultTable.Rows.Add(dt.Rows[0].ItemArray);
                            }
                        }
                    }
                    else
                    {
                        double num = 0;
                        for (int i = 0; i < allDatas.Rows.Count; i++)
                        {
                            if (i > 0)
                            {
                                num = num + Convert.ToDouble(allDatas.Rows[i][editField].ToString());
                            }
                        }
                        allDatas.Rows[0][editField] = (Convert.ToDouble(allDatas.Rows[0][editField].ToString()) + num).ToString();
                        source.Rows.Add(allDatas.Rows[0].ItemArray);
                    }
                    #endregion
                }
            }

            return source;
        }

        #region[“算法”判断比较]
        public static bool JudgeCompare(string field, string compare, string value)
        {
            bool isMeet = false;
            if (compare == "等于")
            {
                if (field == value)
                {
                    isMeet = true;
                }
            }
            else if (compare == "不等于")
            {
                if (field != value)
                {
                    isMeet = true;
                }
            }
            else if (compare == "包含")
            {
                if (value.Contains("、"))
                {
                    List<string> strs = new List<string>();
                    string[] sArray = value.Split('、');
                    foreach (string str in sArray)
                    {
                        if (str != null && str != "")
                        {
                            strs.Add(str);
                        }
                    }
                    isMeet = IsEqual(strs, field, true);
                }
                else
                {
                    if (field.Contains(value))
                    {
                        isMeet = true;
                    }
                }
            }
            else if (compare == "不包含")
            {
                if (value.Contains("、"))
                {
                    List<string> strs = new List<string>();
                    string[] sArray = value.Split('、');
                    foreach (string str in sArray)
                    {
                        if (str != null && str != "")
                        {
                            strs.Add(str);
                        }
                    }
                    isMeet = IsEqual(strs, field, false);
                }
                else
                {
                    if (!field.Contains(value))
                    {
                        isMeet = true;
                    }
                }
            }

            return isMeet;
        }

        #endregion

        #region[判断全部都包含/不包含]
        public static bool IsEqual(List<string> strs, string value, bool isEqual)
        {
            bool e = true;
            foreach (string str in strs)
            {
                if (isEqual == true)
                {
                    if (!value.Contains(str))
                    {
                        e = false;
                        break;
                    }
                }
                else
                {
                    if (value.Contains(str))
                    {
                        e = false;
                        break;
                    }
                }
            }
            return e;
        }
        #endregion

        #region[删除源数据中需要被合并的数据]
        private static DataTable DelSource(DataTable source, DataTable DelData)
        {
            DataTable result = BuildAssembleSheet.CreateNewDataTable(source);
            //foreach (DataRow delDr in DelData.Rows)
            //{
            //    bool isEqual = true;
            //    foreach(DataRow sDr in result.Rows)
            //    {
            //        for(int i = 0; i < result.Columns.Count; i++)
            //        {
            //            if(delDr[i] != sDr[i])
            //            {
            //                isEqual = false;
            //                break;
            //            }
            //        }
            //        if(isEqual == true)
            //        {
            //            result.Rows.Remove(sDr);
            //            break;
            //        }
            //    }
            //}

            foreach (DataRow sDr in source.Rows)
            {
                if (sDr["序号"].ToString() == null || sDr["序号"].ToString() == "")
                {
                    break;
                }
                string sNum = sDr["序号"].ToString();
                bool isEqual = false;
                foreach (DataRow delDr in DelData.Rows)
                {
                    if (delDr["序号"].ToString() == sNum)
                    {
                        isEqual = true;
                        break;
                    }
                }
                if (isEqual == false)
                {
                    result.Rows.Add(sDr.ItemArray);
                }
            }

            return result;
        }

        #endregion

        private static string EditString(string str)
        {
            string result = null;
            string del = str;
            //del.Replace("-", "@");  
            //int len = System.Text.RegularExpressions.Regex.Matches(fieldValue_before, "@").Count;
            //if(System.Text.RegularExpressions.Regex.Matches(del, "@").Count > 1)
            //{
            string[] sArray = del.Split('-');
            del = "-" + sArray[sArray.Count() - 1];
            result = str.Replace(del, "");
            //}
            return result;
        }


    }
}
