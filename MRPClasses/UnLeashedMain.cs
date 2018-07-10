using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MRPClasses
{
    public class UnLeashedMain
    {
        private const string ApiHost = "https://api.unleashedsoftware.com";



        public static string GetJson(string resource, string id, string key, string guid)
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
            const string query = "format=json&pageSize=1000";
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            //ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }

        public static string GetJsonForBillOfMaterials(string resource, string id, string key, string guid)
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
            const string query = "format=json&pageSize=500";
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            //ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }

        public static string GetPurchaseOrderJson(string resource, string id, string key, string guid)
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
            const string query = "format=json&pageSize=10";
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            //ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }
        public static string CheckPurchaseOrderPageNumber(string resource, string id, string key, string guid)
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
            const string query = "format=json&pageSize=1";
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }

        public static string GetLastPagePurchaseOrders(string resource, string id, string key, string guid,string pageSize)
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
             string query = "format=json&pageSize="+pageSize;
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }
        public static string GetSupplierJson(string resource, string id, string key, string guid, string supplierCode = "")
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
            string query = "format=json&pageSize=10&SupplierCode=" + supplierCode;
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }

        public static string GetWarehouseCodeJson(string resource, string id, string key, string guid, string WarehouseCode = "")
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
            string query = "format=json&pageSize=10&WarehouseCode=" + WarehouseCode;
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }

        public static string GetProductJson(string resource, string id, string key, string guid, string productCode = "")
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
            string query = "format=json&pageSize=10&ProductCode=" + productCode;
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }

        public static string GetProductGroupsJson(string resource, string id, string key, string guid)
        {
            string uri = guid != string.Empty ? string.Format("{0}/{1}/{2}", ApiHost, resource, guid) : string.Format("{0}/{1}", ApiHost, resource);

            var client = new WebClient();
            string query = "format=json&pageSize=20";
            SetAuthenticationHeaders(client, query, RequestType.Json, id, key);
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            return Get(client, string.Format("{0}?{1}", uri, query));
        }
        // allow untrusted certificate
        public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
            //if (sslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateNameMismatch))
            //    return true;
            //return false;
        }

        private static void SetAuthenticationHeaders(WebClient client, string query, RequestType requestType, string id, string key)
        {
            string signature = GetSignature(query, key);
            client.Headers.Add("api-auth-id", id);
            client.Headers.Add("api-auth-signature", signature);

            if (requestType == RequestType.Xml)
            {
                client.Headers.Add("Accept", "application/xml");
                client.Headers.Add("Content-Type", "application/xml; charset=" + client.Encoding.WebName);
            }
            else
            {
                client.Headers.Add("Accept", "application/json");
                client.Headers.Add("Content-Type", "application/json; charset=" + client.Encoding.WebName);
            }
        }
        private static string GetSignature(string args, string privatekey)
        {
            var encoding = new System.Text.ASCIIEncoding();
            byte[] key = encoding.GetBytes(privatekey);
            var myhmacsha256 = new HMACSHA256(key);
            byte[] hashValue = myhmacsha256.ComputeHash(encoding.GetBytes(args));
            string hmac64 = Convert.ToBase64String(hashValue);
            myhmacsha256.Clear();
            return hmac64;
        }

        private static string Get(WebClient client, string uri)
        {
            string response = string.Empty;
            try
            {
                response = client.DownloadString(uri);
            }
            catch (WebException ex)
            {
                if (ex.Response != null)
                {
                    var stream = ex.Response.GetResponseStream();
                    if (stream != null)
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            response = reader.ReadToEnd();
                        }
                    }
                }
            }
            return response;
        }

        public static string PostXml(string resource, string id, string key, string guid, string postData)
        {
            string uri = string.Format("{0}/{1}/{2}", ApiHost, resource, guid);
            var client = new WebClient();
            string query = string.Empty;
            SetAuthenticationHeaders(client, query, RequestType.Xml, id, key);
            ServicePointManager.ServerCertificateValidationCallback += ValidateServerCertificate;
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            string xml = Post(client, uri, postData);

            var xmlDocument = new XmlDocument { PreserveWhitespace = true };
            xmlDocument.LoadXml(xml);
            XmlNodeList _wordObjects = xmlDocument.SelectNodes("//Guid");
            foreach (XmlNode eachWordObj in _wordObjects)
            {
                String sText = eachWordObj.InnerText;
                Console.WriteLine(sText);
            }
            return xmlDocument.InnerXml;
        }
        private static string Post(WebClient client, string uri, string postData)
        {
            string response = string.Empty;
            try
            {
                response = client.UploadString(uri, "POST", postData);
            }
            catch (WebException ex)
            {
                var stream = ex.Response.GetResponseStream();
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        response = reader.ReadToEnd();
                    }
                }
            }
            return response;
        }
    }

    public enum RequestType { Xml = 0, Json = 1 }
}
