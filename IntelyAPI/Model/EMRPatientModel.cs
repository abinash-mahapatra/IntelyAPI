namespace IntelyAPI.Model
{
    public class EMRPatientModel
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public string type { get; set; }
        public List<links> link { get; set; }
        public List<entrys> entry { get; set; }

    }
    public class links
    {
        public string relation { get; set; }
        public string url { get; set; }
    }
    public class entrys
    {
        public string fullUrl { get; set; }
        public resource resource { get; set; }
        public search  search { get; set; }
    }
    public class resource
    {
        public string resourceType { get; set; }
        public string id { get; set; }
        public meta meta { get; set; }
        public text text { get; set; }
        public List<identifier> identifier { get; set; }
        public bool active { get; set; }
        public List<name> name { get; set; }
        public List<telecom> telecom { get; set; }
        public string gender { get; set; }
        public string birthDate { get; set; }
        public List<address> address { get; set; }
        public maritalStatus maritalStatus { get; set; }
        public List<communication> communication { get; set; }
        public List<generalPractitioner> generalPractitioner { get; set; }
    }
    public class meta
    {
        public string versionId { get; set; }
        public string lastUpdatednId { get; set; }
    }
    public class text
    {
        public string status { get; set; }
        public string div { get; set; }
    }
    public class identifier
    {
        public string id { get; set; }
        public string use { get; set; }
        public type type { get; set; }
        public string system { get; set; }
        public string value { get; set; }
        public values _value { get; set; }
        public period period { get; set; }
    }
    public class type
    {
        public List<coding>  coding { get; set; }
        public string text { get; set; }
    }
    public class coding
    {
        public string system { set; get; }
        public string code { set; get; }
        public string display { set; get; }
        public bool userSelected { get; set; }
    }
    public class values
    {
        public List<extensions> extension { get; set; }
    }
    public class extensions
    {
        public string valueString { get; set; }
        public string url { get; set; }
    }
    public class period
    {
        public string start { get; set; }
    }
    public class search
    {
        public string mode { get; set; }
    }
    public class name
    {
        public string id { set; get; }
        public string use { set; get; }
        public string text { set; get; }
        public string family { set; get; }
        public List<string> given { get; set; }
    }
    public class telecom
    {
        public string id { set; get; }
        public List<extensions> extension { set; get; }
        public string system { get; set; }
        public string value { set; get; }
        public string use { get; set; }
        public int rank { set; get; }
        public period period { set; get; }
    }
    public class address
    {
        public string id { set; get; }
        public string home { set; get; }
        public string text { get; set; }
        public List<string > line { get; set; }
        public string city { set; get; }
        public string district { set; get; }
        public string state { set; get; }
        public string postalCode { set; get; }
        public string country { set; get; }
        public period period { get; set; }
    }
    public class maritalStatus
    {
        public List<coding> coding { get; set; }
        public string text { set; get; }
    }
    public class communication
    {
        public languages language { get; set; }
        public bool preferred { set; get; }
    }
    public class languages
    {
        public List<coding> coding { get; set; }
        public string text { set; get; }
    }
    public class generalPractitioner
    {
        public string id { set; get; }
        public string reference { set; get; }
        public string display { set; get; }
    }
}
