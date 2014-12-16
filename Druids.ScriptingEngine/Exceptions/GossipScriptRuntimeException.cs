using System;

namespace Druids.ScriptingEngine.Exceptions
{
    public class GossipScriptRuntimeException : Exception
    {
        public GossipScriptRuntimeException(String message)
            : base(message)
        {
        }

        public GossipScriptRuntimeException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}