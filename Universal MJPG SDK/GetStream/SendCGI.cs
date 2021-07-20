using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;


namespace QuickTest
{
    public class CSendCGI
    {
        private string mIP;
        private string mUser;
        private string mPass;
        private int mTimeout;

        public void Init(string ip ,string user ,string pass)
        {
            mIP = ip;
            mUser = user;
            mPass = pass;
            mTimeout = -1;
        }

        public void Init(string ip, string user, string pass, int timeout)
        {
            mIP = ip;
            mUser = user;
            mPass = pass;
            mTimeout = timeout;
        }

        private WebRequest CreateWebRequest(string url, string admin, string pass)
        {
            WebRequest myWebRequest = WebRequest.Create(url);
            myWebRequest.Method = "GET";

            if (mTimeout > 0)
            {
                myWebRequest.Timeout = mTimeout;
            }
            else
            {
                myWebRequest.Timeout = 10000;
            }

            /*
             * 因為使用new NetworkCredential(admin, pass)
             * 第一次會先用沒有帳號密碼的方式送出CGI
             * 所以直接將帳號密碼強制加到Headers
             */
            string code = Convert.ToBase64String(Encoding.ASCII.GetBytes(string.Format("{0}:{1}", admin, pass)));
            myWebRequest.Headers.Add("Authorization", "Basic " + code);
            
            myWebRequest.Credentials = new NetworkCredential(admin, pass);
            return myWebRequest;
        }

        public bool Send(string cgi, ref string req)
        {
            bool res = false;
            //HttpWebRequest request = (HttpWebRequest)CreateWebRequest("http://" + mIP + "/" + cgi, mUser, mPass);
            HttpWebRequest request = (HttpWebRequest)CreateWebRequest("http://" + mIP  + cgi, mUser, mPass);
            WebResponse response = null;
            StreamReader streamReader = null;
            try
            {
                using (response = request.GetResponse())
                {
                    using (streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        req += streamReader.ReadToEnd();
                        streamReader.Close();
                    }
                    response.Close();
                }

                request = null;
                response = null;
                streamReader = null;

                res = true;
            }
            catch (WebException err)
            {
                using (response = err.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    //using (streamReader = new StreamReader(response.GetResponseStream()))
                    //{
                    //    Console.WriteLine(streamReader.ReadToEnd());
                    //}
                }

                if (streamReader != null)
                    streamReader.Close();

                if (response != null)
                    response.Close();

                request = null;
                response = null;
                streamReader = null;

            }

            return res;
        }

        public delegate void CallbackEventHandler(string something);
        public event CallbackEventHandler Callback;

        public void DoRequest(string request)
        {
            if (Callback != null)
                Callback(request);
        }

        public bool SendStop = false;
        private HttpWebRequest wreqScrape;
        private CookieContainer cookieContainer;
        private HttpWebResponse wresScrape;

        public bool SendContinuous(string cgi, ref string req)
        {
            SendStop = false;
            bool res = false;
            HttpWebRequest request = (HttpWebRequest)CreateWebRequest("http://" + mIP + "/" + cgi, mUser, mPass);
            WebResponse response = null;
            StreamReader streamReader = null;
            try
            {
                using (response = request.GetResponse())
                {
                    using (streamReader = new StreamReader(response.GetResponseStream()))
                    {
                        string getstr = null;
                        while ( ((getstr = streamReader.ReadLine()) != null) && (SendStop == false) )
                        {
                            //Console.WriteLine(getstr);
                            DoRequest(getstr);
                        }
                        streamReader.Close();
                    }
                    response.Close();
                }

                request = null;
                response = null;
                streamReader = null;

                res = true;
            }
            catch (WebException err)
            {
                using (response = err.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    //Console.WriteLine("Error code: {0}", httpResponse.StatusCode);
                    //using (streamReader = new StreamReader(response.GetResponseStream()))
                    //{
                    //    Console.WriteLine(streamReader.ReadToEnd());
                    //}
                }

                if (streamReader != null)
                    streamReader.Close();

                if (response != null)
                    response.Close();

                request = null;
                response = null;
                streamReader = null;

            }

            return res;
        }  


    }
}
