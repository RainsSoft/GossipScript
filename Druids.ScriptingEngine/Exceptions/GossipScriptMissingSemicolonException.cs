using System;
using Druids.ScriptingEngine.Compiler;

namespace Druids.ScriptingEngine.Exceptions
{
    public class GossipScriptMissingSemiColonException : Exception
    {
        private readonly Token m_Token;

        public GossipScriptMissingSemiColonException(String message)
            : base(message)
        {
        }

        public GossipScriptMissingSemiColonException(string message, Exception exception)
            : base(message, exception)
        {
        }

        public GossipScriptMissingSemiColonException(String message, Token token)
            : base(message)
        {
            m_Token = token;
        }

        public Token Token
        {
            get { return m_Token; }
        }

        public override string ToString()
        {
            if (m_Token == null)
            {
                return Message;
            }
            return String.Format("{0} {1}", Message, Token);
        }
    }
}