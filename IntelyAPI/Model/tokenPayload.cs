using Newtonsoft.Json;

namespace IntelyAPI.Model
{
    public class tokenPayload
    {
       
        public string? email { get; set; }
        public string? userId { get; set; }  
        public string? firstName { get; set; }
        public string? lastName { get; set; }
        public string? defaultOrganizationId { get; set; }
        public string? scope { get; set; }
        public int? iat { get; set; }
        public string? iss { get; set; }
        public int? exp { get; set; }


    }
}
