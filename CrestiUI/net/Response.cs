using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CrestiUI.net
{
    internal class Response
    {
        public string Name { get; set; }
        public Dictionary<string, string> Args { get; set; }


        public Response()
        {
        }


        public Response(string name, Dictionary<string, string> args) // todo исправить констурктор, так как нарушаются права доступа
        {
            Args = args;
            Name = name;
        }


        public Response(byte[] data)
        {
            var json = JsonSerializer.Deserialize<Response>(data);
            Name = json.Name;
            Args = json.Args;
        }


        public Response(string jsonString)
        {
            var json = JsonSerializer.Deserialize<Response>(jsonString);
            Name = json.Name;
            Args = json.Args;
        }


        public byte[] ToJsonBytes()
        {
            var json = ToJsonString();

            return Encoding.UTF8.GetBytes(json);
        }


        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}