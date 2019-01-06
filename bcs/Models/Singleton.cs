using BCS_User_Interface;
using CLProject;
using Newtonsoft.Json;
using ServerLib.JTypes.Client;
using ServerLib.JTypes.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcs.Models
{ 

    public class Singleton
    {
        public event EventHandler<LoginClass> OnLogonSuccess;
        public event EventHandler<String> OnError;

        public String SessionID; 
        private static Singleton _instance;
        public static Singleton Instance
        {
            get
            {
                if (_instance == null)
                    _instance =new Singleton();
                return _instance;
            }
        } 

        ProjectClass Project = new ProjectClass("Application");
        public WebSocketClass WebSocketClient { set; get; }

        public Singleton()
        {
            WebSocketClient = new WebSocketClass(ref Project, Properties.Settings.Default.WSServer, "/");
            WebSocketClient.GotResponse += ParseMessage;
            WebSocketClient.Start();
        }  

        public void ParseMessage(object sender, ResponseInfo e)
        {
            switch (e.Command)
            {
                case "login": 
                    OnLogonSuccess?.Invoke(this, JsonConvert.DeserializeObject<LoginClass>(e.Data));

                    break;
                case "err":
                    OnError?.Invoke(this, e.Data);

                    break; 
                default:

                    break;
            }
        }

        public void SendLogin(Login login)
        { 
            WebSocketClient.Send(login);
        }
    } 
}
