using CRESTI;

namespace ChatServer
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var g = new Board();
            var s = new Server(g);
        }
    }
}