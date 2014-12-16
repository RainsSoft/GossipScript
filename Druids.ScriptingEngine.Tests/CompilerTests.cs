using System;
using System.Diagnostics;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Host;
using JRPG.ScriptingEngine.Tests.Module;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    internal class CompilerTests
    {
        private GossipVM m_GossipVM = new GossipVM();

        public CompilerTests()
        {
            m_GossipVM.RegisterGlobalAction("conversation", new ActionInfo("say", Say, null, null));
            m_GossipVM.RegisterCustomEvent("OnCustomParameter()", typeof(CustomEventParameters));
        }

        private ScriptResult Say(GossipActionParameter gossipActionParameter)
        {
            Trace.WriteLine(String.Format("say {0}", gossipActionParameter.Parameters[0]));
            return ScriptResult.Continue;
        }

        [Test]
        public void IsTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void CompilerTest001()
        {
            var input = @"
	            globalVar[43] = 1;
	            globalVar[44] = 1;
	            globalVar[45] = 2;

	            if (globalVar[43] + globalVar[44] == globalVar[45])
	            {
		            conversation.say(""Correct"");
	            }
	            else
	            {
		            conversation.say(""Logic Error"");
	            }";

            var results = m_GossipVM.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_GossipVM, results);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptMissingSemiColonException))]
        public void MissingSemiColonTest001()
        {
            const string input = @"OnStart() 
                            { 
                                system.Print(""Start""); 
                            }
                          OnEnd()
                            {
                                system.Print(""End"");
                            }
                          OnCustomParameter()
                            {
                                system.Print(""Custom"");
                                event.Name = ""John""   // Missing Semicolon Here
                                system.Print(event.Name);
                                localVar[6] = 128;
                            }";

            var script = m_GossipVM.CompileScript(input);
            var binding = m_GossipVM.HostBridge.EventBindingTable.GetBindingByName("OnCustomParameter()");


            TestHelper.RunScriptToCompletion(m_GossipVM, script);
            Assert.IsFalse(m_GossipVM.HasRunningScripts());

            var eventParameters = new CustomEventParameters();
            eventParameters.Name = "Josh";

            script.RaiseEvent(binding, eventParameters);
            Assert.IsTrue(script.LocalState.Var[6] == 128);
            Assert.IsTrue(eventParameters.Name == "John");
        }
    }
}
