using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CrestiUI.net
{
    class Response
    {
        public Dictionary<string,string> ResponseArgs { get; private set; }


        public Response(Dictionary<string, string> args) // todo исправить констурктор, так как нарушаются права доступа
        {
            ResponseArgs = args;
        }


        public Response(byte[] data)
        {
            var json = JsonSerializer.Deserialize<Request>(data);
            this.ResponseArgs = json.Args;
        }


        public Response(string jsonString)
        {
            var json = JsonSerializer.Deserialize<Request>(jsonString);
            this.ResponseArgs = json.Args;
        }

        public byte[] ToJsonBytes()
        {
            string json = JsonSerializer.Serialize<Response>(this);

            return Encoding.UTF8.GetBytes(json);
        }


        public string ToJsonString()
        {
            return JsonSerializer.Serialize<Response>(this);
        }
    }
}
