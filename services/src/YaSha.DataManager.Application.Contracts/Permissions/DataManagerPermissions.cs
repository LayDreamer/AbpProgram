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

       
       
    }
}