using System;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Compiler;
using JRPG.ScriptingEngine.Tests.Module;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{

    [TestFixture]
    public class ScriptNodeTests
    {
        private readonly ScriptNodeParser m_ScriptNodeParser;
         
        private readonly GossipVM m_GossipVM = new GossipVM();

        public ScriptNodeTests()
        {
            m_ScriptNodeParser = new ScriptNodeParser();

            m_GossipVM.RegisterCustomEvent("OnCustom()");
            m_GossipVM.RegisterCustomEvent("OnCustomParameter()", typeof(CustomEventParameters));
        }

        [Test]
        public void Pass()
        {
            Assert.Pass();
        }

        [Test]
        public void ParseTest001()
        {
            const string input = @"OnStart() 
                            { 
                                system.Print(""Start""); 
                            }
                          OnEnd()
                            {
                                system.Print(""End"");
                            }";

            var scriptNodeInfo =  m_ScriptNodeParser.ParseScriptNode(input);

            Assert.IsTrue(scriptNodeInfo.OnEnterScript.EventScript == @"system.Print(""Start"");");
            Assert.IsTrue(scriptNodeInfo.OnExitScript.EventScript == @"system.Print(""End"");");
            Assert.IsTrue(scriptNodeInfo.CustomEventScripts.Count == 0);
            Assert.IsTrue(scriptNodeInfo.OnUpdateScript == null);
            Assert.IsTrue(scriptNodeInfo.OnInterruptScript == null);
        }

        [Test]
        public void ParseTest002()
        {
            const string input = @"OnStart() 
                            { 
                                system.Print(""Start""); 
                            }
                          OnEnd()
                            {
                                system.Print(""End"");
                            }
                          OnUpdate() 
                            { 
                                system.Print(""Run""); 
                            }
                          OnInterrupt() 
                            { 
                                system.Print(""Interrupt""); 
                            }";

            var scriptletInfo = m_ScriptNodeParser.ParseScriptNode(input);

            Assert.IsTrue(scriptletInfo.OnEnterScript.EventScript == @"system.Print(""Start"");");
            Assert.IsTrue(scriptletInfo.OnExitScript.EventScript == @"system.Print(""End"");");
            Assert.IsTrue(scriptletInfo.CustomEventScripts.Count == 0);
            Assert.IsTrue(scriptletInfo.OnUpdateScript.EventScript == @"system.Print(""Run"");");
            Assert.IsTrue(scriptletInfo.OnInterruptScript.EventScript == @"system.Print(""Interrupt"");");
        }

        [Test]
        public void ParseTest003()
        {
            const string input = @"system.Print(""Run"");";

            var scriptletInfo = m_ScriptNodeParser.ParseScriptNode(input);
            Assert.IsTrue(scriptletInfo.OnUpdateScript.EventScript == @"system.Print(""Run"");");
        }

        [Test]
        public void ParseTest004()
        {
            const string originalScriptString = @"  OnStart()
                                                    {
                                                       system.Print(""base start"");
                                                    }
                                                    OnUpdate() 
                                                    { 
                                                        localVar[0] = 5; 
                                                        system.Print(""base script""); 
                                                    }
                                                    OnEnd()
                                                    {
                                                        system.Print(""base end"");
                                                    }
                                                    ";

            const string scriptletString = @"OnStart()
                                             {
                                                system.Print(""starting scriptlet""); 
                                             };

                                             OnUpdate() 
                                             { 
                                                localVar[1] = 6; 
                                                system.Print(""scriptlet1""); 
                                             } 
                                             OnEnd() 
                                             {
                                                localVar[2] = 12; 
                                                system.Print(""exiting scriptlet""); 
                                             }";

            var scriptProcess = m_GossipVM.CompileScript(originalScriptString);
            var scriptlet = m_GossipVM.CompileNode(scriptletString);

            scriptProcess.PushActiveNode(scriptlet);
            m_GossipVM.RunScript(scriptProcess);

            Assert.IsTrue(scriptProcess.LocalState.Var[1] == 6);
            Assert.IsTrue(scriptProcess.LocalState.Var[0] == 0);

            scriptProcess.PopActiveNode();
            m_GossipVM.RunScript(scriptProcess);
            Assert.IsTrue(scriptProcess.LocalState.Var[2] == 12);
            Assert.IsTrue(scriptProcess.LocalState.Var[0] == 5);

            m_GossipVM.RunScript(scriptProcess);
        }

        [Test]
        public void ParseTest005()
        {
            const string originalScriptString = @"  OnStart()
                                                    {
                                                       system.Print(""base start"");
                                                    }
                                                    OnUpdate() 
                                                    { 
                                                        localVar[0] = 5; 
                                                        system.Print(""base script""); 
                                                    }
                                                    OnEnd()
                                                    {
                                                        system.Print(""base end"");
                                                    }
                                                    ";

            const string scriptletString = @"OnStart()
                                             {
                                                localVar[1] = 6; 
                                                system.Print(""starting scriptlet""); 
                                             };

                                             
                                             OnEnd() 
                                             {
                                                localVar[2] = 12; 
                                                system.Print(""exiting scriptlet""); 
                                             }";

            var scriptProcess = m_GossipVM.CompileScript(originalScriptString);
            var scriptlet = m_GossipVM.CompileNode(scriptletString);

            scriptProcess.PushActiveNode(scriptlet);
            m_GossipVM.RunScript(scriptProcess);

            Assert.IsTrue(scriptProcess.LocalState.Var[1] == 6);
            Assert.IsTrue(scriptProcess.LocalState.Var[0] == 5);

            scriptProcess.PopActiveNode();
            m_GossipVM.RunScript(scriptProcess);
            Assert.IsTrue(scriptProcess.LocalState.Var[2] == 12);

            m_GossipVM.RunScript(scriptProcess);
        }

        [Test]
        public void ParseTest006()
        {
            const string originalScriptString = @"  OnStart()
                                                    {
                                                       system.Print(""base start"");
                                                    }
                                                    OnUpdate() 
                                                    { 
                                                        localVar[0] = 5; 
                                                        system.Print(""base script""); 
                                                        exit;
                                                    }
                                                    OnEnd()
                                                    {
                                                        system.Print(""base end"");
                                                    }
                                                    ";

            const string scriptletString = @"OnStart()
                                             {
                                                localVar[1] = 6; 
                                                system.Print(""starting scriptlet""); 
                                             };

                                            OnUpdate() 
                                            {   
                                                system.Print(""start wait 2000""); 
                                                system.Wait(2000);
                                                system.Print(""done wait""); 
                                                localVar[1] = 6; 
                                                exit;
                                             }
                                             
                                             OnEnd() 
                                             {
                                                localVar[2] = 12; 
                                                system.Print(""exiting scriptlet""); 
                                             }";

            var scriptProcess = m_GossipVM.CompileScript(originalScriptString);
            var scriptlet = m_GossipVM.CompileNode(scriptletString);

            Console.WriteLine("Compiled");

            scriptProcess.PushActiveNode(scriptlet);
            m_GossipVM.RunScript(scriptProcess);

           
            TestHelper.RunScriptToCompletion(m_GossipVM, scriptProcess);
            
           // Assert.IsTrue(scriptProcess.LocalState.Var[2] == 12);
            Assert.IsTrue(scriptProcess.LocalState.Var[1] == 6);
            Assert.IsTrue(scriptProcess.LocalState.Var[0] == 5);
        }

        [Test]
        public void ParseTest007()
        {
            const string originalScriptString = @"  OnStart()
                                                    {
                                                       system.Print(""base start"");
                                                    }
                                                    OnUpdate() 
                                                    { 
                                                        localVar[0] = 5; 
                                                        system.Print(""base script""); 
                                                        exit;
                                                    }
                                                    OnEnd()
                                                    {
                                                        system.Print(""base end"");
                                                    }
                                                    OnInterrupt()
                                                    {
                                                        system.Print(""Interrupt"");
                                                    }
                                                    ";

            const string scriptletString = @"OnStart()
                                             {
                                                localVar[1] = 6; 
                                                system.Print(""starting scriptlet""); 
                                             };

                                            OnUpdate() 
                                            {   
                                                system.Print(""start wait 100""); 
                                                system.Wait(100);
                                                system.Print(""done wait""); 
                                                localVar[1] = 6; 
                                                exit;
                                             }
                                             
                                             OnEnd() 
                                             {
                                                localVar[2] = 12; 
                                                system.Print(""exiting scriptlet""); 
                                             }";

            var scriptProcess = m_GossipVM.CompileScript(originalScriptString);
            var scriptlet = m_GossipVM.CompileNode(scriptletString);

            Console.WriteLine("Compiled");

            scriptProcess.PushActiveNode(scriptlet);
            m_GossipVM.RunScript(scriptProcess);


            TestHelper.RunScriptToCompletion(m_GossipVM, scriptProcess);

            Assert.IsTrue(scriptProcess.LocalState.Var[1] == 6);
            Assert.IsTrue(scriptProcess.LocalState.Var[0] == 5);
        }

        [Test]
        public void ParseTestCustomEvent001()
        {
            const string input = @"OnStart() 
                            { 
                                system.Print(""Start""); 
                            }
                          OnEnd()
                            {
                                system.Print(""End"");
                            }
                          OnCustom()
                            {
                                system.Print(""Custom"");
                                localVar[6] = 128;
                            }";

            var script = m_GossipVM.CompileScript(input);
            var binding = m_GossipVM.HostBridge.EventBindingTable.GetBindingByName("OnCustom()");

     
            TestHelper.RunScriptToCompletion(m_GossipVM, script);

            Assert.IsFalse(m_GossipVM.HasRunningScripts());

            script.RaiseEvent(binding);
            Assert.IsTrue(script.LocalState.Var[6] == 128);
           
        }

        [Test]
        public void ParseTestCustomEvent002()
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
                                system.Print(event.Name); // TODO Custom event
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
        }

        [Test]
        public void ParseTestCustomEvent003()
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
                                event.Name = ""John"";
                                system.Print(event.Name); // TODO Custom event
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
