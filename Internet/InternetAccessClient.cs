#if DEBUG && INTERNET
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Net;
using System.IO;

namespace Photon.Database.Connection
{
    public class InternetAccessClient : IConnection
    {
        public InternetAccessClient(InternetConnectionPath URL)
        {
            this.url = URL;
        }

        InternetConnectionPath url;
        WebRequest webRequest;
        string cookie;
        WebResponse webRespons;
        Stream webStream;

        string Respond;

        public event ConnectionStingSetHandler ConnectionStringChange;
        public event EventHandler DataBaseTypeChange;

        public IConnection Clone()
        {
            return new InternetAccessClient(
                new InternetConnectionPath(url.ToString()));
        }

        public void Open()
        {
            webRequest = WebRequest.Create(url.ToString());
        }
        public int ExecuteNonQuery()
        {
            WebResponse webRespons = webRequest.GetResponse();
            Stream webPage = webRespons.GetResponseStream();

            int rcv; Respond = null;
            byte[] buffer = new byte[1];
            while (webPage.CanRead)
            {
                rcv = webPage.ReadByte();
                if (rcv <= 0) break;
                buffer[0] = (byte)rcv;
                Respond += Encoding.ASCII.GetString(buffer);
            }

            return int.Parse(Respond);
        }
        public void ExecuteReader()
        {
            webRespons = webRequest.GetResponse();
            webStream = webRespons.GetResponseStream();
        }
        public bool Read()
        {
            int rcv; Respond = null;
            byte[] buffer = new byte[1];

            while (webStream.CanRead)
            {
                rcv = webStream.ReadByte();
                if (rcv <= 0) return false;
                buffer[0] = (byte)rcv;
                Respond += Encoding.ASCII.GetString(buffer);
            }

            return true;
        }
        void CloseConnection();
        void CloseReader();

        byte[] GetBytes(int ordinal);
        string Cor(string index);
        string Cor(int index);
        string GetName(int index);

        object this[int index] { get; }
        object this[string index] { get; }

        string CommandText
        {
            set { cookie = value; }
            get { return cookie; }
        }
        ConnectionPath ConnectionString
        {
            set { url = (InternetConnectionPath)value; }
            get { return url; }
        }
        DataBaseTypes DBType { set; get; }
        bool ReaderIsClose { get; }
        ConnectionState State { get; }
        int FeildCount { get; }


    }
}
#endif