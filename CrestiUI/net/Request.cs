using System.Collections.Generic;
using System.Text;
using System.Text.Json;

using CrestiUI.net;

namespace CrestiUI
{
    public class Request
    {
        public string RequestType { get; set; } //POST отправляет запрос на сервер, GET - получает данные с сервера
        public string FuncName { get; set; }

        public Dictionary<string, string> Args { get; set; }


        public Request()
        {
        }


        public Request(string requestType, RequestCommands command, Dictionary<string, string> args)
        {
            RequestType = requestType;
            FuncName = command.ToString();
            Args = args;
        }


        public Request(byte[] data)
        {
            var json = JsonSerializer.Deserialize<Request>(data);
            RequestType = json.RequestType;
            FuncName = json.FuncName;
            Args = json.Args;
        }


        public Request(string jsonString)
        {
            jsonString.Replace("\u0013", string.Empty);
            var json = JsonSerializer.Deserialize<Request>(jsonString);
            RequestType = json.RequestType;
            FuncName = json.FuncName;
            Args = json.Args;
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