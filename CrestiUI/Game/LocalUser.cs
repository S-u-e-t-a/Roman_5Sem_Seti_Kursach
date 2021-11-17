namespace CrestiUI.Game
{
    public class LocalUser
    {
        public string Name { get; set; }
        public string Ip { get; set; }


        public LocalUser()
        {
        }


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