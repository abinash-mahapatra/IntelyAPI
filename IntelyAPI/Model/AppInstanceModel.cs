namespace IntelyAPI.Model
{
    public class AppInstanceModel
    {
        public int? totalNumberOfResults { get; set; }
        public int? currentResultSetNumber { get; set; }
        public int? resultSetSize { get; set; }
        public int? numberOfResultSets { get; set; }
        public bool? isPaginated { get; set; }
        public List<results> results { get; set; }
    }
    public class results
    {
        public string? interfaceId { get; set; }
        public string? name { get; set; }
        public string? description { get; set; }
        public List<fields> fields { get; set; }
        public string? createdBy { get; set; }
        public string? updatedBy { get; set; }
        public string? _id { get; set; }
        public string? createdAt { get; set; }
        public string? updatedAt { get; set; }
        public string? appId { get; set; }
        public string? appName { get; set; }
        public string? appDescription { get; set; }
        public string? appCategory { get; set; }
        public string? appImage { get; set; }

    }
    public class fields
    {
        public string? fieldId { get; set; }
        public string? value { get; set; }
        public string? _id { get; set; }
    }
}
