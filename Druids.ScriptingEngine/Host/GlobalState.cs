using System;

namespace Druids.ScriptingEngine.Host
{
    public class GlobalState
    {
        // TODO Should this be a stack or dictionary?

        // TODO Should max memory be configurable

        public readonly Boolean[] Flags = new Boolean[1024];
        public readonly String[] Str = new string[1024];
        public readonly Double[] Var = new Double[1024];
    }
}