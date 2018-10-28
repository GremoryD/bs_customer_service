using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Net;
using CLProject;
using System.Windows;
using System.Threading;

namespace bcs
{
    class WebSocketClientClass
    {
        private ProjectClass Project;
        private WebSocket ws = new WebSocket("ws://192.168.1.67:8091");
        private bool IsStoping;

        public WebSocketClientClass(ref ProjectClass AProject)
        {
            Project = AProject;
            IsStoping = false;
            ws.OnOpen += HandlerOpen;
            ws.OnMessage += HandlerMessage;
            ws.OnClose += HandlerClose;
        }

        private void HandlerOpen(object sender, EventArgs e)
        {
            MessageBox.Show("Open");
        }

        private void HandlerClose(object sender, CloseEventArgs e)
        {
            Connect();
        }

        private void HandlerMessage(object sender, MessageEventArgs e)
        {
            MessageBox.Show(e.Data);
        }

        public void Start()
        {
            IsStoping = false;
            Connect();
        }

        public void Stop()
        {
            IsStoping = true;
            ws.Close();
        }

        public void Send(string AMessage)
        {
            if (ws.IsAlive)
            {
                ws.Send(AMessage);
            }
        }

        private void Connect()
        {
            Thread Reconnect = new Thread(() =>
            {
                while (!ws.IsAlive && !IsStoping)
                {
                    try
                    {
                        ws.Connect();
                    }
                    catch { }
                    Thread.Sleep(1);
                }
            });
            Reconnect.Start();
        }
    }
}
