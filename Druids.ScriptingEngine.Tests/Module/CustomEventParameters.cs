using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine.Attributes;

namespace JRPG.ScriptingEngine.Tests.Module
{
    [GossipScriptType]
    public class CustomEventParameters
    {
        [GossipScriptProperty("Name")]
        public String Name { get; set; }
    }
}
