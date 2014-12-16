using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Compiler;
using JRPG.ScriptingEngine.Tests.Module;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    public class StateMachineTests
    {
        private readonly ScriptNodeParser m_ScriptNodeParser;

        private readonly GossipVM m_GossipVM = new GossipVM();

        public StateMachineTests()
        {
            m_ScriptNodeParser = new ScriptNodeParser();

            m_GossipVM.RegisterCustomEvent("OnCustom()");
            m_GossipVM.RegisterCustomEvent("OnCustomParameter()", typeof (CustomEventParameters));
        }

        [Test]
        public void Pass()
        {
            Assert.Pass();
        }

        [Test]
        public void ParseTest001()
        {
            const string input = @"

                            @stateOne
                            {
                                OnStart() 
                                { 
                                    localVar[0] = 1;
                                    system.Print(""Start 1""); 
                                }

                                OnEnd()
                                {
                                    localVar[0]++;
                                    system.Print(""End 1"");
                                }

                                OnUpdate()
                                {
                                    localVar[0]++;
                                    system.Print(""Update"");
                                }
                            }";

            var compiledScript = m_GossipVM.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_GossipVM, compiledScript);

            var nodes = compiledScript.GetNodes().ToList();
            Assert.IsTrue(nodes.Count() == 1);
            Assert.IsTrue(nodes[0].Name == "stateOne");
            Assert.IsTrue(compiledScript.LocalState.Var[0] == 3.0f);
        }
     
        [Test]
        public void ParseTest002()
        {
            const string input = @"

                            @stateOne
                            {
                                OnStart() 
                                { 
                                    localVar[0] = 1;
                                    system.Print(""Start 1""); 
                                }

                                OnEnd()
                                {
                                    localVar[2] = localVar[0] + localVar[1];    //2
                                    system.Print(""End 1"");
                                }

                                OnUpdate()
                                {
                                    localVar[1] = 1;
                                    system.Print(""Update 1"");
                                    system.Goto(""stateTwo"");
                                }
                            }

                            @stateTwo
                            {
                                OnStart() 
                                { 
                                    localVar[3] = 1;
                                    system.Print(""Start 2""); 
                                }

                                OnEnd()
                                {
                                    localVar[5] = localVar[4] + localVar[3] + localVar[2];  //4
                                    system.Print(""End 2"");
                                }
                                
                                OnUpdate()
                                {
                                    localVar[4] = 1;
                                    system.Print(""Update 2"");
                                    // End
                                }
                            }";

            var compiledScript = m_GossipVM.CompileScript(input);
           
            var nodes = compiledScript.GetNodes().ToList();
            Assert.IsTrue(nodes.Count() == 2);
            Assert.IsTrue(nodes[0].Name == "stateOne");
            Assert.IsTrue(nodes[1].Name == "stateTwo");

            TestHelper.RunScriptToCompletion(m_GossipVM, compiledScript);

            Assert.IsTrue(compiledScript.LocalState.Var[0] == 1);
            Assert.IsTrue(compiledScript.LocalState.Var[1] == 1);
            
            Assert.IsTrue(compiledScript.LocalState.Var[3] == 1);
            Assert.IsTrue(compiledScript.LocalState.Var[4] == 1);

            Assert.IsTrue(compiledScript.LocalState.Var[2] == 2);
            Assert.IsTrue(compiledScript.LocalState.Var[5] == 4);
        }


        [Test]
        public void ParseTest003()
        {
            const string input = @"

                            @stateOne
                            {
                                OnUpdate()
                                {
                                    localVar[0] = 1;
                                    system.Print(""Update 1"");
                                    system.Goto(""stateTwo"");
                                }
                            }

                            @stateTwo
                            {   
                                OnUpdate()
                                {
                                    localVar[1] = 1;
                                    system.Print(""Update 2"");
                                    // End
                                }
                            }";

            var compiledScript = m_GossipVM.CompileScript(input);

            var nodes = compiledScript.GetNodes().ToList();
            Assert.IsTrue(nodes.Count() == 2);
            Assert.IsTrue(nodes[0].Name == "stateOne");
            Assert.IsTrue(nodes[1].Name == "stateTwo");

            TestHelper.RunScriptToCompletion(m_GossipVM, compiledScript);

            Assert.IsTrue(compiledScript.LocalState.Var[0] == 1);
            Assert.IsTrue(compiledScript.LocalState.Var[1] == 1);
        }
    }
}
