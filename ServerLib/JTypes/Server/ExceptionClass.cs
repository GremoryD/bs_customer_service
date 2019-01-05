﻿using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ServerLib.JTypes.Server
{
    /// <summary>
    /// Класс исключений
    /// </summary>
    public class ExceptionClass
    {
        /// <summary>
        /// Команда, в обработке которой произошла ошибка
        /// </summary>
        [JsonProperty("command", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.Commands Command { get; set; } = Enums.Commands.none;

        /// <summary>
        /// Поле статуса всегда содержащее значение error
        /// </summary>
        [JsonProperty(PropertyName = "state", Required = Required.Always)]
        public string State { get; set; } = "error";

        /// <summary>
        /// Код ошибки
        /// </summary>
        [JsonProperty(PropertyName = "code", Required = Required.Always), JsonConverter(typeof(StringEnumConverter))]
        public Enums.ErrorCodes Code { get; set; } = Enums.ErrorCodes.NoError;

        /// <summary>
        /// Описание ошибки
        /// </summary>
        [JsonProperty(PropertyName = "description", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; } = null;

        /// <summary>
        /// Создание исключения
        /// </summary>
        /// <param name="ACommand">Команда, в обработке которой произошла ошибка</param>
        /// <param name="ACode">Код ошибки</param>
        /// <param name="ADescription">Описание ошибки</param>
        public ExceptionClass(Enums.Commands ACommand, Enums.ErrorCodes ACode, string ADescription = null)
        {
            Command = ACommand;
            Code = ACode;
            Description = ADescription;
        }
    }
}