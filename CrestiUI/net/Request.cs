using System;
using System.Collections.Generic;

namespace CrestiUI
{
    internal class Request
    {
        public Request(string request)
        {
            var s = request.Split('?');
            if (s.Length != 2)
            {
                throw new ArgumentException();
            }

            FuncName = s[0];
            s = s[1].Split('&', StringSplitOptions.RemoveEmptyEntries);
            Args = new Dictionary<string, string>();
            for (var i = 0; i < s.Length; i++)
            {
                if (s[i] != string.Empty)
                {
                    var command = s[i].Split('=', StringSplitOptions.RemoveEmptyEntries);
                    if (command.Length == 2)
                    {
                        //Args.Add(command[0], command[1]);
                        Args[command[0]] = command[1];
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }


        public string FuncName { get; }


        public Dictionary<string, string> Args { get; set; }
    }
}