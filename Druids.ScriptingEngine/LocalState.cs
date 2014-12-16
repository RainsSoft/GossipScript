using System;

namespace Druids.ScriptingEngine
{
    public class LocalState
    {
        // Do wonder if this should be a stack to save memory?

        public readonly Boolean[] Flags = new Boolean[64];
        public readonly object[] Ref = new object[64];
        public readonly String[] Str = new string[64];
        public readonly Double[] Var = new Double[64];

        public void Reset()
        {
            for (int i = 0; i < Flags.Length; i++)
            {
                Flags[i] = false;
            }

            for (int i = 0; i < Var.Length; i++)
            {
                Var[i] = 0;
            }

            for (int i = 0; i < Str.Length; i++)
            {
                Str[i] = "";
            }

            for (int i = 0; i < Ref.Length; i++)
            {
                Ref[i] = null;
            }
        }
    }
}