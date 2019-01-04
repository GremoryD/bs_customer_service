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
    public enum ResponseType
    {
        Login,
        AnythingElse
    }

    public class Singleton
    {
        private ResponseType? _remainingData = null;

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
            //WebSocketClient.OnMessage += MessageRecievd;
            WebSocketClient.Start();
        }

        private void MessageRecievd(object sender, string e)
        {
            if (_remainingData == null)
                return;

            switch (_remainingData)
            {
                case ResponseType.Login:

                    break;
                case ResponseType.AnythingElse:

                    break;
            }

            _remainingData = null;
        }

        public void SendLogin(Login login)
        {
            _remainingData = ResponseType.Login;
            WebSocketClient.Send(login);
        }
    } 
}
