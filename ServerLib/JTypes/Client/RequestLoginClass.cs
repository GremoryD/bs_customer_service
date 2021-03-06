﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Client
{
    /// <summary>
    /// Запрос аутентификации пользователя в системе
    /// </summary>
    public class RequestLoginClass
    {
        /// <summary>
        /// Команда запроса
        /// </summary>
        [JsonProperty("command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.Commands Command { get; set; } = Enums.Commands.login;

        /// <summary>
        /// Логин
        /// </summary>
        [JsonProperty("login", Required = Required.Always)]
        public string UserName { get; set; } = null;

        /// <summary>
        /// Пароль
        /// </summary>
        [JsonProperty("password", Required = Required.Always)]
        public string Password { get; set; } = null;
    }
}
