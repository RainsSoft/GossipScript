using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    internal class StringTests
    {
        private GossipVM m_Engine = new GossipVM();

        public StringTests()
        {

        }

        [Test]
        public void IsTrue()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void StringTest001()
        {
            const string input = @"
	                globalStr[43] = """";
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(m_Engine.GlobalState.Str[43] == "");

        }

        [Test]
        public void StringTest002()
        {
            const string input = @"
	                globalStr[43] = ""Hello"";
                    localStr[16] = globalStr[43];
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(output.LocalState.Str[16] == "Hello");
            Assert.IsTrue(m_Engine.GlobalState.Str[43] == "Hello");

        }

        [Test]
        public void StringTest003()
        {
            const string input = @"
	                globalStr[43] = ""Hello"";
                    localStr[16] = globalStr[43];
	                if (globalStr[43] == ""Hello"") { system.Print(""success""); } else { system.Print(""false""); }
	                if (globalStr[43] != ""Hellop"") { system.Print(""success""); } else { system.Print(""false""); }
	                if (localStr[16] == ""Hello"") { system.Print(""success""); } else { system.Print(""false""); }
	                if (localStr[16] != ""Hellop"") { system.Print(""success""); } else { system.Print(""false""); }
	                if (""Hello"" == ""Hello"") { system.Print(""success""); } else { system.Print(""false""); }
	                if (""Hello"" != ""Hellop"") { system.Print(""success""); } else { system.Print(""false""); }
                    if (globalStr[43] == localStr[16]) { system.Print(""success""); } else { system.Print(""false""); }
                    if (globalStr[43] == globalStr[43]) { system.Print(""success""); } else { system.Print(""false""); }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }


        [Test]
        public void StringTest004()
        {
            const string input = @"
	                globalStr[43] = ""Hello"";
                    localStr[16] = globalStr[43];
	                if (globalStr[43] != ""H"" + ""ello"" + ""p"") { system.Print(""success""); } else { system.Print(""false""); }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }
        
        [Test]
        public void StringTest005()
        {
            const string input = @"
	                globalStr[43] = ""Hellop"";
                    localStr[16] = globalStr[43];
	                if (globalStr[43] == (""H"" + ""ello"" + ""p"")) { system.Print(""success""); } else { system.Print(""false""); }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void StringTest006()
        {
            const string input = @"
	                globalStr[43] = ""Hello"";
                    localStr[16] = globalStr[43];
	                if (globalStr[43] == ""Hello"") { system.Print(""success""); } else { system.Print(""false""); }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void StringFormatTest008()
        {
            const string input = @"
                    globalStr[42] = 5 + ""test"" + 5 ;
                    localVar[16] = 16;
	                globalStr[43] = globalStr[42] + localVar[16];
	                if (globalStr[43] == ""5test516"") { system.Print(""success""); } else { system.Print(""false""); }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(m_Engine.GlobalState.Str[43] == "5test516");
        }
    }
}
