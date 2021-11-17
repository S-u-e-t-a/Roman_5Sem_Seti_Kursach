using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace CrestiUI
{
    public class Request
    {
        public string RequestType { get; private set; } //POST отправляет запрос на сервер, GET - получает данные с сервера
        public string FuncName { get; private set; }

        public Dictionary<string, string> Args { get; private set; }
        public Request(string requestType,string funcName, Dictionary<string,string> args)
        {
            RequestType = requestType;
            FuncName = funcName;
            Args = args;
        }

        public Request(byte[] data)
        {
            var json = JsonSerializer.Deserialize<Request>(data);
            this.RequestType = json.RequestType;
            this.FuncName = json.FuncName;
            this.Args = json.Args;
        }

        public Request(string jsonString)
        {
            var json = JsonSerializer.Deserialize<Request>(jsonString);
            this.RequestType = json.RequestType;
            this.FuncName = json.FuncName;
            this.Args = json.Args;
        }
        public byte[] ToJsonBytes()
        {
            string json = JsonSerializer.Serialize<Request>(this);

            return Encoding.UTF8.GetBytes(json);
        }


        public string ToJsonString()
        {
            return JsonSerializer.Serialize<Request>(this);
        }
    }
}