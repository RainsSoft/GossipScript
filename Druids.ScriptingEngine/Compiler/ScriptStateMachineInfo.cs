using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Druids.ScriptingEngine.Compiler
{
    public class ScriptStateMachineInfo
    {
        public List<ScriptStateNodeRecord> Nodes { get; set; } 
        public ScriptStateMachineInfo()
        {
            Nodes = new List<ScriptStateNodeRecord>();
        }
    }
}
