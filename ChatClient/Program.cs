using CRESTI;

namespace ChatClient
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var g = new Board();
            var c = new Client(g);
        }
    }
}