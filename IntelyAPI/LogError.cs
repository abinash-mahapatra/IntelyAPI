namespace IntelyAPI
{
    public class LogError
    {
        public void logsData(string value)
        {
            string path = $"logs/log_{DateTime.Now.ToString("dd - MMM - yyyy_HH - mm - ss").ToUpper()}.txt";

            using (var writer = (File.Exists(path)) ? File.AppendText(path) : File.CreateText(path))
            {
                writer.WriteLine($"[{DateTime.Now}] : {value}");
            }
        }
    }
}
