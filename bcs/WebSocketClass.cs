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
    class WebSocketClass
    {
        private ProjectClass Project;
        private WebSocket ws;
        private string wsURL;
        private string wsPath;
        private bool IsStoping;

        public bool IsAlive { get => ws.IsAlive; }

        public WebSocketClass(ref ProjectClass AProject, string AwsURL, string AwsPath)
        {
            Project = AProject;
            wsURL = AwsURL;
            wsPath = AwsPath;
            IsStoping = false;
            ws = new WebSocket(AwsURL + AwsPath);
            ws.OnOpen += HandlerOpen;
            ws.OnMessage += HandlerMessage;
            ws.OnClose += HandlerClose;
            ws.OnError += HandlerError;
        }

        private void HandlerError(object sender, ErrorEventArgs e)
        {
            MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
