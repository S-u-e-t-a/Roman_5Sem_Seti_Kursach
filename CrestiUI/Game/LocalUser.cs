namespace CrestiUI.Game
{
    public class LocalUser
    {
        public string Name { get; }
        public string Ip { get; }


        public LocalUser(string name, string ip)
        {
            Name = name;
            Ip = ip;
        }


        public LocalUser(string name)
        {
            Name = name;
        }
    }
}