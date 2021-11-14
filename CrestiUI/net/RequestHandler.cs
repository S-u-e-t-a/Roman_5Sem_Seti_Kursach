using System;
using System.Collections.Generic;
using System.Linq;

namespace CrestiUI
{
    internal static class RequestHandler
    {
        private static readonly Dictionary<Type, string> typeWithTypeConverter = new()
        {
            {typeof(bool), nameof(Convert.ToBoolean)},
            {typeof(byte), nameof(Convert.ToByte)},
            {typeof(sbyte), nameof(Convert.ToSByte)},
            {typeof(int), nameof(Convert.ToInt32)},
            {typeof(short), nameof(Convert.ToInt16)},
            {typeof(long), nameof(Convert.ToInt64)},
            {typeof(char), nameof(Convert.ToChar)},
            {typeof(string), nameof(Convert.ToString)},
            {typeof(double), nameof(Convert.ToDouble)}
        };


        public static object ExecuteRequest(object executor, Request request)
        {
            var m = executor.GetType().GetMethod(request.FuncName);

            var parameters = m.GetParameters().ToList();
            var args = new object[parameters.Count];
            foreach (var parameter in parameters) // для каждого параметра функции
            {
                var convertMethod = typeof(Convert).GetMethod(typeWithTypeConverter[parameter.ParameterType], new[] {typeof(string)}); // метод ковертации
                var convertMethod1 = typeof(Convert).GetMethod(nameof(Convert.ToInt32), new[] {typeof(string)});
                args[parameters.IndexOf(parameter)] = convertMethod.Invoke(typeof(Convert), new object?[] {request.Args[parameter.Name]});
            }

            Console.WriteLine(args);

            return m.Invoke(executor, args);
        }
    }
}