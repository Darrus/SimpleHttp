using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace SimpleHttp
{
    [Serializable]
    public struct PostData
    {
        public string body;
        public string contentType;
    }

    [Serializable]
    public struct ResponseData
    {
        public string body;
        public string contentType;
        public string method;
        public string statusCode;
        public string uri;
    }

    public class HttpAPI : MonoBehaviour
    {
        public delegate void ReceiveCallback(ResponseData response);

        private enum HTTP_VERB
        {
            GET,
            POST
            // Not implemented yet
            //PUT,
            //PATCH,
            //DELETE
        }

        public static void Get(string uri, Dictionary<string, string> headers = null, ReceiveCallback callback = null, X509Certificate certificate = null)
        {
            // Force project to use TLS 1.2 (NOTE: If you're using Unity, Set Unity .net framework to 4.6)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = HTTP_VERB.GET.ToString();

            if (certificate != null)
                request.ClientCertificates.Add(certificate);

            if (headers != null)
                foreach(KeyValuePair<string, string> pair in headers)
                    request.Headers.Add(pair.Key, pair.Value);

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            ResponseData respData = ConvertResponse(response);
            if (callback == null)
                callback = DefaultReceiveCallback;
            callback(respData);
        }

        public static void Post(string uri, PostData postData, Dictionary<string, string> headers = null, ReceiveCallback callback = null, X509Certificate certificate = null)
        {
            // Force project to use TLS 1.2 (NOTE: If you're using Unity, Set Unity .net framework to 4.6)
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            byte[] buffer = Encoding.ASCII.GetBytes(postData.body);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = HTTP_VERB.POST.ToString();

            if (certificate != null)
                request.ClientCertificates.Add(certificate);

            if (headers != null)
                foreach (KeyValuePair<string, string> pair in headers)
                    request.Headers.Add(pair.Key, pair.Value);

            request.ContentType = postData.contentType;
            request.ContentLength = buffer.Length;
            Stream postStream = request.GetRequestStream();
            postStream.Write(buffer, 0, buffer.Length);
            postStream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            ResponseData respData = ConvertResponse(response);
            if (callback == null)
                callback = DefaultReceiveCallback;
            callback(respData);
        }

        private static void DefaultReceiveCallback(ResponseData response)
        {
            Debug.Log(response.body);
        }

        private static ResponseData ConvertResponse(HttpWebResponse response)
        {
            ResponseData data = new ResponseData();
            using (Stream respStream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(respStream))
            {
                data.body = reader.ReadToEnd();
            }
            data.contentType = response.ContentType;
            data.method = response.Method;
            data.statusCode = response.StatusCode.ToString();
            data.uri = response.ResponseUri.ToString();

            return data;
        }
    }
}

