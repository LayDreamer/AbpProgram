namespace YaSha.DataManager.DataDictionaryManagement.DataDictionaries.Dtos
{
    public class PagingDataDictionaryDetailInput : PagingBase
    {
        public Guid DataDictionaryId { get; set; }
        public string Filter { get; set; }
    }
}