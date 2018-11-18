using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerLib
{
    public class JUtils
    {
        private static Dictionary<Type, JsonSchema> _shs = new Dictionary<Type, JsonSchema>();
        private static JsonSchema Schema(Type type)
        {
            if (_shs.ContainsKey(type))
            {
                return _shs[type];
            }
            else
            {
                JsonSchemaGenerator generator = new JsonSchemaGenerator();
                var sh = generator.Generate(type);
                _shs.Add(type, sh);

                return sh;
            }
        }
        static public JObject Deserialize(string json)
        {
            return JObject.Parse(json);
        }

        static public bool ValidType(JObject jObject, Type type)
        {
            return (Newtonsoft.Json.Schema.Extensions.IsValid(jObject, Schema(type)));
        }

        static public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
