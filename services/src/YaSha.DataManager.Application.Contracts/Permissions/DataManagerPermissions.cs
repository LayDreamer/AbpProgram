namespace YaSha.DataManager.Permissions
{
    public static class DataManagerPermissions
    {
        /// <summary>
        /// 系统管理扩展权限
        /// </summary>
        public static class SystemManagement
        {
            public const string Default = "AbpIdentity";
        }




        public const string GroupName = "YaSha.DataManager";

        public static class ProductList
        {
            public const string Default = GroupName + ".ProductList";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";

        }

        public static class ProductRetrieval
        {
            public const string Default = GroupName + ".ProductRetrieval";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";

        }

        public static class FamilyLib
        {
            public const string Default = GroupName + ".FamilyLib";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";

        }

        public static class StandardPolicy
        {
            public const string Default = GroupName + ".StandardPolicy";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class ProcessingList
        {
            public const string Default = GroupName + ".ProcessingList";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class ListProcessing
        {
            public const string Default = GroupName + ".ListProcessing";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class MeasuringExcel
        {
            public const string Default = GroupName + ".MeasuringExcel";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }
        //public static class NewProductList
        //{
        //    public const string Default = GroupName + ".NewProductList";
        //    public const string Delete = Default + ".Delete";
        //    public const string Update = Default + ".Update";
        //    public const string Create = Default + ".Create";
        //}

        public static class SaleOder
        {
            public const string Default = GroupName + ".SaleOder";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class NewFamilyLib
        {
            public const string Default = GroupName + ".NewFamilyLib";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        
        public static class MaterialManage
        {
            public const string Default = GroupName + ".MaterialManage";
            public const string Delete = Default + ".Delete";
            public const string Update = Default + ".Update";
            public const string Create = Default + ".Create";
        }

        public static class ArchitectureList
        {
            public const string Default = GroupName + ".ArchitectureList";
            public const string SolidifyEdit = Default + ".SolidifyEdit";
            public const string ProjectEdit = Default + ".ProjectEdit";
            //public const string View = Default + ".View";
            public const string Download = Default + ".Download";
        }

    }
}