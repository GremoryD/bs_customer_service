using System;
using WebSocketSharp;
using CLProject;
using System.Windows;
using System.Threading;
using Newtonsoft.Json;

namespace BCS_User_Interface
{
    public class WebSocketClass
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
            //Project.Log.Error(e.Message);
        }

        private void HandlerOpen(object sender, EventArgs e)
        {
            //MessageBox.Show("Open");
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

        public void Send(object AMessage)
        { 
            if (ws.IsAlive)
            {
                ws.Send(JsonConvert.SerializeObject(AMessage));
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
