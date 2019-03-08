using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using CLProject;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Пользователь в составе списка
    /// </summary>
    public class ResponseUserClass : ResponseBaseItemClass
    {
        /// <summary>
        /// Логин
        /// </summary>
        [JsonProperty(PropertyName = "login", Required = Required.Always)]
        public string Login { get; set; } = null;

        /// <summary>
        /// Имя
        /// </summary>
        [JsonProperty(PropertyName = "first_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string FirstName { get; set; } = null;

        /// <summary>
        /// Фамилия
        /// </summary>
        [JsonProperty(PropertyName = "last_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string LastName { get; set; } = null;

        /// <summary>
        /// Отчество
        /// </summary>
        [JsonProperty(PropertyName = "midle_name", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string MidleName { get; set; } = null;

        /// <summary>
        /// Идентификатор должности
        /// </summary>
        [JsonProperty(PropertyName = "job_id", Required = Required.Always)]
        public long JobID { get; set; }

        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "job_name", Required = Required.Always)]
        public string JobName { get; set; } = null;

        /// <summary>
        /// Признак активности
        /// </summary>
        [JsonProperty(PropertyName = "active", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.UserActive Active { get; set; }


        public bool ActiveBool { get => this.Active == Enums.UserActive.activated; }

        public UserBuilder Builder { get; private set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + Login + FirstName + LastName + MidleName + JobName + Active.ToString());

        public ResponseUserClass()
        {
            Builder = new UserBuilder(this);
        }
    }

    public class UserBuilder
    {
        private ResponseUserClass _relatedUser;

        private string name = null;
        private string secname = null;
        private string midname = null;
        private long? jobId = null;
        private string jobName = null;
        private Enums.UserActive? active = null;
        
        public UserBuilder(ResponseUserClass user)
        {
            _relatedUser = user;
        }

        public void Update()
        {
            if (name != null)
                _relatedUser.FirstName = name;

            if (secname != null)
                _relatedUser.LastName = secname;

            if (midname != null)
                _relatedUser.MidleName = midname;

            if (jobId != null)
                _relatedUser.JobID = jobId.Value;

            if (jobName != null)
                _relatedUser.JobName = jobName;

            if (active != null)
                _relatedUser.Active = active.Value;

            name = null;
            secname = null;
            midname = null;
            jobId = null;
            jobName = null;
            active = null;
        }

        public UserBuilder From(ResponseUserClass user)
        {
            WithName(user.FirstName);
            WithSecondName(user.LastName);
            WithMiddleName(user.MidleName);
            WithJobId(user.JobID);
            WithJobName(user.JobName);
            WithActive(user.Active);

            return this;
        }

        public UserBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public UserBuilder WithSecondName(string secondName)
        {
            this.secname = secondName;
            return this;
        }

        public UserBuilder WithMiddleName(string middleName)
        {
            this.midname = middleName;
            return this;
        }

        public UserBuilder WithJobId(long jobId)
        {
            this.jobId = jobId;
            return this;
        }

        public UserBuilder WithJobName(string jobName)
        {
            this.jobName = jobName;
            return this;
        }

        public UserBuilder WithActive(Enums.UserActive active)
        {
            this.active = active;
            return this;
        }
    }
}
