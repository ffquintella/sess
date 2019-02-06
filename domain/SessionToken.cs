namespace domain
{
    public class SessionToken
    {
        public string Hash { get; set; }
        public int Ttl { get; set; }
    }
}