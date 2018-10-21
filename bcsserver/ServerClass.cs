using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLProject;
using System.Threading;
using WebSocketSharp;
using WebSocketSharp.Server;
using System.Collections.Concurrent;
using Newtonsoft.Json;

namespace bcsserver
{
    class ServerClass
    {
        private ProjectClass Project;

        private readonly MainWindow MainForm;

        private WebSocketServer wsServer;


        /// <summary>
        /// Дата и время запуска сервера
        /// </summary>
        public DateTime StartTime;

        /// <summary>
        /// Количество активных сессий
        /// </summary>
        public int SessionCount
        {
            get
            {
                return wsServer == null ? 0 : wsServer.WebSocketServices.SessionCount;
            }
        }

        /// <summary>
        /// Признак запущен  ли сервер
        /// </summary>
        public bool IsRunning = false;

        //private HttpServer httpsServer;

        public ServerClass(MainWindow AMainForm, ref ProjectClass AProject)
        {
            MainForm = AMainForm;
            Project = AProject;
            StartTime = DateTime.MinValue;
        }

        private HandlerMain HandlerMainCreator()
        {
            return new HandlerMain(ref Project);
        }

        public void Start()
        {
            wsServer = new WebSocketServer(System.Net.IPAddress.Any, Properties.Settings.Default.port);

            //System.Security.Cryptography.X509Certificates.X509Certificate2 cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\Temp\sslforfree-kalosha.net\kalosha.net\2018-09-14\certificate.crt");
            //wsServer.SslConfiguration.ServerCertificate = cert;

            wsServer.AddWebSocketService("/", HandlerMainCreator);

            wsServer.Start();

            /*
            httpsServer = new HttpServer(System.Net.IPAddress.Any, 4649, true);
            System.Security.Cryptography.X509Certificates.X509Certificate2 cert = new System.Security.Cryptography.X509Certificates.X509Certificate2(@"D:\Temp\sslforfree-kalosha.net\kalosha.net\2018-09-14\certificate.crt");

            if (cert.Verify())
            {
                Project.Log.Trace(string.Format("SSL-сетификат Версия: {0} Серийный номер: {1} Действителен до: {2}", cert.Version, cert.SerialNumber, cert.GetExpirationDateString()));
            }
            else
            {
                Project.Log.Trace("Ошибка проверки сертификата");
            }

            httpsServer.SslConfiguration.ServerCertificate = cert;

            httpsServer.OnGet += (sender, e) =>
            {
                e.Response.WriteContent(Encoding.ASCII.GetBytes("OK-OK"));
            };
            httpsServer.Start();
            */

            StartTime = DateTime.Now;
            IsRunning = true;
        }

        public void Stop()
        {
            wsServer.Stop();
            IsRunning = false;
            StartTime = DateTime.MinValue;
        }
    }

    class HandlerMain : WebSocketBehavior
    {

        private static ConcurrentDictionary<string, UserClass> Users = new ConcurrentDictionary<string, UserClass>();
        private ProjectClass Project;


        public HandlerMain(ref ProjectClass AProject)
        {
            Project = AProject;
        }

        public void SendString(string AString)
        {
            Send(AString);
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            Users.TryAdd(ID, new UserClass(ID, this, ref Project));
        }

        protected override void OnClose(CloseEventArgs e)
        {
            if (Users.TryGetValue(ID, out UserClass User))
            {
                User.Stop();
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                if (e.IsText)
                {
                    if (Users.TryGetValue(ID, out UserClass User))
                    {
                        string Response = User.Receive(e.Data);
                        if (Response.Length > 0)
                        {
                            Send(Response);
                        }
                    }
                    else
                    {
                        Send(JsonConvert.SerializeObject(new
                        {
                            state = "error",
                            code = 1,
                            description = "Session not found"
                        }));
                    }
                }
                else
                {
                    throw new Exception("Received non-text message");
                }
            }
            catch (Exception ex)
            {
                Send(JsonConvert.SerializeObject(new
                {
                    state = "error",
                    code = 2,
                    description = ex.Message
                }));
            }
        }
    }

    class UserClass
    {
        private static HandlerMain Handler;
        private static ProjectClass Project;
        private static ConcurrentQueue<string> Requests = new ConcurrentQueue<string>();
        private static ConcurrentQueue<string> Responses = new ConcurrentQueue<string>();
        private static bool IsClosing = false;

        private string SessionID { get; set; }
        private static bool IsAuthenticated { get; set; }

        private Thread Receiver = new Thread(ReceiverThread);
        private Thread Monitoring = new Thread(MonitoringThread);
        private Thread Sender = new Thread(SenderThread);

        private static UserInfoClass UserInfo = new UserInfoClass();

        private static readonly object ErrorNotAuthenticated = new
        {
            state = "error",
            code = 3,
            description = "User is not authenticated"
        };

        private static readonly object ErrorNotJSONObject = new
        {
            state = "error",
            code = 4,
            description = "Not a json object"
        };

        private static readonly object ErrorUnknownCommand = new
        {
            state = "error",
            code = 5,
            description = "Unknown command"
        };

        public UserClass(string ASessionID, HandlerMain AHandler, ref ProjectClass AProject)
        {
            SessionID = ASessionID;
            Handler = AHandler;
            Project = AProject;
            IsAuthenticated = false;
            Receiver.Start();
            Monitoring.Start();
            Sender.Start();
        }

        private static void ReceiverThread()
        {
            while (!IsClosing)
            {
                if (Requests.TryDequeue(out string Request))
                {
                    // Обработка запросов
                    try
                    {
                        Handler.SendString(string.Format("Длина очереди: {0} Текст запроса: {1}", Responses.Count, Request));

                        switch (JsonConvert.DeserializeAnonymousType(Request, new { command = "" }).command.ToLower())
                        {
                            case "login":
                                DatabaseParameterValuesClass Param = new DatabaseParameterValuesClass();
                                Param.CreateParameterValue("Request", Request);
                                Param.CreateParameterValue("Response");
                                Project.Database.Execute("Login", ref Param);
                                string Response = Param.ParameterByName("Response").Value.ToString();
                                UserInfoClass UserInfoNew = JsonConvert.DeserializeObject<UserInfoClass>(Response);
                                if(UserInfoNew.state == "ok")
                                {
                                    UserInfo.Login = UserInfoNew.Login;
                                    UserInfo.FirstName = UserInfoNew.FirstName;
                                    UserInfo.MidleName = UserInfoNew.MidleName;
                                    UserInfo.LastName = UserInfoNew.LastName;
                                    UserInfo.LastLogin = UserInfoNew.LastLogin;
                                    UserInfo.Commit();
                                    IsAuthenticated = true;
                                }
                                else
                                {
                                    Responses.Enqueue(Response);
                                    IsAuthenticated = false;
                                }
                                break;
                            default:
                                Responses.Enqueue(JsonConvert.SerializeObject(ErrorUnknownCommand));
                                break;
                        }

                    }
                    catch(Exception ex)
                    {
                        Responses.Enqueue(ex.Message);
                        Responses.Enqueue(JsonConvert.SerializeObject(ErrorNotJSONObject));
                    }
                }
                Thread.Sleep(1);
            }
        }

        private static void MonitoringThread()
        {
            while (!IsClosing)
            {
                if(UserInfo.NeedSend())
                {
                    Responses.Enqueue(UserInfo.GetSending());
                }
                Thread.Sleep(1);
            }
        }

        private static void SenderThread()
        {
            while (!IsClosing)
            {
                if (Responses.TryDequeue(out string Response))
                {
                    Handler.SendString(Response);
                }
                Thread.Sleep(1);
            }
        }

        public void Stop()
        {
            IsClosing = true;
        }

        public string Receive(string AData)
        {
            string Response = string.Empty;
            bool IsLogin = false;

            try
            {
                IsLogin = JsonConvert.DeserializeAnonymousType(AData, new { command = "" }).command.ToLower() == "login";
            }
            catch { }

            if (IsAuthenticated || (!IsAuthenticated && IsLogin))
            {
                Requests.Enqueue(AData);
            }
            else
            {
                Response = JsonConvert.SerializeObject(ErrorNotAuthenticated);
            }
            return (Response);
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    class UserParnetClass
    {
        private bool IsSending = false;
        private bool IsCommit = false;

        [JsonProperty("state")]
        public string state = "ok";

        public virtual string GetSending()
        {
            if (!IsSending)
            {
                IsSending = true;
                return JsonConvert.SerializeObject(this);
            }
            else
            {
                return string.Empty;
            }
        }

        public virtual void Updating()
        {
            IsSending = false;
            IsCommit = false;
        }

        public void Commit()
        {
            IsCommit = true;
        }

        public bool NeedSend()
        {
            return IsCommit && !IsSending;
        }
    }

    [JsonObject(MemberSerialization.OptIn)]
    class UserInfoClass : UserParnetClass
    {
        [JsonProperty("command")]
        public string Command = "login";

        [JsonProperty("login")]
        private string login;
        public string Login
        {
            get
            {
                return login;
            }
            set
            {
                if (login != value)
                {
                    login = value;
                    Updating();
                }
            }
        }

        [JsonProperty("firstname")]
        private string firstname;
        public string FirstName
        {
            get
            {
                return firstname;
            }
            set
            {
                if (firstname != value)
                {
                    firstname = value;
                    Updating();
                }
            }
        }

        [JsonProperty("midlename")]
        private string midlename;
        public string MidleName
        {
            get
            {
                return midlename;
            }
            set
            {
                if (midlename != value)
                {
                    midlename = value;
                    Updating();
                }
            }
        }

        [JsonProperty("lastname")]
        private string lastname;
        public string LastName
        {
            get
            {
                return lastname;
            }
            set
            {
                if (lastname != value)
                {
                    lastname = value;
                    Updating();
                }
            }
        }

        [JsonProperty("lastlogin")]
        private string lastlogin;
        public string LastLogin
        {
            get
            {
                return lastlogin;
            }
            set
            {
                if (lastlogin != value)
                {
                    lastlogin = value;
                    Updating();
                }
            }
        }
    }
}
