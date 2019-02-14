using Newtonsoft.Json;
using CLProject;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Должность в составе списка
    /// </summary>
    public class ResponseJobClass : ResponseBaseItemClass
    {
        /// <summary>
        /// Наименование должности
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + Name);



        public JobBuilder Builder { get; private set; }

        public ResponseJobClass()
        { 
            Builder = new JobBuilder(this);
        }

    }

    public class JobBuilder
    {
        private ResponseJobClass _relatedJob;

        private string name = null;  

        public JobBuilder(ResponseJobClass job)
        {
            _relatedJob = job;
        }

        public void Update()
        {
            if (name != null)
                _relatedJob.Name = name; 
            name = null;
        }

        public JobBuilder From(ResponseJobClass job)
        {
            WithName(job.Name); 

            return this;
        }


        public JobBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }
         
    }
}
