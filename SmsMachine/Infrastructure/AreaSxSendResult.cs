namespace SmsMachine.Infrastructure
{
    public class AreaSxSendResult
    {
        public string Errno { get; set; }
        public string Errdesc { get; set; }
        public string Index { get; set; }

        public bool IsSuccess => Errno == "0";
        public int GetIndex() => int.Parse(Index);

    }
}
