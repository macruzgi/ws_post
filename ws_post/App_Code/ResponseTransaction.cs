public class ResponseTransaction
{
    public string ResponseCode { get; internal set; }
    public string AuthorizationCode { get; internal set; }
    public string RRNN { get; internal set; }
    public string Invoice { get; internal set; }
    public string STAN { get; internal set; }
    public string ResponseText { get; internal set; }
    public string TerminalId { get; internal set; }
    public string EmisorName { get; internal set; }
    public string MerchantId { get; internal set; }
}
public class RespuestaJson
{
    public string _ResponseCode { get; internal set; }
    public string _AuthorizationCode { get; internal set; }
    public string _RRNN { get; internal set; }
    public string _Invoice { get; internal set; }
    public string _STAN { get; internal set; }
    public string _ResponseText { get; internal set; }
    public string _TerminalId { get; internal set; }
    public string _EmisorName { get; internal set; }
    public string _MerchantId { get; internal set; }
}