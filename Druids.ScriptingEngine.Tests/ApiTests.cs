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
    public class ApiTests
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

            m_Vm.RegisterType(typeof(Customer));
            m_Vm.RegisterType(typeof(Account));
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
            Trace.WriteLine(String.Format("addresponse {0}, {1}, {2}", gossipActionParameter.Parameters[0], gossipActionParameter.Parameters[1], gossipActionParameter.Parameters[2]));
            return ScriptResult.Continue;
        }

        private ScriptResult GotoLabel(GossipActionParameter gossipActionParameter)
        {
            Trace.WriteLine(String.Format("gotolabel {0}", gossipActionParameter.Parameters[0]));
            return ScriptResult.Continue;
        }

        private ScriptResult Interject(GossipActionParameter gossipActionParameter)
        {
            Trace.WriteLine(String.Format("interject {0}, {1}", gossipActionParameter.Parameters[0], gossipActionParameter.Parameters[1]));
            return ScriptResult.Continue;
        }


        [Test]
        public void TestTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void FailTest001()
        {
            // Shouldn't be able to reassign ref[0] as a different type after it has been assigned
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[0] = GetAccount();
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void FailTest002()
        {
            // Shouldn't be able to set a read only property
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[0].Id = 5;  // Invalid
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void FailTest003()
        {
            // Account.Id does not exist
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetAccount();
                    if (ref[0].Id <= ref[1].Blah) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void FailTest004()
        {
            // Account.Id does not exist
            const string input = @"
                    ref[0] = GetCustomer(0);
                    if (ref[0].Id <= GetAccount().Blah) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void ApiTest0001()
        {
            // Should report invalid number of parameters
            const string input = @"localVar[16] = max(5,10,5)";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void LoadTestConversationScript001()
        {
            // Read file
            var script = TestHelper.ReadTextFile("testscript001.txt");
            Assert.IsTrue(script.Length > 0);

            var splitScript = TestHelper.SplitScript(script);
            Assert.IsTrue(splitScript.Count == 4);

            foreach (var s in splitScript)
            {
                Trace.WriteLine("Running ScriptText");
               // Trace.WriteLine(s);
                var compiledScript = m_Vm.RunScript(s);

                Trace.WriteLine("");
            }

            while (m_Vm.HasRunningScripts())
            {
                Thread.Sleep(16);
                m_Vm.Update(16);
            }
        }

        [Test]
        public void LoadTestParallel001()
        {
            // Read file
            var script = TestHelper.ReadTextFile("testscript_parallel.txt");
            Assert.IsTrue(script.Length > 0);

            var splitScript = TestHelper.SplitScript(script);
            Assert.IsTrue(splitScript.Count == 1);

            foreach (var s in splitScript)
            {
                Trace.WriteLine("Running ScriptText");
                var compiledScript = m_Vm.RunScript(s);

                Trace.WriteLine("");
            }

            while (m_Vm.HasRunningScripts())
            {
                Thread.Sleep(16);
                m_Vm.Update(16);
            }
        }

        [Test]
        public void LoadTestConversationScript002()
        {
            // Read file
            var script = TestHelper.ReadTextFile("testscript002.txt");
            Assert.IsTrue(script.Length > 0);

            var splitScript = TestHelper.SplitScript(script);
            Assert.IsTrue(splitScript.Count == 6);

            foreach (var s in splitScript)
            {
                Trace.WriteLine("Running ScriptText");
                var compiledScript = m_Vm.RunScript(s);
                Trace.WriteLine("");
            }

            while (m_Vm.HasRunningScripts())
            {
                Thread.Sleep(16);
                m_Vm.Update(16);
            }
        }

        [Test]
        public void ApiTest001()
        {
            const string input = @"dialog.Say(""test"");";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }


        [Test]
        public void ApiTest002()
        {
            const string input = @"dialog.Say(""test"" + ""test"");";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ApiTest003()
        {
            const string input = @"dialog.Say(""test"" + ""test"");";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ApiTest004()
        {
            const string input = @"localVar[16] = max(5,10+5)";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[16] == 15);
        }

        [Test]
        public void ApiTest005()
        {
            const string input = @"test.NoParams();";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ApiTest006_EnumInFunction()
        {
            const string input = @"dialog.GotoLabel(TEST_ENUM);";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ApiTest007_DefineEnum()
        {
            // Add define enum feature

            const string input = @"
                #Define MY_ENUM = 0;
                localVar[MY_ENUM] = 5;
                if (localVar[MY_ENUM] == 5)
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
        public void ReferenceTest000()
        {
            // Tests for equality of two references
            const string input = @"
            if(localVar[0] == localVar[1]) {
                    if (ref[0] == ref[1]) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest001()
        {
            // Tests for equality of two references
            const string input = @"
                    ref[0] = GetAccount();
                    ref[1] = GetAccount();
                    if (ref[0] == ref[1]) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest002()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetCustomer(1);
                    if (ref[0].Id <= ref[1].Id) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest002b()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetCustomer(1);
                    if (ref[0].Id <= 1) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest003()
        {
            const string input = @"
                    if (GetCustomer(0).Id <= GetCustomer(1).Id) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest004()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetCustomer(1);
                    if (ref[0] == ref[0]) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest005()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetCustomer(1);
                    if (ref[0] != ref[1]) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest006()
        {
            const string input = @"
                    localVar[0] = GetCustomer(0).Id;
                    localVar[1] = GetCustomer(1).Id;
                    if (localVar[0] != localVar[1]) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest010()
        {
            const string input = @"
                    ref[1] = GetCustomer(1);
                    ref[1].Age = 18;
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(m_Customer2.Age == 18);
        }

        [Test]
        public void ReferenceTest011()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetCustomer(0);
                    localVar[0] = ref[0].Id;
                    localVar[1] = ref[1].Id;
                    if (localVar[0] == localVar[1] && localVar[1] == 1 && ref[0].Id == 1) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest012()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetCustomer(1);
                    if (ref[0].Account.Credit <= ref[1].Account.Credit) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest013()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    if (2 + ref[0].Account.Credit == 2 + GetCustomer(0).Account.Credit) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest014()
        {
            const string input = @"localStr[0] = GetCustomer(0).Account.Name;";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Str[0] == m_Customer1.Account.Name);
        }

        [Test]
        public void ReferenceTest015()
        {
            const string input = @"
                    localVar[0] = GetCustomer(0).Id;
                    localVar[1] = GetCustomer(1).Id;

                    if (1 < localVar[0] + localVar[1]) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceTest016()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetCustomer(1);

                    if (1 < ref[0].Id + ref[1].Id) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 222);
        }

        [Test]
        public void ReferenceMethodCallTest001()
        {
            const string input = @"localVar[0] = GetCustomer(0).Account.Method1();";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 2);
        }

        [Test]
        public void ReferenceMethodCallTest002()
        {
            const string input = @"localVar[0] = 2* GetCustomer(1).Account.Method3(5+2, GetCustomer(1).Id) - 1;";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[0] == 9);
        }

        [Test]
        public void ReferenceMethodCallTest003()
        {
            const string input = @"localStr[0] = GetCustomer(1).Concat(""a"",""bc"",""d"");";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Str[0] == "abcd");
        }

        [Test]
        public void ReferenceMethodCallTest004()
        {
            const string input = @"localStr[0] = GetCustomer(1).Mixed(1.5,""bc"",2);";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Str[0] == "1.5bc2");
        }

        [Test]
        public void ReferenceMethodCallTest005()
        {
            const string input = @"localStr[0] = GetCustomer(1).Mixed(1.5,""bc"", GetCustomer(1).Id);";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Str[0] == "1.5bc2");
        }

        [Test]
        [ExpectedException(typeof(GossipScriptParameterException))]
        public void ReferenceMethodCallTest006()
        {
            // Should not compile
            const string input = @"localStr[0] = GetCustomer(1).Mixed(1.5, GetCustomer(1).Id, GetCustomer(1).Id);";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptMethodNotFoundException))]
        public void ReferenceMethodCallTest007()
        {
            // Should not compile
            const string input = @"ref[0] = GetCustomer(1); localVar[1] = ref[0].Method1();";
            var output = m_Vm.CompileScript(input);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptParameterException))]
        public void ReferenceMethodCallTest008()
        {
            const string input = @"ref[0] = GetCustomer(1); localStr[1] = ref[0].Mixed(1.5, GetCustomer(1).Id, GetCustomer(1).Id);";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Str[1] == "1.522" );
        }

        [Test]
        public void ReferenceMethodCallTest009()
        {
            const string input = @"ref[0] = GetCustomer(1); localStr[2] = ref[0].Mixed(1.5,""bc"", GetCustomer(1).Id);";

            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Str[2] == "1.5bc2");
        }

        [Test]
        public void ReferenceActionCallTest000()
        {
            // Should be valid script
            const string input = @"GetCustomer(1).CustomAction()";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptParameterException))]
        public void ReferenceActionCallTest001()
        {
            // Should be valid script
            const string input = @"GetCustomer(1).CustomAction(""Hello"")";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ReferenceActionCallTest001b()
        {
            // Should be valid script
            const string input = @"GetCustomer(1).CustomAction2(""Hello"")";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }


        [Test]
        public void ReferenceActionCallTest002()
        {
            const string input = @"ref[1] = GetCustomer(1); ref[1].CustomAction()";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptParameterException))]
        public void ReferenceActionCallTest003()
        {
            // Should report invalid number of parameters
            const string input = @"ref[1] = GetCustomer(1); ref[1].CustomAction(""Test"");";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ReferenceActionCallTest004()
        {
            const string input = @"ref[1] = GetCustomer(1); ref[1].CustomAction2(""Test"");";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptParameterException))]
        public void ReferenceActionCallTest005()
        {
            // Should report invalid number of parameters
            const string input = @"ref[1] = GetCustomer(1); ref[1].CustomAction2(""Test"",5);";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ReferenceActionCallTest006()
        {
            const string input = @"ref[1] = GetCustomer(1); ref[1].CustomAction3(""Test"", 5);";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptParameterException))]
        public void ReferenceActionCallTest007()
        {
            // Should report invalid number of parameters
            const string input = @"ref[1] = GetCustomer(1).Account; ref[1].CustomAction(""Test"");";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ReferenceActionCallTest008()
        {
            const string input = @"ref[1] = GetCustomer(1).Account; ref[1].CustomAction();";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ReferenceActionCallTest009()
        {
            const string input = @"ref[1] = GetCustomer(1); ref[1].Account.CustomAction();";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ReferencePropertySet()
        {
            // Should be valid script
            const string input = @"GetCustomer(1).Age = 64;";
            var output = m_Vm.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(m_Customer2.Age == 64);
        }

        [Test]
        public void SelfTest_Method_001()
        {
            const string input = @"localVar[1] = self.Method1()";
          
            var output = m_Vm.CompileScript(input, typeof(Account));
            output.ScriptOwner = GetAccount();

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[1] == 2);
        }

        [Test]
        public void SelfTest_Method_002()
        {
            const string input = @"localVar[1] = self.Method2(2)*2";

            var output = m_Vm.CompileScript(input, typeof(Account));
            output.ScriptOwner = GetAccount();

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[1] == 8);
        }

        [Test]
        public void SelfTest_Property_001()
        {
            const string input = @"localVar[6] = self.Id";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            output.ScriptOwner = GetCustomer(1);

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[6] == 2);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptException))]
        public void SelfTest_Property_002()
        {
            // Should report is read only
            const string input = @"localVar[6] = 4; self.Id = 4;";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            output.ScriptOwner = GetCustomer(1);

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[6] == 2);
        }

        [Test]
        public void SelfTest_Property_003()
        {
            // Should report is read only
            const string input = @"localVar[6] = 4; self.Age = localVar[6];";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            var customer = GetCustomer(1);
            output.ScriptOwner = customer;

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(customer.Age == 4);
        }


        [Test]
        public void SelfTest_Property_004()
        {
            const string input = @"localStr[2] = self.Account.Name";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            output.ScriptOwner = GetCustomer(1);

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Str[2] == "Savings");
        }

        [Test]
        public void SelfTest_PropertySet_001()
        {
            // Should report is read only
            const string input = @"localVar[6] = 4; self.Age = 4;";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            var customer = GetCustomer(1);
            output.ScriptOwner = customer;

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(customer.Age == 4);
        }

        [Test]
        public void SelfTest_PropertySet_002()
        {
            const string input = @"self.Account.Credit = 4";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            var customer = GetCustomer(1);
            output.ScriptOwner = customer;

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(customer.Account.Credit == 4);
        }

        [Test]
        public void SelfTest_Action_001()
        {
            const string input = @"self.CustomAction2(""Hello"")";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            output.ScriptOwner = GetCustomer(1);

            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void SelfTest_Action_002()
        {
            const string input = @"self.CustomAction3(""Hello"", 5);";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            output.ScriptOwner = GetCustomer(1);

            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void SelfTest_Action_003()
        {
            const string input = @"self.Account.CustomAction();";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            output.ScriptOwner = GetCustomer(1);

            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void SelfTest_Action_004()
        {
            const string input = @"self.Account.CustomAction2(""Hello"");";

            var output = m_Vm.CompileScript(input, typeof(Customer));
            output.ScriptOwner = GetCustomer(1);

            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ModuleTest_Method_001()
        {
            const string input = @"localVar[42] = GetCustomer(0).TestMethod001();";
            var output = m_Vm.CompileScript(input, typeof(Customer));

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Var[42] == 1);
        }

        [Test]
        public void ModuleTest_Action_001()
        {
            const string input = @"GetCustomer(0).CustomAction();";
            var output = m_Vm.CompileScript(input, typeof(Customer));

            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ModuleTest_Action_002()
        {
            const string input = @"GetCustomer(0).Account.CustomAction2(""Hello"");";
            var output = m_Vm.CompileScript(input, typeof(Customer));

            TestHelper.RunScriptToCompletion(m_Vm, output);
        }

        [Test]
        public void ModuleTest_Property_001()
        {
            const string input = @"localStr[42] = GetCustomer(GetCustomer(0).TestMethod001()).Name";
            var output = m_Vm.CompileScript(input, typeof(Customer));

            TestHelper.RunScriptToCompletion(m_Vm, output);

            Assert.IsTrue(output.LocalState.Str[42] == "Jane");
        }   
    }
}
