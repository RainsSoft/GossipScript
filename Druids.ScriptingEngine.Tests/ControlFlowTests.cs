using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    class ControlFlowTests
    {
        readonly GossipVM m_Engine = new GossipVM();

        public ControlFlowTests()
        {

        }

        [Test]
        public void Pass()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Yeild001()
        {
            const string input = @"
                    system.Print(""Start ScriptText"");
                    localVar[16] = 2;
	                if (localVar[16] == 2)  {
		                yeild; // Exists Execution
                        system.Print(""Success"");  // Resumes Execution at this point
                        localVar[32] = 666;
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
            Assert.IsTrue(output.LocalState.Var[32] == 666);
        }

        [Test]
        public void exit002()
        {
            const string input = @"
                    system.Print(""Start ScriptText"");
                    localVar[16] = 2;
	                if (localVar[16] == 2)  {
                        localVar[32] = 666;
		                exit; // Yeilds Execution resumes at the top
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
            Assert.IsTrue(output.LocalState.Var[32] == 666);
        }

        [Test]
        public void return002()
        {
            const string input = @"
                    system.Print(""Start ScriptText""); // Should be printed twice
                    system.Print(localVar[16]);
                    localVar[16] = localVar[16]+localVar[16];
                    
                    system.Print(""pre: "" + localVar[16]);
	                if (localVar[16] == 2)  {
                        system.Print(""post:"" + localVar[16]);
                        localVar[32] = 666;
                        system.Print(""return"");
		                return; // Ends the script
	                }
                    else
                    {
                        system.Print(""Not 2"");
                    }
                    localVar[2] = 5;
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            output.LocalState.Var[16] = 1;
            TestHelper.RunScriptToCompletion(m_Engine, output);
            Assert.IsTrue(output.LocalState.Var[32] == 666);
            Assert.IsTrue(output.LocalState.Var[16] == 4);
        }

        [Test]
        public void exit001()
        {
            const string input = @"
                    system.Print(""Start ScriptText"");
                    localVar[16] = 2;
	                if (localVar[16] == 2)  {
                        localVar[32] = 222; 
		                exit; // Exits process
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
            Assert.IsTrue(output.LocalState.Var[32] == 222);

            output.Restart();
            Console.WriteLine("");
            Console.WriteLine("Restart");
            Console.WriteLine("");

            TestHelper.RunScriptToCompletion(m_Engine, output);
            Assert.IsTrue(output.LocalState.Var[32] == 222);

        }
    }
}
