using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace _5PaisaLibrary
{
    internal class WebSocket : IWebsocket

    {
        ManualResetEvent receivedEvent = new ManualResetEvent(false);
        int receivedCount = 0;
        ClientWebSocket _ws;
        string _url = "wss://Openfeed.5paisa.com/Feeds/";
        public event EventHandler<MessageEventArgs> MessageReceived;

       
        public class MessageEventArgs : EventArgs
        {
            public string Message { get; set; }
        }
        public void Connect(string Url, string JwtToken,string ClientCode)
        {
            _url = Url + "api/chat?Value1=" + JwtToken + "|" + ClientCode;
            try
            {
                // Initialize ClientWebSocket instance and connect with Url
                _ws = new ClientWebSocket();
                _ws.ConnectAsync(new Uri(_url), CancellationToken.None).Wait();
            }

            catch (Exception e)
            {

                return;
            }

        }

        public void Connect(string jwttoken, string clientcode)
        {
          string URL = _url + "api/chat?Value1=" + jwttoken + "|" + clientcode;
            try
            {
                // Initialize ClientWebSocket instance and connect with Url
                _ws = new ClientWebSocket();


                _ws.ConnectAsync(new Uri(_url), CancellationToken.None).Wait();
            }

            catch (Exception e)
            {

                return;
            }
        }
    }
}
