using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine.Attributes;

namespace JRPG.ScriptingEngine.Tests.Module
{
    [GossipScriptType]
    public class Customer
    {
        [GossipScriptProperty("Id", Usage = PropertyUsage.ReadOnly)]
        public Int32 Id { get; set; }

        [GossipScriptProperty("Name")]
        public String Name { get; set; }

        [GossipScriptProperty("Age")]
        public Int32 Age { get; set; }

        [GossipScriptProperty("Account")]
        public Account Account { get; set; }

        public Customer()
        {
            Account = new Account();
        }

        [GossipScriptMethod("TestMethod001")]
        public Int32 TestMethod001()
        {
            return 1;
        }

        public void TestMethod002()
        {
            // this method should not be registrable
        }

        [GossipScriptMethod("TestMethod003")]
        public Int32 TestMethod003(Int32 p)
        {
            return p*2;
        }

        [GossipScriptMethod("TestMethod004")]
        public Int32 TestMethod004(Int32 p1, Int32 p2)
        {
            return p1 * p2;
        }

        [GossipScriptMethod("Concat")]
        public String Concat(String a, String b, String c)
        {
            return a + b + c;
        }

        [GossipScriptMethod("Mixed")]
        public String Mixed(Double a, String b, Int32 c)
        {
            return a + b + c;
        }

        [GossipScriptAction("CustomAction", typeof(CustomActionNode))]
        public static void CustomAction()
        {
            // Must be void
        }

        [GossipScriptAction("CustomAction2", typeof(CustomActionNode2))]
        public static void CustomAction(String parameter)
        {
           // Muse be void
        }

        [GossipScriptAction("CustomAction3", typeof(CustomActionNode3))]
        public static void CustomAction(String parameter, Int32 parameter2)
        {
            // Muse be void
        }
    }
}
