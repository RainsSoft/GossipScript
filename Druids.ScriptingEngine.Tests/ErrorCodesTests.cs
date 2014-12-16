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
    internal class ErrorCodesTests
    {
        private readonly GossipVM m_Engine = new GossipVM();
        private Customer m_Customer1 = new Customer();
        private Customer m_Customer2 = new Customer();

        public ErrorCodesTests()
        {
            m_Engine.RegisterGlobalAction("conversation", new ActionInfo("say", Say, null, null));
            m_Engine.RegisterGlobalFunction("GetCustomer", new Func<Int32, Customer>(GetCustomer));

            m_Engine.RegisterType(typeof(Customer));
        }

        public Customer GetCustomer(Int32 id)
        {
            if (id == 0)
                return m_Customer1;

            if (id == 1)
                return m_Customer2;

            throw new GossipScriptException("Error");
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
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram001b()
        {
            const string input = @"";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ValidProgram002()
        {
            const string input = @"globalVar[5] = 2;";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ValidProgram003()
        {
            const string input = @"if (5+4==5+4) { }";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ValidProgram006()
        {
            const string input = @"globalVar[3] = 85; if (globalVar[3] > 15) { }";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ValidProgram007()
        {
            const string input = @"if (localVar[3] > 15) { }";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram001()
        {
            const string input = @"globalVar[5+5] = 2;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram002()
        {
            const string input = @"if (5+4=5+4) { }";
            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram004()
        {
            const string input = @"else { }";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram005()
        {
            const string input = @"globalVar[3] = 15,5";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram006()
        {
            const string input = @"if (globalVar[3] > 15,5) { }";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram007()
        {
            const string input = @"if (globalVar[3] > 155)";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram008()
        {
            // Out of bounds
            const string input = @"globalVar[-1] = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram009()
        {
            // Out of bounds
            const string input = @"globalVar[1024] = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram010()
        {
            // Out of bounds
            const string input = @"localVar[-1] = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram011()
        {
            // Out of bounds
            const string input = @"localVar[1024] = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram012()
        {
            // Out of bounds
            const string input = @"localFlag[-1] = 5;";
            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram013()
        {
            // Out of bounds
            const string input = @"localFlag[1024] = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram014()
        {
            // Out of bounds
            const string input = @"globalFlag[-1] = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram015()
        {
            // Out of bounds
            const string input = @"globalFlag[1024] = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram016()
        {
            // Out of bounds
            const string input = @"globalFlag = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram017()
        {
            // Invalid Assignment
            const string input = @"ref[0] = 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram018()
        {
            // Invalid Assignment
            const string input = @"ref[0] = GetCustomer(1) + 5;";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram019()
        {
            // Invalid number of parameters
            const string input = @"ref[0] = GetCustomer()";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram020()
        {
            // Invalid number of parameters
            const string input = @"ref[0] = GetCustomer(1,1)";

            var output = m_Engine.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void InValidProgram021()
        {
            // Invalid property call on reference object
            const string input = @"if(ref[5].Id == 0) { system.Print(""error""); }";

            var output = m_Engine.CompileScript(input);
        }
    }
}
