using System.Data;

namespace YaSha.DataManager.ListProcessing.AssembleSheet
{
    public static class SortData
    {
        public static DataTable SortData_Assembly(DataTable SourceDataExcel, DataTable SortExcel)
        {
            foreach (DataRow sort_dr in SortExcel.Rows)
            {
                if (sort_dr["优先级"].ToString() == null || sort_dr["优先级"].ToString() == "")
                {
                    break;
                }
                if (sort_dr["类型"].ToString() != "主值")
                {
                    continue;
                }
                string sort_field = sort_dr["字段"].ToString();
                string sort_dir = sort_dr["排序规则"].ToString();
                if (sort_field == "图号")
                {
                    if (sort_dr["备注"].ToString() == "图号没有内容的在最下面")
                    {
                        DataTable haveDrawCode = BuildAssembleSheet.CreateNewDataTable(SourceDataExcel);    //有图号数据
                        DataTable nullDrawCode = BuildAssembleSheet.CreateNewDataTable(SourceDataExcel);   //无图号数据

                        foreach (DataRow dr in SourceDataExcel.Rows)
                        {
                            if (dr["图号"].ToString() != null && dr["图号"].ToString() != "/")
                            {
                                haveDrawCode.Rows.Add(dr.ItemArray);
                            }
                            else
                            {
                                nullDrawCode.Rows.Add(dr.ItemArray);
                            }
                        }

                        if (haveDrawCode.Rows.Count > 0)
                        {
                            if (sort_dir == "升序")
                            {
                                haveDrawCode.DefaultView.Sort = sort_field + " ASC";
                                haveDrawCode = haveDrawCode.DefaultView.ToTable();
                            }
                            else if (sort_dir == "降序")
                            {
                                haveDrawCode.DefaultView.Sort = sort_field + " DESC";
                                haveDrawCode = haveDrawCode.DefaultView.ToTable();
                            }
                        }
                        #region[合并数据]
                        if (nullDrawCode.Rows.Count > 0)
                        {
                            foreach (DataRow dr in nullDrawCode.Rows)
                            {
                                haveDrawCode.Rows.Add(dr.ItemArray);
                            }
                        }
                        SourceDataExcel = haveDrawCode;
                        #endregion
                    }
                    else
                    {
                        if (sort_dir == "升序")
                        {
                            SourceDataExcel.DefaultView.Sort = sort_field + " ASC";
                            SourceDataExcel = SourceDataExcel.DefaultView.ToTable();
                        }
                        else if (sort_dir == "降序")
                        {
                            SourceDataExcel.DefaultView.Sort = sort_field + " DESC";
                            SourceDataExcel = SourceDataExcel.DefaultView.ToTable();
                        }
                    }
                }
                else if (sort_field == "模块编码[流水码]")
                {
                    DataTable haveCodeData = BuildAssembleSheet.CreateNewDataTable(SourceDataExcel);

                    DataTable nullCodeData = BuildAssembleSheet.CreateNewDataTable(SourceDataExcel);

                    foreach (DataRow dr in SourceDataExcel.Rows)
                    {
                        if (dr["非标码"].ToString() != null && dr["非标码"].ToString() != "/")
                        {
                            haveCodeData.Rows.Add(dr.ItemArray);
                        }
                        else
                        {
                            nullCodeData.Rows.Add(dr.ItemArray);
                        }
                    }
                    if (haveCodeData.Rows.Count > 0)
                    {
                        if (sort_dir == "升序")
                        {
                            haveCodeData.DefaultView.Sort = "非标码" + " ASC";
                            haveCodeData = haveCodeData.DefaultView.ToTable();
                        }
                        else if (sort_dir == "降序")
                        {
                            haveCodeData.DefaultView.Sort = "非标码" + " DESC";
                            haveCodeData = haveCodeData.DefaultView.ToTable();
                        }
                    }
                    #region[合并数据]
                    if (nullCodeData.Rows.Count > 0)
                    {
                        foreach (DataRow dr in nullCodeData.Rows)
                        {
                            haveCodeData.Rows.Add(dr.ItemArray);
                        }
                    }
                    SourceDataExcel = haveCodeData;
                    #endregion
                }
                else
                {
                    if (sort_dir == "升序")
                    {
                        //if (sort_field == "图号")
                        //{

                        //    sort_field = "详图图号";
                        //}
                        SourceDataExcel.DefaultView.Sort = sort_field + " ASC";
                        SourceDataExcel = SourceDataExcel.DefaultView.ToTable();
                    }
                    else if (sort_dir == "降序")
                    {
                        SourceDataExcel.DefaultView.Sort = sort_field + " DESC";
                        SourceDataExcel = SourceDataExcel.DefaultView.ToTable();
                    }
                }
            }
            return SourceDataExcel;
        }
    }
}
