﻿using WS.SocketClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WS.WebSocketClients
{
    class WebSocketClient
    {
        public String ID { get; private set; }
        public bool HandshakeDone { get; set; }

        byte[] buffer;
        int bufferPointer;

        public WebSocketClient(String id, int bufferSize)
        {
            this.ID = id;
            this.buffer = new byte[bufferSize];

            bufferPointer = 0;
        }

        public bool AddToBuffer(byte[] data)
        {
            if (bufferPointer + data.Length > buffer.Length)
                return false;

            Array.Copy(data, 0, buffer, bufferPointer, data.Length);
            bufferPointer += data.Length;
            return true;
        }

        public void RemoveFromBuffer(int length)
        {
            Array.Copy(buffer, length, buffer, 0, buffer.Length - length);
            bufferPointer -= length;
        }

        public void ClearBuffer()
        {
            Array.Clear(buffer, 0, buffer.Length);
            bufferPointer = 0;
        }

        public byte[] GetBufferData()
        {
            return buffer.Take(bufferPointer).ToArray();
        }
    }
}