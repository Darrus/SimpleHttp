using UnityEngine;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System;
using System.IO;
using System.Text;

namespace SimpleHttp
{
    public class HttpListenerHelper : MonoBehaviour
    {
        public delegate void ListenerCallback(HttpListenerContext context);

        static bool running = true;

        public static bool StartListening(string[] prefixes, ListenerCallback callback = null, bool continous = false)
        {
            // Used Lambda Expression as a workaround.
            return ThreadPool.QueueUserWorkItem((System.Object obj) => ListeningThread(prefixes, callback != null ? callback : CallbackExample, continous));
        }

        public static void ConstructResponse(ref HttpListenerResponse response, string body = null, int statusCode = 200)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(body);
            response.ContentLength64 = buffer.Length;
            response.StatusCode = statusCode;
            Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            output.Close();
        }

        public static string GetRequestBody(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                Console.WriteLine("No client data was sent with the request.");
                return "";
            }

            Stream body = request.InputStream;
            Encoding encoding = request.ContentEncoding;
            Debug.Log(encoding.EncodingName);
            Debug.Log(request.ContentType);

            StreamReader reader = new StreamReader(body, encoding);
            if (request.ContentType != null)
            {
                Debug.Log("Client data content type " + request.ContentType);
            }
            Debug.Log("Client data content length " + request.ContentLength64.ToString());

            string s = reader.ReadToEnd();
            body.Close();
            reader.Close();

            return s;
        }

        static void ListeningThread(string[] prefixes, ListenerCallback callback = null, bool continous = false)
        {
            if (!HttpListener.IsSupported)
            {
                Debug.LogError("Windows XP SP2 or Server 2003 is required to use the HttpListener class.");
                return;
            }

            if (prefixes == null || prefixes.Length == 0)
            {
                Debug.LogError("Missing prefixes");
                return;
            }

            try
            {
                HttpListener listener = new HttpListener();
                foreach (string s in prefixes)
                {
                    listener.Prefixes.Add(s);
                }

                listener.Start();
                do
                {
                    Debug.Log("Listening...");
                    Debug.Log("Waiting for request to be processed asyncronously.");
                    HttpListenerContext context = listener.GetContext();
                    callback(context);
                    //IAsyncResult result = listener.GetContext()
                    //result.AsyncWaitHandle.WaitOne();
                    Debug.Log("Request processed asyncronously.");
                } while (continous && running);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        static void CallbackExample(HttpListenerContext result)
        {
            //HttpListener listener = (HttpListener)result.AsyncState;
            //HttpListenerContext context = result.EndGetContext(result);
            HttpListenerRequest request = result.Request;
            HttpListenerResponse response = result.Response;

            // Construct a response.
            // Note: You MUST return a response
            ConstructResponse(ref response, "Succesfully received.");
        }

        private void OnApplicationExit()
        {
            running = false;
        }
    }
}
