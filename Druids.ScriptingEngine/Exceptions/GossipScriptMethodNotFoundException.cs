using System;

namespace Druids.ScriptingEngine.Exceptions
{
    public class GossipScriptMethodNotFoundException : Exception
    {
        public GossipScriptMethodNotFoundException()
        {
        }

        public GossipScriptMethodNotFoundException(String message)
            : base(message)
        {
        }

        public GossipScriptMethodNotFoundException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}