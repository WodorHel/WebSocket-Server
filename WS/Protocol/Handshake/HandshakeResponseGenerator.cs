﻿using System;
using System.Linq;
using System.Text;

namespace WS.Protocol.Handshake
{
    internal class HandshakeResponseGenerator : IHandshakeResponseGenerator
    {
        HandshakeParser _handshakeParser;
        HandshakeKeyGenerator _handshakeKeyGenerator;

        const string WebSocketKeyName = "Sec-WebSocket-Key";
        const string EndSequence = "\r\n\r\n";

        public HandshakeResponseGenerator()
        {
            _handshakeParser = new HandshakeParser();
            _handshakeKeyGenerator = new HandshakeKeyGenerator();
        }

        public byte[] GetResponse(byte[] request)
        {
            var data = ASCIIEncoding.UTF8.GetString(request);

            if (!data.Contains(EndSequence))
                return new byte[0];

            var handshakeFields = _handshakeParser.ParseToDictionary(data);
            if (!handshakeFields.Any(p => p.Key == WebSocketKeyName))
                return new byte[0];

            var key = handshakeFields[WebSocketKeyName];
            var responseKey = _handshakeKeyGenerator.Get(key);

            var response = string.Format("HTTP/1.1 101 Switching Protocols\r\n" +
                                         "Upgrade: websocket\r\n" +
                                         "Connection: Upgrade\r\n" +
                                         "Sec-WebSocket-Accept: {0}" +
                                         "{1}",
                                          responseKey, EndSequence);

            return ASCIIEncoding.UTF8.GetBytes(response);
        }
    }
}
