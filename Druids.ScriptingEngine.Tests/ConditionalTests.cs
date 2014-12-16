using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Druids.ScriptingEngine;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    class ConditionalTests
    {
        readonly GossipVM m_Engine = new GossipVM();

        public ConditionalTests()
        {
            m_Engine.RegisterEnum("TEST_ONE", 1);
            m_Engine.RegisterEnum("TEST_TWO", 2);
        }

        [Test]
        public void Pass()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void Conditional001()
        {
            const string input = @"
	                globalVar[43] = 1;
	                globalVar[44] = 1;
	                globalVar[45] = 2;

	                if (globalVar[43] + globalVar[44] == globalVar[45])
	                {
		                system.Print(""Correct"");
	                }
	                else
	                {
		                system.Print(""Logic Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional002()
        {
            const string input = @"	
                    //say(""commented out"");
	                system.Print(""test script one"");
	                if (true)
	                {
		                system.Print(""conditional success"");
		                system.Print(""conditional success x2"");
	                }
	                else
	                {
			                system.Print(""logicError"");
		                system.Print(""logicError"");
	                }
		             system.Print(""Jump"");
	                //system.Print(""Jump"");
                ";

            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional003()
        {
            const string input = @"
	                localFlag[43] = true;
	                localFlag[44] = true;

	                if (localFlag[43] && localFlag[44] == true) {
		                system.Print(""Correct"");
	                }
	                else
	                {
		                system.Print(""Logic Error"");
	                }

	                if (localFlag[43] && localFlag[44]) {
		                system.Print(""Correct"");
	                }
	                else
	                {
		                system.Print(""Logic Error"");
	                }

	                if (localFlag[43] || localFlag[44]) {
		                system.Print(""Correct"");
	                }
	                else
	                {
		                system.Print(""Logic Error"");
	                }

	                if (localFlag[43] || !localFlag[44]) {
		                system.Print(""Correct"");
	                }
	                else
	                {
		                system.Print(""Logic Error"");
	                }

	                if (!localFlag[43] || !localFlag[44]) {
		                system.Print(""Logic Error"");
	                }
	                else
	                {
		                system.Print(""Correct"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional004()
        {
            const string input = @"
	                if (!true || !true)  {
		                system.Print(""Logic Error"");
	                }
	                else  {
		                system.Print(""Correct"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional005()
        {
            const string input = @"
	                if (true || (true && false)) {
		                system.Print(""Correct"");
	                }
	                else {
		                system.Print(""Logic Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional006()
        {
            const string input = @"
            system.Print(""Test script 6"");

            if (true) {
                system.Print(""conditional success"");
                system.Print(""conditional success"");
            }

            system.Print(""Jump"");
            ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional007()
        {
            const string input = @"
            system.Print(""Test script 7"");

            if (!false) {
                system.Print(""conditional success"");
                system.Print(""conditional success"");
            }

            system.Print(""Jump"");
            ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional008()
        {
            const string input = @"
            system.Print(""Test script 8"");

            globalVar[16] = 1024;

            if (globalVar[16] < 2048) {system.Print(""conditional success""); }
            if (!(globalVar[16] > 512 && globalVar[16] + 1024 == 2048)) {system.Print(""logic error""); } else { system.Print(""conditional success""); }          
            if (globalVar[16] <= 1024) { system.Print(""conditional success""); }

            system.Print(""Jump"");
            ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional009()
        {
            const string input = @"        
                system.Print(""Testing Flags"");

                globalFlag[0] = false;
	            if (globalFlag[0] == true) {
		            system.Print(""logic error"");
	            }

	            globalFlag[0] = true;

	            if (globalFlag[0] == true)  	{
		            system.Print(""conditional success"");
	            }
	            else {
		            system.Print(""logic error"");
	            }";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional010()
        {
            const string input = @"        
            if (globalVar[10] > 5)    {
		        system.Print(""Logic Error"");
	        }
	        else  {
		        system.Print(""GlobalVar 10 is less than 5"");
	        }

	        system.Print(""Assign globalVar[10] to 6"");
	        globalVar[10] = 6;

	        if (globalVar[10] >= 6) {
		        system.Print(""GlobalVar[10] is greater than or equal to six"");
	        }
	        else    {
		        system.Print(""logic error"");
	        };";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional011()
        {
            const string input = @"   
	                system.Print(""Testing Global Var increment and decrement"");
	                globalVar[66] = 10;
	                if (globalVar[66] == 128) {
		                system.Print(""Logic error"");
	                }
	                else
	                {
		                if (globalVar[66] == 10) {
			                system.Print(""Correct"");
		                }
		                else {
		                system.Print(""Logic error"");
		                }
	                }

	                globalVar[66]++;
	                if (globalVar[66] == 11)  {
	                system.Print(""correct"");
	                }
	                else   {
		                system.Print(""logic error"");
	                }

	                globalVar[66]--;
	                if (globalVar[66] == 10)   { 
	                system.Print(""correct""); 
	                } else { system.Print(""logic error""); }
            ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional012()
        {
            var random = new Random();
            var r = random.Next(0, 1024);
            string input = "  globalVar[128] = " + r;
            var output = m_Engine.CompileScript(input);
            TestHelper.RunScriptToCompletion(m_Engine, output);

            string input2 = @"if (globalVar[128] == " + r + @") { system.Print(""success""); } else { system.Print(""fail""); }";
            var output2 = m_Engine.CompileScript(input2);
            TestHelper.RunScriptToCompletion(m_Engine, output2);
        }


        [Test]
        public void Conditional013()
        {
            const string input = @"   
	            system.Print(""Testing local variables increment and decrement"");
	            if (localVar[10] == 0) {
		            system.Print(""Success"");
	            }
	            else {
		            system.Print(""Logic Error"");
	            }

	            localVar[10]++;
	            if (localVar[10] == 1) {
		            system.Print(""Success"");
	            }
	            else {
		            system.Print(""Logic error"");
	            }

	            localVar[10]--;
	            if (localVar[10] == 0) {
		            system.Print(""Success"");
	            } else { system.Print (""Logic error""); }

	            localVar[10] = 1;
	            if (localVar[10] == 1) {
		            system.Print(""Success"");
	            }
	            else {
		            system.Print(""Logic error"");
	            }
            ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional014()
        {
            const string input = @"
                    localVar[22] = 10;
	                if (localVar[22] >= system.rand(5,10))  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional015()
        {
            const string input = @"
                    localVar[22] = 10;
	                if (localVar[22] >= system.rand(5, localVar[22]-2))  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional016()
        {
            const string input = @"
                    localVar[22] = 10;
                    localVar[23] = 20;
	                if (localVar[23] == localVar[22]+10)  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional017()
        {
            const string input = @"
                    localVar[22] = 10;
	                if (20 == localVar[22]+10)  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional018()
        {
            const string input = @"
                    localVar[22] = 10;
	                if (localVar[22]+10 == 20)  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional019()
        {
            const string input = @"
                    localVar[22] = 10;
	                if (localVar[22] == 20-10)  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional020()
        {
            const string input = @"
	                if (""a"" + ""b"" == ""a"" + ""b"")  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional021()
        {
            const string input = @"
	                if (""a"" + ""b"" + 5 == ""a"" + ""b5"")  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional022()
        {
            const string input = @"
	                if (5 + ""a"" + ""b"" + 5 == ""5a"" + ""b5"")  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional023()
        {
            
            const string input = @"
                    localVar[1] = 1;
                    localVar[TEST_ONE] = 4;
	                if (localVar[1] == 4)  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(output.LocalState.Var[1] == 4);
        }


        [Test]
        public void Conditional024()
        {

            const string input = @"
                    globalVar[1] = 1;
                    globalVar[TEST_ONE] = 4;
	                if (globalVar[1] == 4)  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(m_Engine.GlobalState.Var[1] == 4);
        }

        [Test]
        public void Conditional025()
        {

            const string input = @"
                    localFlag[1] = false;
                    localFlag[TEST_ONE] = true;
	                if (localFlag[1] == true)  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(output.LocalState.Flags[1] == true);
        }

        [Test]
        public void Conditional026()
        {

            const string input = @"
                    localFlag[1] = false;
                    localFlag[TEST_ONE] = true;
	                if (localFlag[1])  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(output.LocalState.Flags[1] == true);
        }

        [Test]
        public void Conditional027()
        {

            const string input = @"
                    globalFlag[1] = false;
                    globalFlag[TEST_ONE] = true;
	                if (globalFlag[1])  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(m_Engine.GlobalState.Flags[1] == true);
        }

        [Test]
        public void Conditional028()
        {

            const string input = @"
                    globalStr[1] = """";
                    globalStr[TEST_ONE] = ""test"";
	                if (globalStr[TEST_ONE] == ""test"")  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(m_Engine.GlobalState.Str[1] == "test");
        }

        [Test]
        public void Conditional029()
        {
            const string input = @"
                    localStr[1] = """";
                    localStr[TEST_ONE] = ""test"";
	                if (localStr[1] == ""test"")  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);

            Assert.IsTrue(output.LocalState.Str[1] == "test");
        }

        [Test]
        public void Conditional030()
        {
            const string input = @"
	                if (localStr[0] == localStr[1])  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional031()
        {
            const string input = @"
	                if (localStr[TEST_ONE] == localStr[TEST_ONE])  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional032()
        {
            const string input = @"
	                if (localVar[TEST_ONE] == localVar[TEST_ONE])  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional033()
        {
            const string input = @"
	                if (globalVar[TEST_ONE] == globalVar[TEST_ONE])  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional034()
        {
            const string input = @"
	                if (globalFlag[TEST_ONE] == globalFlag[TEST_ONE])  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional035()
        {
            const string input = @"
	                if (globalFlag[TEST_ONE] == globalFlag[TEST_ONE])  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
        }

        [Test]
        public void Conditional036()
        {
            const string input = @"
	                if (globalFlag[TEST_ONE] == globalFlag[TEST_ONE])  {
                        if (globalFlag[TEST_ONE] != globalFlag[TEST_ONE])  {
		                    system.Print(""Error"");
                        }
                        else
                        {
                            if (true || false)
                            {
                                system.Print(""Correct"");
                                localVar[0] = 6;
                            }
                        }
	                }
	                else  {
		                system.Print(""Error"");
	                }
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
            Assert.IsTrue(output.LocalState.Var[0] == 6);
        }

        [Test]
        public void Conditional037()
        {
            const string input = @"
                    localVar[22] = 10;
                    localVar[23] = 20;
	                if (localVar[23] == localVar[22]+10)  {
		                system.Print(""Correct"");
	                }
	                else  {
		                system.Print(""Error"");
	                }
                    localVar[25] = 20;
                ";

            var output = m_Engine.CompileScript(input);
            Console.WriteLine("Compiled");

            TestHelper.RunScriptToCompletion(m_Engine, output);
            Assert.IsTrue(output.LocalState.Var[25] == 20);
        }
    }
}
