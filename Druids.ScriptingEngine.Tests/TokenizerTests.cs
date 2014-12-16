using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Druids.ScriptingEngine.Compiler;
using Druids.ScriptingEngine.Enums;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    class TokenizerTests
    {
        GossipLexer tokenizer = new GossipLexer();

        public TokenizerTests()
        {
            tokenizer.AddDefinition(new TokenDefinition(TokenType.HostFunctionCall, OperationType.FunctionCall, new Regex("say")));
            tokenizer.AddDefinition(new TokenDefinition(TokenType.HostFunctionCall, OperationType.FunctionCall, new Regex("multi")));
        }

        [Test]
        public void Pass()
        {
            Assert.IsTrue(true);
        }

        [Test]
        public void tokenize_001()
        {
            var input = @"
	                globalVar[43] = 1;
	                globalVar[44] = 1;
	                globalVar[45] = 2;

	                if (globalVar[43] + globalVar[44] == globalVar[45])
	                {
		                say(""Correct"");
	                }
	                else
	                {
		                say(""Logic Error"");
	                }
                ";
            
            var results = tokenizer.Tokenize(input).ToList();
            Assert.IsTrue(results.Count == 36);
        }

        [Test]
        public void tokenize_002()
        {
            var input = @"
	                globalVar[43] = 7.5;
	                globalVar[44] = 0;
	                globalVar[45] = -2.5;

	                if (globalVar[44] - -globalVar[45] == globalVar[43] / 3)
	                {
		                say(""Correct"");
	                }
	                else
	                {
		                say(""Logic Error"");
	                }
                ";

            var results = tokenizer.Tokenize(input).ToList();
            Assert.IsTrue(results.Count == 40);
        }

        [Test]
        public void tokenize_003()
        {
            var input = @"
                    if (2.5 * 2.50 == 6.25) {
		                say(""Correct"");
	                }";

            var results = tokenizer.Tokenize(input).ToList();
            Assert.IsTrue(results.Count == 16);
        }

        [Test]
        public void tokenize_004()
        {
            var input = @"
                    if (2.5 * 2.50 != -6.25) {
		                multi(""Correct"",5,    true,""test"");
	                }";

            var results = tokenizer.Tokenize(input).ToList();
            Assert.IsTrue(results.Count == 23);
        }

        [Test]
        public void tokenize_005()
        {
            var input = @"multi(""Correct"",5,    true,""test"");";

            var results = tokenizer.Tokenize(input).ToList();
            Assert.IsTrue(results.Count == 12);
        }
    }
}
