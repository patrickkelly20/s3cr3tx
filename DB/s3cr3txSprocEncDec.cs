using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SqlServer.Server;
using System.Data.SqlTypes;
using System.Security.Cryptography;
using System.Text;
namespace PKSQLSecrets
{
    public class uspEncDec
    {
        [Microsoft.SqlServer.Server.SqlProcedure]
        public static void EncDec(string email, string input, bool EorD, bool blnDefault, out string strText) //, out string text)
        {
            try
            {
                string result = "";
                if (blnDefault)
                {
                    ebundle eb = ebundle.GetEbundle(@"pmkelly2@icloud.com");
                    if (EorD)
                    {
                        using (RSACryptoServiceProvider rSA1 = new RSACryptoServiceProvider())
                        {
                            rSA1.FromXmlString(eb.kyp);
                            byte[] bytInput = System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(0), System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0).GetBytes(input));
                            byte[] pubEnc = rSA1.Encrypt(bytInput, true);
                            string strB64 = System.Convert.ToBase64String(pubEnc);
                            result = strB64;
                        }
                    }
                    else
                    {
                        string strB64Pri = eb.ky;
                        //string strPriXML = System.Text.Encoding.GetEncoding(0).GetString(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0), System.Convert.FromBase64String(strB64Pri)));
                        RSACryptoServiceProvider rSA2 = new RSACryptoServiceProvider();
                        rSA2.FromXmlString(strB64Pri);
                        string stringDec = System.Text.Encoding.GetEncoding(0).GetString(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0), (rSA2.Decrypt(System.Convert.FromBase64String(input), true))));
                        result = stringDec;
                    }
                }
                else if (EorD)
                {
                    ebundle eb = ebundle.GetEbundle(email);
                    using (RSACryptoServiceProvider rSA1 = new RSACryptoServiceProvider())
                    {
                        rSA1.FromXmlString(eb.kyp);
                        byte[] bytInput = System.Text.Encoding.Convert(System.Text.Encoding.GetEncoding(0), System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0).GetBytes(input));
                        byte[] pubEnc = rSA1.Encrypt(bytInput, true);
                        string strB64 = System.Convert.ToBase64String(pubEnc);
                        result = strB64;
                    }

                }
                else
                {
                    ebundle eb = ebundle.GetEbundle(email);
                    string strB64Pri = eb.ky;
                    //string strPriXML = System.Text.Encoding.GetEncoding(0).GetString(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0), System.Convert.FromBase64String(strB64Pri)));
                    RSACryptoServiceProvider rSA2 = new RSACryptoServiceProvider();
                    rSA2.FromXmlString(strB64Pri);
                    string stringDec = System.Text.Encoding.GetEncoding(0).GetString(System.Text.Encoding.Convert(System.Text.Encoding.UTF8, System.Text.Encoding.GetEncoding(0), (rSA2.Decrypt(System.Convert.FromBase64String(input), true))));
                    result = stringDec;
                }

                strText = result;
                SqlContext.Pipe.Send(result);
                //text = result;
            }
            catch (Exception ex)
            {
                using (SqlConnection con = new SqlConnection("context connection=true"))
                {

                    using (SqlCommand command = new SqlCommand())
                    {
                        command.CommandText = @"dbo.insertLog";
                        command.CommandType = CommandType.StoredProcedure;
                        SqlParameter p1 = new SqlParameter(@"source", @"EncDec_Sproc");
                        SqlParameter p2 = new SqlParameter(@"logMessage", ex.GetBaseException().ToString());
                        con.Open();
                        command.Connection = con;
                        command.Parameters.Add(p1);
                        command.Parameters.Add(p2);
                        int i = command.ExecuteNonQuery();
                        strText = @" ";
                    }
                }
            }
        }

    }

    class ebundle
    {
        public string strG1;
        public string strG2;
        public string strapikey;
        public string strauth;
        public string ky;
        public string kyp;
        public ebundle() { }

        public ebundle(string G1, string G2, string apikey, string auth, string strKy, string strKyp)
        {
            strG1 = G1;
            strG2 = G2;
            strapikey = apikey;
            strauth = auth;
            ky = strKy;
            kyp = strKyp;
        }

        public static ebundle GetEbundle(string email)
        {
            using (SqlConnection con = new SqlConnection("context connection=true"))
            {
                //string strConnection = "Server=localhost;Database=S;User Id=sa;Password=Sunsh1n3-20p;";
                //SqlConnection con = new SqlConnection(strConnection);
                using (SqlCommand command = new SqlCommand())
                {
                    command.CommandText = @"dbo.getBundle";
                    command.CommandType = CommandType.StoredProcedure;
                    SqlParameter p1 = new SqlParameter(@"email", email);
                    con.Open();
                    command.Connection = con;
                    command.Parameters.Add(p1);
                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(ds);
                    ebundle eb = new ebundle();
                    eb.strapikey = ds.Tables[0].Rows[0].ItemArray[4].ToString();
                    eb.strauth = ds.Tables[0].Rows[0].ItemArray[5].ToString();
                    eb.ky = Encoding.GetEncoding(0).GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(0), Convert.FromBase64String(ds.Tables[0].Rows[0].ItemArray[6].ToString().Trim())));
                    eb.kyp = Encoding.GetEncoding(0).GetString(Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding(0), Convert.FromBase64String(ds.Tables[0].Rows[0].ItemArray[7].ToString().Trim())));
                    eb.strG2 = ds.Tables[0].Rows[0].ItemArray[5].ToString();
                    return eb;
                }
            }
        }
    }
}
