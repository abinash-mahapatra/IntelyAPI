namespace IntelyAPI.Logic
{
    public class Security
    {

        static string key = "dsg_6.00";
        public static string EncryptPassword(string data)
        {
            string retrunstring = string.Empty;
            try
            {
                // byte[] bytes = Encoding.ASCII.GetBytes(data);
                retrunstring = RSA.Encrypt(data, key);
            }
            catch (Exception ex)
            {

            }
            return retrunstring;

        }
        public static string DecryptPassword(string data)
        {
            string retrunstring = string.Empty;

            try
            {
                // byte[] bytes = Encoding.ASCII.GetBytes(data);
                retrunstring = RSA.Decrypt(data, key);
            }
            catch (Exception ex)
            {

            }
            return retrunstring;

        }
    }
}
