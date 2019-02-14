using Newtonsoft.Json;
using CLProject;

namespace ServerLib.JTypes.Server
{
    public class ResponseRoleClass : ResponseBaseItemClass
    {
        /// <summary>
        /// Наименование роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; } = null;

        /// <summary>
        /// Описание роли пользователей
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        /// <summary>
        /// Hash-код объекта
        /// </summary>
        [JsonIgnore]
        public string Hash => Utils.SHA256Base64(ID.ToString() + Name + Description);


        public RolBuilder Builder { get; private set; }

        public ResponseRoleClass()
        {
            Builder = new RolBuilder(this);
        }
    }


    public class RolBuilder
    {
        private ResponseRoleClass _relatedRol;

        private string name = null;
        private string description = null;

        public RolBuilder(ResponseRoleClass rol)
        {
            _relatedRol = rol;
        }

        public void Update()
        {
            if (name != null)
                _relatedRol.Name = name;
            if (description != null)
                _relatedRol.Description = description;
            name = null;
            description = null;
        }

        public RolBuilder From(ResponseRoleClass rol)
        {
            WithName(rol.Name);
            WithDescription(rol.Description);

            return this;
        }


        public RolBuilder WithName(string name)
        {
            this.name = name;
            return this;
        }

        public RolBuilder WithDescription(string description)
        {
            this.description = description;
            return this;
        }

    }
}
