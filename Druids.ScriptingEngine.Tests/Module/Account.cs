using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine.Attributes;

namespace JRPG.ScriptingEngine.Tests.Module
{
    [GossipScriptType]
    public class Account
    {
        [GossipScriptProperty("Id", Usage = PropertyUsage.ReadOnly)]
        public Int32 Id { get; set; }

        [GossipScriptProperty("Credit")]
        public Double Credit { get; set; }

        [GossipScriptProperty("Name")]
        public String Name { get; set; }

        public String IgnoreMe { get; set; }

        [GossipScriptMethod("Method1")]
        public Double MethodA()
        {
            return 2;
        }

        [GossipScriptMethod("Method2")]
        public Double MethodB(Int32 p1)
        {
            return 2+p1;
        }

        [GossipScriptMethod("Method3")]
        public Double MethodC(Int32 p1, Int32 p2)
        {
            return p1-p2;
        }

        [GossipScriptAction("CustomAction", typeof(CustomActionNode))]
        public static void CustomAction()
        {
            // Must be void
        }

        [GossipScriptAction("CustomAction2", typeof(CustomActionNode2))]
        public static void CustomAction(String parameter)
        {
            // Must be void
        }

        [GossipScriptAction("CustomAction3", typeof(CustomActionNode3))]
        public static void CustomAction(String parameter, Int32 parameter2)
        {
            // Must be void
        }
    }
}
