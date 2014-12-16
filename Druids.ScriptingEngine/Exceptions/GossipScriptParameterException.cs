using System;

namespace Druids.ScriptingEngine.Exceptions
{
    public class GossipScriptParameterException : Exception
    {
        public GossipScriptParameterException()
        {
        }

        public GossipScriptParameterException(String message)
            : base(message)
        {
        }

        public GossipScriptParameterException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}