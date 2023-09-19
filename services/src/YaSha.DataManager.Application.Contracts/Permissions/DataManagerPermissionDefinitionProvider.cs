using Volo.Abp.MultiTenancy;

namespace YaSha.DataManager.Permissions
{
    public class DataManagerPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(DataManagerPermissions.GroupName, L("Permission:YaShaDataManager"));
            //var productManagementGroup = context.AddGroup(DataManagerPermissions.ProductManagement.Default, L("Permission:ProductManagement"));

            var familyLibPermission = myGroup.AddPermission(DataManagerPermissions.FamilyLib.Default, L("Permission:FamilyLib"));
            familyLibPermission.AddChild(DataManagerPermissions.FamilyLib.Delete, L("Permission:Delete"));
            familyLibPermission.AddChild(DataManagerPermissions.FamilyLib.Create, L("Permission:Create"));
            familyLibPermission.AddChild(DataManagerPermissions.FamilyLib.Update, L("Permission:Update"));

            var productListPermission = myGroup.AddPermission(DataManagerPermissions.ProductList.Default, L("Permission:ProductList"));
            productListPermission.AddChild(DataManagerPermissions.ProductList.Delete, L("Permission:Delete"));
            productListPermission.AddChild(DataManagerPermissions.ProductList.Create, L("Permission:Create"));
            productListPermission.AddChild(DataManagerPermissions.ProductList.Update, L("Permission:Update"));

            var productRetrievalPermission = myGroup.AddPermission(DataManagerPermissions.ProductRetrieval.Default, L("Permission:ProductRetrieval"));
            productRetrievalPermission.AddChild(DataManagerPermissions.ProductRetrieval.Delete, L("Permission:Delete"));
            productRetrievalPermission.AddChild(DataManagerPermissions.ProductRetrieval.Create, L("Permission:Create"));
            productRetrievalPermission.AddChild(DataManagerPermissions.ProductRetrieval.Update, L("Permission:Update"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<DataManagerResource>(name);
        }
    }
}