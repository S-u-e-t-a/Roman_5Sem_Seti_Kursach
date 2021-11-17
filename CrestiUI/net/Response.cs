using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CrestiUI.net
{
    internal class Response
    {
        public Dictionary<string, string> ResponseArgs { get; set; }


        public Response()
        {
        }


        public Response(Dictionary<string, string> args) // todo исправить констурктор, так как нарушаются права доступа
        {
            ResponseArgs = args;
        }


        public Response(byte[] data)
        {
            var json = JsonSerializer.Deserialize<Request>(data);
            ResponseArgs = json.Args;
        }


        public Response(string jsonString)
        {
            var json = JsonSerializer.Deserialize<Response>(jsonString);
            ResponseArgs = json.ResponseArgs;
        }


        public byte[] ToJsonBytes()
        {
            var json = JsonSerializer.Serialize(this);

            return Encoding.UTF8.GetBytes(json);
        }


        public string ToJsonString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}