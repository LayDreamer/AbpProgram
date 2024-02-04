using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YaSha.DataManager.ListProcessing.AssembleSheet
{
    public static class Assembly
    {
        public static int FilterWork(DataRow dr, System.Data.DataTable FilterExcel)
        {
            int isAccordWith = 1;

            string field = null;
            int num_field = 0;   //字段（条件）数量
            int num_true = 0;   //（条件）正确的数量
            //int num = 0;
            bool single = false;
            foreach (DataRow dr_filter in FilterExcel.Rows)
            {
                if (dr_filter["序号"].ToString() == null || dr_filter["序号"].ToString() == "")
                {
                    break;
                }
                field = dr_filter["字段"].ToString();
                string value = dr[field].ToString();
                if (num_field == 0)
                {
                    if (dr_filter["类型1"].ToString() == "主值")
                    {
                        //field = dr_filter["算法"].ToString();
                        num_field++;
                    }
                    else
                    {
                        throw new Exception("《装配单-过滤设置》第一条数据“类型1”列内容应为“主值”");
                        isAccordWith = 0;
                        return isAccordWith;
                    }
                }
                else
                {
                    if (dr_filter["类型1"].ToString() == "主值")
                    {
                        field = dr_filter["算法"].ToString();
                        num_field++;
                        if (single == true)
                        {
                            num_true++;
                            single = false;
                        }
                    }
                }

                if (dr_filter["类型1"].ToString().Contains("主值") || dr_filter["类型1"].ToString().Contains("或"))
                {
                    if (dr_filter["算法"].ToString() == "包含")
                    {
                        if (dr[field].ToString().Contains(dr_filter["值"].ToString()))
                        {
                            single = true;
                        }
                    }
                    else if (dr_filter["算法"].ToString() == "不包含")
                    {
                        if (!dr[field].ToString().Contains(dr_filter["值"].ToString()))
                        {
                            single = true;
                        }
                    }
                    else if (dr_filter["算法"].ToString() == "等于")
                    {
                        if (dr[field].ToString() == dr_filter["值"].ToString())
                        {
                            single = true;
                        }
                    }
                    else if (dr_filter["算法"].ToString() == "不等于")
                    {
                        if (dr[field].ToString() != dr_filter["值"].ToString())
                        {
                            single = true;
                        }
                    }
                }
                #region["类型1"包含“或”]
                //else if (dr_filter["类型1"].ToString().Contains("或"))
                //{
                //    if (dr_filter["算法"].ToString() == "包含")
                //    {
                //        if (dr[field].ToString().Contains(dr_filter["值"].ToString()))
                //        {
                //            single = true;
                //        }
                //    }
                //    else if (dr_filter["算法"].ToString() == "不包含")
                //    {
                //        if (!dr[field].ToString().Contains(dr_filter["值"].ToString()))
                //        {
                //            single = true;
                //        }
                //    }
                //    else if (dr_filter["算法"].ToString() == "等于")
                //    {
                //        if (dr[field].ToString() == dr_filter["值"].ToString())
                //        {
                //            single = true;
                //        }
                //    }
                //    else if (dr_filter["算法"].ToString() == "不等于")
                //    {
                //        if (dr[field].ToString() != dr_filter["值"].ToString())
                //        {
                //            single = true;
                //        }
                //    }
                //}
                #endregion
                else if (dr_filter["类型1"].ToString().Contains("且"))
                {
                    if (num_field == 1 || isAccordWith == 2)
                    {
                        if (dr_filter["算法"].ToString() == "包含")
                        {
                            if (!dr[field].ToString().Contains(dr_filter["值"].ToString()))
                            {
                                single = false;
                            }
                        }
                        else if (dr_filter["算法"].ToString() == "不包含")
                        {
                            if (dr[field].ToString().Contains(dr_filter["值"].ToString()))
                            {
                                single = false;
                            }
                        }
                        else if (dr_filter["算法"].ToString() == "等于")
                        {
                            if (dr[field].ToString() != dr_filter["值"].ToString())
                            {
                                single = false;
                            }
                        }
                        else if (dr_filter["算法"].ToString() == "不等于")
                        {
                            if (dr[field].ToString() == dr_filter["值"].ToString())
                            {
                                single = false;
                            }
                        }
                    }
                }

            }

            #region[判断最后一组条件（每个字段一组）结果]
            if (single == true)
            {
                num_true++;
            }
            #endregion

            #region[相同字段为一组条件，字段数量与每组结果正确数量一致则符合条件]
            if (num_field == num_true)
            {
                isAccordWith = 2;
            }
            #endregion
            return isAccordWith;
        }

        private static DataTable source_copy = null;
        public static DataTable EditWork(DataTable SourceDataExcel, DataTable EditExcel, int num, DataTable SourceDataExcel_copy)
        {
            source_copy = SourceDataExcel_copy;
            for (int i = 0; i < SourceDataExcel.Rows.Count; i++)
            {
                DataRow dr = SourceDataExcel.Rows[i];

                foreach (DataRow dr_edit in EditExcel.Rows)
                {
                    if (dr_edit["序号"].ToString() == null || dr_edit["序号"].ToString() == "")
                    {
                        break;
                    }

                    if (dr_edit["类型1"].ToString() == "主值")
                    {
                        string field = dr_edit["字段"].ToString();   //当前字段
                        bool isEligible = JudgeValue(SourceDataExcel, dr, field, dr_edit, null);
                        //bool isEligible = JudgeIsEligible(dr[field].ToString(), dr_edit);
                        if (isEligible == true)
                        {
                            if (dr_edit["多条件"].ToString() != null && dr_edit["多条件"].ToString() != "")
                            {
                                if (dr_edit["多条件"].ToString().Contains("、"))
                                {
                                    List<int> allCondition = GetAllCondition(dr_edit["多条件"].ToString());

                                    bool sub_isEligible = true;   //判断每个“多条件”是否符合
                                    foreach (int no in allCondition)
                                    {
                                        DataRow subDr = ReturnDataRow(EditExcel, no);

                                        string sub_field = subDr["字段"].ToString();
                                        sub_isEligible = JudgeValue(SourceDataExcel, dr, sub_field, subDr, dr[field].ToString());

                                        if (sub_isEligible == true)
                                        {
                                            if (subDr["赋值字段"].ToString() != null && subDr["赋值字段"].ToString() != "")
                                            {
                                                if (!subDr["赋值类型"].ToString().Contains("累加"))
                                                {
                                                    dr = EditDataRow(dr, subDr["赋值字段"].ToString(), subDr["赋值类型"].ToString(), subDr["赋值"].ToString(), num);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (subDr["多条件"].ToString() != null && subDr["多条件"].ToString() != "")
                                            {
                                                DataRow subDr_else = ReturnDataRow(EditExcel, Convert.ToInt32(subDr["多条件"].ToString()));
                                                if (subDr_else["类型1"].ToString() == "否则")
                                                {
                                                    if (!EditExcel.Rows[no - 1]["赋值类型"].ToString().Contains("累加"))
                                                    {
                                                        dr = EditDataRow(dr, subDr_else["赋值字段"].ToString(), subDr_else["赋值类型"].ToString(), subDr_else["赋值"].ToString(), num);
                                                    }
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    int no = Convert.ToInt16(dr_edit["多条件"].ToString());
                                    DataRow subDr = ReturnDataRow(EditExcel, no);
                                    //DataRow subDr = EditExcel.Rows[no];
                                    if (subDr["类型1"].ToString() == "且")
                                    {
                                        string sub_field = subDr["字段"].ToString();

                                        bool sub_isEligible = JudgeValue(SourceDataExcel, dr, sub_field, subDr, dr[field].ToString());
                                        //bool sub_isEligible = JudgeIsEligible(dr[sub_field].ToString(), subDr);
                                        if (sub_isEligible == true)
                                        {
                                            if (!dr_edit["赋值类型"].ToString().Contains("累加"))
                                            {
                                                dr = EditDataRow(dr, subDr["赋值字段"].ToString(), subDr["赋值类型"].ToString(), subDr["赋值"].ToString(), num);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!dr_edit["赋值类型"].ToString().Contains("累加"))
                                {
                                    dr = EditDataRow(dr, dr_edit["赋值字段"].ToString(), dr_edit["赋值类型"].ToString(), dr_edit["赋值"].ToString(), num);
                                }
                            }
                        }
                        else
                        {
                            if (dr_edit["多条件"].ToString() != null && dr_edit["多条件"].ToString() != "")
                            {
                                List<int> allCondition = GetAllCondition(dr_edit["多条件"].ToString());
                                foreach (int no in allCondition)
                                {

                                    DataRow subDr = EditExcel.Rows[no - 1];
                                    if (subDr["类型1"].ToString() == "否则")
                                    {
                                        if (EditExcel.Rows[no - 1]["赋值类型"].ToString().Contains("累加"))
                                        {
                                            break;
                                        }
                                        dr = EditDataRow(dr, EditExcel.Rows[no - 1]["赋值字段"].ToString(), EditExcel.Rows[no - 1]["赋值类型"].ToString(), EditExcel.Rows[no - 1]["赋值"].ToString(), num);
                                        break;
                                    }
                                }
                            }
                        }
                        #region[注释内容]
                        //if (dr_edit["算法"].ToString() == "包含")
                        //{
                        //    string value = dr_edit["值"].ToString();
                        //    //bool isEqual = false;
                        //    if(dr[field].ToString().Contains(value))
                        //    {
                        //        //isEqual = true;

                        //        if(dr_edit["多条件"].ToString() != null && dr_edit["多条件"].ToString() != "")
                        //        {
                        //            if(dr_edit["多条件"].ToString().Contains("、"))
                        //            {
                        //                List<int> allCondition = GetAllCondition(dr_edit["多条件"].ToString());

                        //                foreach (int no in allCondition)
                        //                {
                        //                    DataRow subDr = EditExcel.Rows[no - 1];

                        //                    if (subDr["类型1"].ToString() == "且")
                        //                    {
                        //                        if (subDr["字段"].ToString().Contains("["))
                        //                        {

                        //                        }
                        //                        else
                        //                        {
                        //                            string sub_field = subDr["字段"].ToString();


                        //                        }
                        //                    }
                        //                }
                        //            }
                        //            else
                        //            {
                        //                int no = Convert.ToInt16(dr_edit["多条件"].ToString());

                        //                DataRow subDr = EditExcel.Rows[no - 1];
                        //                if(subDr["类型1"].ToString() == "且")
                        //                {
                        //                    string sub_field = subDr["字段"].ToString();
                        //                    bool isEligible = JudgeIsEligible(dr[sub_field].ToString() , subDr);
                        //                    if(isEligible == true)
                        //                    {
                        //                        dr = EditDataRow(dr, dr_edit["赋值字段"].ToString(), dr_edit["赋值类型"].ToString(), dr_edit["赋值"].ToString(), num);
                        //                    }
                        //                }
                        //            }
                        //        }
                        //        else
                        //        {
                        //            dr = EditDataRow(dr ,dr_edit["赋值字段"].ToString() , dr_edit["赋值类型"].ToString() , dr_edit["赋值"].ToString() , num);
                        //        }
                        //    }
                        //}
                        //else if (dr_edit["算法"].ToString() == "不包含")
                        //{

                        //}
                        //else if (dr_edit["算法"].ToString() == "等于")
                        //{

                        //}
                        //else if (dr_edit["算法"].ToString() == "不等于")
                        //{

                        //}
                        //else if (dr_edit["算法"].ToString() == "小于等于")
                        //{

                        //}
                        //else if (dr_edit["算法"].ToString() == "大于等于")
                        //{

                        //}
                        #endregion
                    }
                }

            }

            return SourceDataExcel;
        }

        #region[判断条件是否符合]
        public static bool JudgeIsEligible(string source, DataRow dr_edit)
        {
            bool isEligible = false;
            string category = dr_edit["类型2"].ToString();
            string value = dr_edit["值"].ToString();
            if (value == "空")
            {
                value = null;
            }
            if (dr_edit["算法"].ToString() == "包含")
            {
                //if(category == "值")
                //{
                if (source.Contains(value))
                {
                    isEligible = true;
                }
                //}
                //else if (category == "数量")
                //{

                //}
            }
            else if (dr_edit["算法"].ToString() == "不包含")
            {
                if (!source.Contains(value))
                {
                    isEligible = true;
                }
            }
            else if (dr_edit["算法"].ToString() == "等于")
            {
                if (source.Contains("+"))
                {
                    isEligible = ValueCompare(source, value, dr_edit["算法"].ToString());
                }
                else
                {
                    if (category == "值")
                    {
                        if (source == value)
                        {
                            isEligible = true;
                        }
                    }
                    else if (category == "数量")
                    {
                        if (Convert.ToDouble(source) == Convert.ToDouble(value))
                        {
                            isEligible = true;
                        }
                    }
                }
            }
            else if (dr_edit["算法"].ToString() == "不等于")
            {
                if (category == "值")
                {
                    if (source != value)
                    {
                        isEligible = true;
                    }
                }
                else if (category == "数量")
                {
                    if (Convert.ToDouble(source) != Convert.ToDouble(value))
                    {
                        isEligible = true;
                    }
                }
            }
            else if (dr_edit["算法"].ToString() == "小于等于")
            {
                if (source.Contains("+"))
                {
                    isEligible = ValueCompare(source, value, dr_edit["算法"].ToString());
                }
                else
                {
                    if (Convert.ToDouble(source) <= Convert.ToDouble(value))
                    {
                        isEligible = true;
                    }
                }
            }
            else if (dr_edit["算法"].ToString() == "大于等于")
            {
                if (source.Contains("+"))
                {
                    isEligible = ValueCompare(source, value, dr_edit["算法"].ToString());
                }
                else
                {
                    if (Convert.ToDouble(source) >= Convert.ToDouble(value))
                    {
                        isEligible = true;
                    }

                }
            }

            return isEligible;
        }

        public static bool JudgeIsEligible_special(string source, DataRow dr_edit)
        {
            bool isEligible = false;
            string[] sArray = source.Split('+');
            string category = dr_edit["类型2"].ToString();
            string value = dr_edit["值"].ToString();
            if (dr_edit["算法"].ToString() == "小于等于")
            {
                foreach (string str in sArray)
                {
                    if (str != null && str != "")
                    {
                        if (Convert.ToDouble(str) <= Convert.ToDouble(value))
                        {
                            isEligible = true;
                            break;
                        }
                    }
                }
            }
            else if (dr_edit["算法"].ToString() == "大于等于")
            {
                foreach (string str in sArray)
                {
                    if (str != null && str != "")
                    {
                        if (Convert.ToDouble(str) >= Convert.ToDouble(value))
                        {
                            isEligible = true;
                            break;
                        }
                    }
                }
            }

            return isEligible;
        }


        private static bool JudgeValue(DataTable SourceDataExcel, DataRow dr, string field_judge, DataRow dr_edit, string fieldValue_before)
        {
            bool isElig = false;
            if (field_judge.Contains("[") && field_judge.Contains("]"))
            {
                //string[] sArry1 = field_judge.Split('[');  //[产品名称/模块名称]物料名称
                //string[] sArry2 = sArry1[1].Split(']');

                field_judge = field_judge.Replace("[", "");
                field_judge = field_judge.Replace("]", "");
                int No = GetDataRowNo(dr);
                if (No == 0)
                {
                    return isElig;
                }
                field_judge = field_judge.Replace("产品名称", "");
                field_judge = field_judge.Replace("模块名称", "");
                field_judge = field_judge.Replace(" ", "");
                foreach (DataRow sub_dr in source_copy.Rows)
                {
                    int sub_No = GetDataRowNo(sub_dr);
                    if (sub_No == No)
                    {
                        isElig = JudgeIsEligible(sub_dr[field_judge].ToString(), dr_edit);
                    }
                    if (isElig == true)
                    {
                        break;
                    }
                }
            }
            else if (field_judge.Contains("-"))
            {
                string[] sArray = field_judge.Split('-');
                string field_judge_before = sArray[0];
                string field_judge_after = sArray[1].Replace("“", "");
                field_judge_after = field_judge_after.Replace("”", "");
                string judgeValue = dr[field_judge_before].ToString().Replace(field_judge_after, "@");
                int count = System.Text.RegularExpressions.Regex.Matches(judgeValue, "@").Count;
                isElig = JudgeIsEligible(count.ToString(), dr_edit);
            }
            else if (field_judge == "+")
            {
                fieldValue_before = fieldValue_before.Replace("+", "@");
                int len = System.Text.RegularExpressions.Regex.Matches(fieldValue_before, "@").Count;
                isElig = JudgeIsEligible(len.ToString(), dr_edit);
            }
            else if (field_judge == "折弯尺寸" && (dr_edit["算法"].ToString() == "大于等于" || dr_edit["算法"].ToString() == "小于等于"))
            {
                isElig = JudgeIsEligible_special(dr[field_judge].ToString(), dr_edit);
            }
            else
            {
                isElig = JudgeIsEligible(dr[field_judge].ToString(), dr_edit);
            }
            return isElig;
        }

        private static int GetDataRowNo(DataRow dr)
        {
            int i = 0;
            try
            {
                string No = dr["序号"].ToString();
                string[] sArray = No.Split('.');
                i = Convert.ToInt32(sArray[0]);
            }
            catch
            { }

            return i;
        }

        private static bool ValueCompare(string source, string number, string way)
        {
            bool result = true;
            double value = 0;
            try
            {
                value = Convert.ToDouble(number);
            }
            catch
            { }
            string[] sArray = source.Split('+');
            foreach (string str in sArray)
            {
                double num = 0;
                try
                {
                    num = Convert.ToDouble(str);
                }
                catch
                { }
                if (way == "等于")
                {
                    if (num != value)
                    {
                        result = false;
                        break;
                    }
                }
                else if (way == "小于等于")
                {
                    if (num > value)
                    {
                        result = false;
                        break;
                    }
                }
                else if (way == "大于等于")
                {
                    if (num < value)
                    {
                        result = false;
                        break;
                    }
                }
            }
            return result;
        }
        #endregion

        #region[修改DataRow数据]
        public static DataRow EditDataRow(DataRow dr, string valueField, string valueCategory, string value, int num)
        {
            if (valueCategory == "值")
            {
                dr[valueField] = value;
            }
            else if (valueCategory == "值附加")
            {
                string str = dr[valueField].ToString();
                if (value.Contains("[") && value.Contains("]"))
                {
                    string[] sArray = value.Split('[');
                    string field = sArray[1].Split(']')[0];
                    string field_value = dr[field].ToString();
                    value = value.Replace("[" + field + "]", field_value);
                    dr[valueField] = str + value;
                }
                else
                {
                    dr[valueField] = str + value;
                }
            }
            else if (valueCategory == "值前缀附加")
            {
                string str = dr[valueField].ToString();
                if (value.Contains("[") && value.Contains("]"))
                {
                    string[] sArray = value.Split('[');
                    string field = sArray[1].Split(']')[0];
                    string field_value = dr[field].ToString();
                    value = value.Replace("[" + field + "]", field_value);
                    dr[valueField] = value + str;
                }
                else
                {
                    dr[valueField] = value + str;
                }
            }
            else if (valueCategory == "字段")
            {
                dr[valueField] = dr[value].ToString();
            }
            else if (valueCategory == "公式")
            {
                double result = 0;
                string[] sArray = value.Split(' ');
                int worknum = 0;
                for (int i = 0; i < sArray.Count(); i++)
                {
                    if (sArray[i] == null || sArray[i] == "")
                    {
                        break;
                    }
                    if (sArray[i] == "+" || sArray[i] == "-" || sArray[i] == "*" || sArray[i] == "/")
                    {
                        if (worknum == 0)
                        {
                            if (sArray[i] == "+")
                            {
                                result = StringToDouble(dr, sArray[i - 1], num) + StringToDouble(dr, sArray[i + 1], num);
                            }
                            else if (sArray[i] == "-")
                            {
                                result = StringToDouble(dr, sArray[i - 1], num) - StringToDouble(dr, sArray[i + 1], num);
                            }
                            else if (sArray[i] == "*")
                            {
                                result = StringToDouble(dr, sArray[i - 1], num) * StringToDouble(dr, sArray[i + 1], num);
                            }
                            else if (sArray[i] == "/")
                            {
                                result = StringToDouble(dr, sArray[i - 1], num) / StringToDouble(dr, sArray[i + 1], num);
                            }
                        }
                        else
                        {
                            if (sArray[i] == "+")
                            {
                                result = result + StringToDouble(dr, sArray[i + 1], num);
                            }
                            else if (sArray[i] == "-")
                            {
                                result = result - StringToDouble(dr, sArray[i + 1], num);
                            }
                            else if (sArray[i] == "*")
                            {
                                result = result * StringToDouble(dr, sArray[i + 1], num);
                            }
                            else if (sArray[i] == "/")
                            {
                                result = result / StringToDouble(dr, sArray[i + 1], num);
                            }
                        }
                        worknum++;
                    }
                }
                //if(valueField != "批量")
                //{
                dr[valueField] = result.ToString();
                //}
            }
            else if (valueCategory == "向上取整")
            {
                dr[valueField] = Math.Ceiling(Convert.ToDouble(dr[valueField])).ToString();
            }
            else if (valueCategory == "向下取整")
            {
                dr[valueField] = Math.Floor(Convert.ToDouble(dr[valueField])).ToString();
            }

            return dr;
        }
        #endregion

        #region[修改内容为计算公式情况，公式中字符串转为数字 ]
        public static double StringToDouble(DataRow dr, string str, int num)
        {
            double value = 0;
            try
            {
                if (str.Contains("[") && str.Contains("]"))
                {
                    string field = str.Replace("[", "");
                    field = field.Replace("]", "");
                    value = Convert.ToDouble(dr[field].ToString());
                }
                else
                {
                    if (str.Contains("批量数"))
                    {
                        value = num;
                    }
                    else
                    {
                        value = Convert.ToDouble(str);
                    }
                }
            }
            catch
            { }
            return value;
        }
        #endregion

        #region[获取多条件所有序号]
        public static List<int> GetAllCondition(string value)
        {
            List<int> all = new List<int>();
            if (value.Contains("、"))
            {
                string[] sArray = value.Split('、');
                foreach (string str in sArray)
                {
                    try
                    {
                        int num = Convert.ToInt16(str);
                        all.Add(num);
                    }
                    catch
                    { }
                }
            }
            else
            {
                try
                {
                    int num = Convert.ToInt16(value);
                    all.Add(num);
                }
                catch
                { }
            }
            return all;
        }
        #endregion

        #region
        private static DataRow ReturnDataRow(DataTable dt, int no)
        {
            int k = 0;
            for (int j = 0; j < dt.Rows.Count; j++)
            {
                int num = Convert.ToInt32(dt.Rows[j]["序号"].ToString());
                if (num == no)
                {
                    k = j;
                    break;
                }
            }
            return dt.Rows[k];
        }
        #endregion

    }
}
