using System;
using System.Diagnostics;
using System.Threading;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Host;
using JRPG.ScriptingEngine.Tests.Module;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    public class TODONewFeatureTests
    {
        private GossipVM m_Vm;

        private Int32 m_TestPlayerHealth = 100;

        private Customer m_Customer = new Customer();
        private Customer m_Customer1 = new Customer();
        private Customer m_Customer2 = new Customer();

        private Account m_Account1 = new Account();

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            m_Customer1.Id = 1;
            m_Customer2.Id = 2;
            m_Customer1.Name = "Joe";
            m_Customer2.Name = "Jane";

            m_Customer1.Account.Name = "Cheque";
            m_Customer2.Account.Name = "Savings";

            m_Vm = new GossipVM();

            m_Vm.RegisterGlobalAction("dialog", new ActionInfo("Say", Say, null, null));
            m_Vm.RegisterGlobalAction("dialog", new ActionInfo("AddResponse", AddResponse, null, null));
            m_Vm.RegisterGlobalAction("dialog", new ActionInfo("GotoLabel", GotoLabel, null, null));
            m_Vm.RegisterGlobalAction("dialog", new ActionInfo("Interject", Interject, null, null));

            m_Vm.RegisterGlobalAction("test", new ActionInfo("NoParams", NoParams));


            m_Vm.RegisterGlobalFunction("GetAccount", new Func<Account>(GetAccount));
            m_Vm.RegisterGlobalFunction("GetCustomer", new Func<Int32, Customer>(GetCustomer));

            m_Vm.RegisterType(typeof (Customer));
            m_Vm.RegisterType(typeof (Account));
            m_Vm.RegisterEnum("TEST_ENUM", 1);
        }

        public Account GetAccount()
        {
            return m_Account1;
        }

        public Customer GetCustomer(Int32 id)
        {
            if (id == 0)
                return m_Customer1;

            if (id == 1)
                return m_Customer2;

            throw new GossipScriptException("Error");
        }


        private ScriptResult NoParams(GossipActionParameter gossipActionParameter)
        {
            Trace.WriteLine(String.Format("No Params"));
            return ScriptResult.Continue;
        }


        private ScriptResult Say(GossipActionParameter gossipActionParameter)
        {
            Trace.WriteLine(String.Format("say {0}", gossipActionParameter.Parameters[0]));
            return ScriptResult.Continue;
        }

        private ScriptResult AddResponse(GossipActionParameter gossipActionParameter)
        {
            Trace.WriteLine(String.Format("addresponse {0}, {1}, {2}", gossipActionParameter.Parameters[0],
                                          gossipActionParameter.Parameters[1], gossipActionParameter.Parameters[2]));
            return ScriptResult.Continue;
        }

        private ScriptResult GotoLabel(GossipActionParameter gossipActionParameter)
        {
            Trace.WriteLine(String.Format("gotolabel {0}", gossipActionParameter.Parameters[0]));
            return ScriptResult.Continue;
        }

        private ScriptResult Interject(GossipActionParameter gossipActionParameter)
        {
            Trace.WriteLine(String.Format("interject {0}, {1}", gossipActionParameter.Parameters[0],
                                          gossipActionParameter.Parameters[1]));
            return ScriptResult.Continue;
        }


        [Test]
        public void TestTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void NewFeature_DefineEnum()
        {
            // Add define enum feature

            const string input = @"
                #Define MY_ENUM = 5;            // Local enum
                #Define global MY_ENUM2 = 5;    // global enum

                localVar[MY_ENUM] = 5;
                globalVar[MY_ENUM2] = MY_ENUM2;

                if (globalVar[MY_ENUM2] == localVar[MY_ENUM])
                {
                  system.Print(""true"");
                }
                else
                {
                  system.Print(""false"");
                }";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void NewFeature_DefineVar()
        {
            // Add define enum feature

            const string input = @"
                global int MY_INT = 1;          // Behind the scenes assign to a free global register
                local int MY_INT2 = 1;          // Behind the scenes assign to a free local register

                if (MY_INT + MY_INT2 == 2)
                {
                    system.Print(""true"")
                } 
                else
                {
                    system.Print(""false"");
                }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }
    }
}
