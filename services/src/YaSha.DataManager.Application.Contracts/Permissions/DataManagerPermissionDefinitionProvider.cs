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


            var newFamilyLibPermission = myGroup.AddPermission(DataManagerPermissions.NewFamilyLib.Default, L("Permission:NewFamilyLib"));
            newFamilyLibPermission.AddChild(DataManagerPermissions.NewFamilyLib.Delete, L("Permission:Delete"));
            newFamilyLibPermission.AddChild(DataManagerPermissions.NewFamilyLib.Create, L("Permission:Create"));
            newFamilyLibPermission.AddChild(DataManagerPermissions.NewFamilyLib.Update, L("Permission:Update"));



            var processingListPermission = myGroup.AddPermission(DataManagerPermissions.ProcessingList.Default, L("Permission:ProcessingList"));
            processingListPermission.AddChild(DataManagerPermissions.ProcessingList.Delete, L("Permission:Delete"));
            processingListPermission.AddChild(DataManagerPermissions.ProcessingList.Create, L("Permission:Create"));
            processingListPermission.AddChild(DataManagerPermissions.ProcessingList.Update, L("Permission:Update"));


            var listProcessingPermission = myGroup.AddPermission(DataManagerPermissions.ListProcessing.Default, L("Permission:ListProcessing"));
            listProcessingPermission.AddChild(DataManagerPermissions.ListProcessing.Delete, L("Permission:Delete"));
            listProcessingPermission.AddChild(DataManagerPermissions.ListProcessing.Create, L("Permission:Create"));
            listProcessingPermission.AddChild(DataManagerPermissions.ListProcessing.Update, L("Permission:Update"));

            var measuringExcelPermission = myGroup.AddPermission(DataManagerPermissions.MeasuringExcel.Default, L("Permission:MeasuringExcel"));
            measuringExcelPermission.AddChild(DataManagerPermissions.MeasuringExcel.Delete, L("Permission:Delete"));
            measuringExcelPermission.AddChild(DataManagerPermissions.MeasuringExcel.Create, L("Permission:Create"));
            measuringExcelPermission.AddChild(DataManagerPermissions.MeasuringExcel.Update, L("Permission:Update"));


            var saleOderPermission = myGroup.AddPermission(DataManagerPermissions.SaleOder.Default, L("Permission:SaleOder"));
            saleOderPermission.AddChild(DataManagerPermissions.SaleOder.Delete, L("Permission:Delete"));
            saleOderPermission.AddChild(DataManagerPermissions.SaleOder.Create, L("Permission:Create"));
            saleOderPermission.AddChild(DataManagerPermissions.SaleOder.Update, L("Permission:Update"));


            var productListPermission = myGroup.AddPermission(DataManagerPermissions.ProductList.Default, L("Permission:ProductList"));
            productListPermission.AddChild(DataManagerPermissions.ProductList.Delete, L("Permission:Delete"));
            productListPermission.AddChild(DataManagerPermissions.ProductList.Create, L("Permission:Create"));
            productListPermission.AddChild(DataManagerPermissions.ProductList.Update, L("Permission:Update"));


            //var newProductListPermission = myGroup.AddPermission(DataManagerPermissions.NewProductList.Default, L("Permission:NewProductList"));
            //newProductListPermission.AddChild(DataManagerPermissions.NewProductList.Delete, L("Permission:Delete"));
            //newProductListPermission.AddChild(DataManagerPermissions.NewProductList.Create, L("Permission:Create"));
            //newProductListPermission.AddChild(DataManagerPermissions.NewProductList.Update, L("Permission:Update"));

            
            var productRetrievalPermission = myGroup.AddPermission(DataManagerPermissions.ProductRetrieval.Default, L("Permission:ProductRetrieval"));
            productRetrievalPermission.AddChild(DataManagerPermissions.ProductRetrieval.Delete, L("Permission:Delete"));
            productRetrievalPermission.AddChild(DataManagerPermissions.ProductRetrieval.Create, L("Permission:Create"));
            productRetrievalPermission.AddChild(DataManagerPermissions.ProductRetrieval.Update, L("Permission:Update"));


            var standardPolicyPermission = myGroup.AddPermission(DataManagerPermissions.StandardPolicy.Default, L("Permission:StandardPolicy"));
            standardPolicyPermission.AddChild(DataManagerPermissions.StandardPolicy.Delete, L("Permission:Delete"));
            standardPolicyPermission.AddChild(DataManagerPermissions.StandardPolicy.Create, L("Permission:Create"));
            standardPolicyPermission.AddChild(DataManagerPermissions.StandardPolicy.Update, L("Permission:Update"));
            
            
            var materialManagePermission = myGroup.AddPermission(DataManagerPermissions.MaterialManage.Default, L("Permission:MaterialManage"));
            materialManagePermission.AddChild(DataManagerPermissions.MaterialManage.Delete, L("Permission:Delete"));
            materialManagePermission.AddChild(DataManagerPermissions.MaterialManage.Create, L("Permission:Create"));
            materialManagePermission.AddChild(DataManagerPermissions.MaterialManage.Update, L("Permission:Update"));

            var architectureListPermission = myGroup.AddPermission(DataManagerPermissions.ArchitectureList.Default, L("Permission:ArchitectureList"));
            architectureListPermission.AddChild(DataManagerPermissions.ArchitectureList.SolidifyEdit, L("Permission:SolidifyEdit"));
            architectureListPermission.AddChild(DataManagerPermissions.ArchitectureList.ProjectEdit, L("Permission:ProjectEdit"));
            //architectureListPermission.AddChild(DataManagerPermissions.ArchitectureList.View, L("Permission:View"));
            architectureListPermission.AddChild(DataManagerPermissions.ArchitectureList.Download, L("Permission:Download"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<DataManagerResource>(name);
        }
    }
}