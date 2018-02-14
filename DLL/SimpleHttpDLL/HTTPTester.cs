using UnityEngine;
using System.IO;
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;

namespace SimpleHttp
{
    public class HTTPTester : MonoBehaviour
    {
        public string uri;
        public string[] listenURI;
        public PostData postData;

        public void Get()
        {
            try
            {
                HttpAPI.Get(uri);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }

        public void GetWithCert(X509Certificate cert = null)
        {
            try
            {
                if (cert == null)
                {
                    // Example Cert
                    cert = new X509Certificate2(Path.Combine(Application.dataPath, "client.pfx"), "amoeba", X509KeyStorageFlags.MachineKeySet);
                }
                HttpAPI.Get(uri, null, null, cert);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void Post()
        {
            HttpAPI.Post(uri, postData);
        }

        public void PostWithCert(X509Certificate cert = null)
        {
            try
            {
                if (cert == null)
                {
                    // Example Cert
                    cert = new X509Certificate2(Path.Combine(Application.dataPath, "client.pfx"), "amoeba", X509KeyStorageFlags.MachineKeySet);
                }
                HttpAPI.Post(uri, postData, null, null, cert);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public void ListenToCurrentIP()
        {
            string url = "http://";
            url += GetLocalIPAddress();
            foreach(string s in listenURI)
                url += s;

            if (url[url.Length - 1] != '/')
                url += '/';

            string[] prefixes = { url };
            HttpListenerHelper.StartListening(prefixes);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}
