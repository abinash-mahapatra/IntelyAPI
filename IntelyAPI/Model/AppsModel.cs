namespace IntelyAPI.Model
{
    public class AppsModel
    {
        public int count { get; set; }
        public List<datas> data { get; set; }
        public string createdAt { get; set; }
        public string createdBy { get; set; }
        public string updatedAt { get; set; }
        public string updatedBy { get; set; }
        public string deletedAt { get; set; }
        public string deletedBy { get; set; }
    }
    public class datas
    {
        public string _id { get; set; }
        public string organizationId { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string category { get; set; }
        public string image { get; set; }
        public string homepageUrl { get; set; }
        public bool isEnabled { get; set; }
        public bool isPublic { get; set; }
        public bool isStandard { get; set; }
        public List<interfaces> interfaces { get; set; }
        public List<instanceFields> instanceFields { get; set; }
        public string createdAt { get; set; }
        public string createdBy { get; set; }
        public string updatedAt { get; set; }
        public string updatedBy { get; set; }
        public string deletedAt { get; set; }
        public string deletedBy { get; set; }
    }
    public class interfaces
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public bool isEnabled { get; set; }
        public string className { get; set; }
        public string baseUrl { get; set; }
        public string tokenUrl { get; set; }
        public oauth oauth { get; set; }
        public List<programFields> programFields { get; set; }
        public rateLimit rateLimit { get; set; }
        public authorizationMethod authorizationMethod { get; set; }
        public List<resources> resources { get; set; }
        public string createdAt { get; set; }
        public string createdBy { get; set; }
        public string updatedAt { get; set; }
        public string updatedBy { get; set; }
        public string deletedAt { get; set; }
        public string deletedBy { get; set; }
    }
    public class instanceFields
    {
        public string _id { get; set; }
        public string name { get; set; }
        public string displayName { get; set; }
        public string inputType { get; set; }
        public List<inputTypeSelectOptions> inputTypeSelectOptions { get; set; }
        public bool required { get; set; }
        public string mappingType { get; set; }
        public List<resourceMappings> resourceMappings { get; set; }
        public bool display { get; set; }
        public string createdAt { get; set; }
        public string createdBy { get; set; }
        public string updatedAt { get; set; }
        public string updatedBy { get; set; }
        public string deletedAt { get; set; }
        public string deletedBy { get; set; }
    }
    public class inputTypeSelectOptions
    {
        public string value { get; set; }
        public string text { get; set; }
    }
    public class resourceMappings
    {
        public string resourceId { get; set; }
        public string actionId { get; set; }
        public string fieldType { get; set; }
        public string field { get; set; }
    }
    public class oauth
    {
        public string authorizationUrl { get; set; }
        public string scopes { get; set; }
        public string clientId { get; set; }
        public string clientSecret { get; set; }
    }
    public class programFields
    {
        public string name { get; set; }
        public string value { get; set; }
    }
    public class rateLimit
    {
        public int numberOfCalls { get; set; }
        public int duration { get; set; }
        public string durationUnits { get; set; }
    }
    public class authorizationMethod
    {
        public string method { get; set; }
        public apiKeyInfo apiKeyInfo { get; set; }
    }
    public class apiKeyInfo 
    { 
        public string name { get; set; }
        public string location { get; set; } 
    }
    public class resources 
    { 
        public string _id { get; set; } 
        public string name { get; set; } 
        public string description { get; set; } 
        public bool isEnabled { get; set; } 
        public List<actions> actions { get; set; } 
        public string createdAt { get; set; }
        public string createdBy { get; set; } 
        public string updatedAt { get; set; } 
        public string updatedBy { get; set; }
        public string deletedAt { get; set; } 
        public string deletedBy { get; set; } 
    }
    public class actions 
    { 
        public string _id { get; set; } 
        public string name { get; set; } 
        public string description { get; set; } 
        public bool isEnabled { get; set; } 
        public string path { get; set; } 
        public string method { get; set; } 
        public bool requireAuthorization { get; set; } 
        public List<parameters> parameters { get; set; } 
        public requestBody requestBody { get; set; } 
        public List<constraints> constraints { get; set; } 
        public List<responses> responses { get; set; } 
        public string createdAt { get; set; } 
        public string createdBy { get; set; } 
        public string updatedAt { get; set; } 
        public string updatedBy { get; set; } 
        public string deletedAt { get; set; }
        public string deletedBy { get; set; }
    }
    public class parameters 
    { 
        public string name { get; set; } 
        public string displayName { get; set; }
        public string description { get; set; }
        public string type { get; set; } 
        public string dataType { get; set; } 
    }
    public class requestBody 
    { 
        public string contentType { get; set; } 
        public string schema { get; set; } 
    }
    public class constraints 
    { 
        public string requirement { get; set; } 
        public string type { get; set; } 
        public List<string> fields { get; set; } 
    }
    public class responses 
    { 
        public int statusCode { get; set; } 
        public string description { get; set; }
        public object responseBodySchema { get; set; } 
    }
}
