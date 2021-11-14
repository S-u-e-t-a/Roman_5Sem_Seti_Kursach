namespace CrestiUI
{
    public class ClientServerMessageEventArgs
    {
        public ClientServerMessageEventArgs(string mes)
        {
            Message = mes;
        }


        public string Message { get; }
    }
}