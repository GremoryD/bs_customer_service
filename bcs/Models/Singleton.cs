using BCS_User_Interface;
using CLProject;
using ServerLib.JTypes.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bcs.Models
{ 

    public class Singleton
    { 

        public String SessionID; 
        private static Singleton _instance;
        public static Singleton instance
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
            WebSocketClient.Start();
        } 

        public void SendLogin(Login login)
        { 
            WebSocketClient.Send(login);
        }
    } 
}
