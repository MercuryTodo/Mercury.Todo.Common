namespace Common.Security
{
    public class JwtSession : JwtBasic
    {
        public string SessionId { get; set; }
        public string Key { get; set; }
    }
}