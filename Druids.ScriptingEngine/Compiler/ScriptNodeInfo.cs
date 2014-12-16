using System.Collections.Generic;

namespace Druids.ScriptingEngine.Compiler
{
    public class ScriptNodeInfo
    {
        public ScriptNodeInfo()
        {
            CustomEventScripts = new List<ScriptNodeEventInfo>();
        }

        public ScriptNodeEventInfo OnUpdateScript { get; set; }

        public ScriptNodeEventInfo OnEnterScript { get; set; }

        public ScriptNodeEventInfo OnExitScript { get; set; }

        public ScriptNodeEventInfo OnInterruptScript { get; set; }

        public List<ScriptNodeEventInfo> CustomEventScripts { get; set; }
    }
}