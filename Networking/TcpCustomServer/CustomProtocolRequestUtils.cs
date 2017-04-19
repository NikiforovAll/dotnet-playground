using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static TcpCustomServer.CustomProtocol;
namespace TcpCustomServer
{
    public static class CustomProtocolRequestUtils
    {
        public static ParseResponse ParseRequest(this string request, SessionManager sessionManager, out string sessionId, out string response)
        {
            sessionId = string.Empty;
            response = string.Empty;
            string[] protocolRequest = request.Split(new string[] { SEPARATOR }, StringSplitOptions.RemoveEmptyEntries);
            if (protocolRequest[0] == COMMANDHELO)
            {
                sessionId = sessionManager.CreateSession();

            }
            else if (protocolRequest[0] == SESSIONID)
            {
                sessionId = protocolRequest[1];

                if (!sessionManager.TouchSession(sessionId))
                {
                    return ParseResponse.TIMEOUT;
                }

                if (protocolRequest[2] == COMMANDBYE)
                {
                    return ParseResponse.CLOSE;
                }
                if (protocolRequest.Length >= 4)
                {
                    response = CustomProtocolRequestUtils.ProcessRequest(protocolRequest, sessionManager: sessionManager);
                }
            }
            else
            {
                return ParseResponse.ERROR;
            }
            return ParseResponse.OK;
        }


        public static string Reverse(string action) => string.Join("", string.Concat(Enumerable.Reverse(action)));
        public static string Echo(string action) => action;

        private static string ProcessRequest(string[] requset, SessionManager sessionManager)
        {
            if (requset.Length < 4) throw new ArgumentException("invalid length");

            string sessionId = requset[1];
            string response = string.Empty;
            string requestCommand = requset[2];
            string requestAction = requset[3];


            switch (requestCommand)
            {
                case COMMANDECHO:
                    response = Echo(requestAction);
                    break;
                case COMMANDREV:
                    response = Reverse(requestAction);
                    break;
                case COMMANDSET:
                    response = sessionManager.ParseSessionData(sessionId, requestAction);
                    break;
                case COMMANDGET:
                    response = $"{sessionManager.GetSessionData(sessionId, requestAction)}";
                    break;
                default:
                    response = STATUSUNKNOWN;
                    break;
            }
            return response;
        }
    }
}
