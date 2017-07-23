namespace WS.Protocol.Frame
{
    enum DecryptResult
    {
        None,
        InvalidHeader,
        PartialMessage,
        SuccessWithFIN,
        SuccessWithoutFIN
    }
}
