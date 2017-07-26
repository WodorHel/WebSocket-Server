namespace WS.Protocol.Frame
{
    internal enum DeserializeResult
    {
        None,
        InvalidHeader,
        PartialMessage,
        SuccessWithFIN,
        SuccessWithoutFIN
    }
}
