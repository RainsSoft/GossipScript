using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    internal class ExpressionTests
    {
        private readonly GossipVM m_Engine = new GossipVM();

        public ExpressionTests()
        {
            m_Engine.RegisterGlobalFunction("double", new Func<Double, double>(o => { return 2*o; }));
        }

        [Test]
        public void IsTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void ExpressionTest001()
        {
            const string input = @"
	                globalVar[43] = 1 + 3;
	                if (globalVar[43] == 4) {
                        system.Print(""success"");
                    }
                    else {
                        system.Print(""false"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ExpressionTest002()
        {
            const string input = @"
                    globalVar[43] = 0;
	                if (globalVar[43] == sin(0)) {
                        system.Print(""success"");
                    }
                    else {
                        system.Print(""false"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ExpressionTest003()
        {
            const string input = @"
                    globalVar[43] = 0;
	                if (globalVar[43] == double(0)) {
                        system.Print(""success"");
                    }
                    else {
                        system.Print(""false"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ExpressionTest004()
        {
            const string input = @"
                    localFlag[43] = !true;
	                if (globalVar[43] == false) {
                        system.Print(""success"");
                    }
                    else {
                        system.Print(""false"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ExpressionTest005()
        {
            const string input = @"
                    localVar[43] = system.rand(10,20);
                    if(localVar[43] <= 20 && localVar[43] >= 10)
                    {
                        system.Print(""correct"");
                    }
                    else
                    {
                        system.Print(""error"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void ExpressionTest007()
        {
            const string input = @"
                    localVar[43] = system.rand(10,20);
                    localVar[44] = localVar[43];
                    if(localVar[43] == localVar[44])
                    {
                        system.Print(""correct"");
                    }
                    else
                    {
                        system.Print(""error"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void IncrementTest001()
        {
            const string input = @"
                    localVar[43] = 5;
                    localVar[43]++;
                    if(localVar[43] == 6)
                    {
                        system.Print(""correct"");
                    }
                    else
                    {
                        system.Print(""error"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(output.LocalState.Var[43] == 6);
        }

        [Test]
        public void IncrementTest002()
        {
            const string input = @"
                    globalVar[43] = 5;
                    globalVar[43]++;
                    if(globalVar[43] == 6)
                    {
                        system.Print(""correct"");
                    }
                    else
                    {
                        system.Print(""error"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(m_Engine.GlobalState.Var[43] == 6);
        }

        [Test]
        public void DecrementTest001()
        {
            const string input = @"
                    globalVar[43] = 5;
                    globalVar[43]--;
                    if(globalVar[43] == 4)
                    {
                        system.Print(""correct"");
                    }
                    else
                    {
                        system.Print(""error"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(m_Engine.GlobalState.Var[43] == 4);
        }

        [Test]
        public void DecrementTest002()
        {
            const string input = @"
                    localVar[43] = 5;
                    localVar[43]--;
                    if(localVar[43] == 4)
                    {
                        system.Print(""correct"");
                    }
                    else
                    {
                        system.Print(""error"");
                    }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(output.LocalState.Var[43] == 4);
        }
    }
}
