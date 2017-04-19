using System;
using System.Collections.Generic;
using System.Text;

namespace SocketServer
{
    interface ISocketServer
    {
        void Listener(int port);
    }
}
