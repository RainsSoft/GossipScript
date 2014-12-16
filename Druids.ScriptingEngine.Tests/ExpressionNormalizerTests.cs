using System;
using System.Diagnostics;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using JRPG.ScriptingEngine.Tests.Module;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    [TestFixture]
    class ExpressionNormalizerTests
    {
        private Customer m_Customer1 = new Customer();
        private Customer m_Customer2 = new Customer();

        private Account m_Account1 = new Account();

        private GossipVM m_Vm;


        public ExpressionNormalizerTests()
        {
            m_Customer1.Id = 0;
            m_Customer2.Id = 1;

            m_Vm = new GossipVM();
            m_Vm.RegisterGlobalFunction("GetAccount", new Func<Account>(GetAccount));
            m_Vm.RegisterGlobalFunction("GetCustomer", new Func<Int32, Customer>(GetCustomer));

            m_Vm.RegisterType(typeof(Customer));
            m_Vm.RegisterType(typeof(Account));
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

        [Test]
        public void Test001()
        {
            var input = @"GetCustomer(0).Id;";          // Mixed fix
            var normalInput = @".Id(GetCustomer(0));";  // Allready Normalized

            var exp1 = m_Vm.ParseExpression(input, true);
            var exp2 = m_Vm.ParseExpression(normalInput,false);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test002()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(0)) == 5;";  
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Id == 5;";          
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test003()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(0)) + 5;";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Id + 5;";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test004()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(0)) + 5 == 2;";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Id + 5 == 2;";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test005()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Credit(.Account(GetCustomer(0))) + 5 == 2;";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Account.Credit + 5 == 2;";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test006()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(0)) + 5 == 2 - .Id(GetCustomer(1));";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Id + 5 == 2 - GetCustomer(1).Id;";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test007()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(0)) == .Id(GetCustomer(1));";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Id == GetCustomer(1).Id;";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test008()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Credit(.Account(GetCustomer(0))) == .Credit(.Account(GetCustomer(1)))";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Account.Credit == GetCustomer(1).Account.Credit;";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }


        [Test]
        public void Test009()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Credit(.Account(GetCustomer(0))) + 2 == 2 - .Credit(.Account(GetCustomer(1))) + 2";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Account.Credit + 2 == 2 - GetCustomer(1).Account.Credit + 2";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test010()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(.Id(GetCustomer(0)))) == 2";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(GetCustomer(0).Id).Id == 2";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test011a()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(.Id(GetCustomer(0+1)))) == 2";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(GetCustomer(0+1).Id).Id == 2";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test011()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(.Id(GetCustomer(0+1)))) == .Id(GetCustomer(.Id(GetCustomer(1))))";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(GetCustomer(0+1).Id).Id == GetCustomer(GetCustomer(1).Id).Id";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test012()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(.Id(GetCustomer(0+1))-1)) == .Id(GetCustomer(.Id(GetCustomer(1))))";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(GetCustomer(0+1).Id - 1).Id == GetCustomer(GetCustomer(1).Id).Id";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test013()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(.Id(GetCustomer(0+1)) - .Id(GetCustomer(0+1))))==0";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(GetCustomer(0+1).Id - GetCustomer(0+1).Id).Id == 0";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test014()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(.Id(GetCustomer(0+1)) - .Id(GetCustomer(0+1)))) == .Id(GetCustomer(.Id(GetCustomer(1))))";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(GetCustomer(0+1).Id - GetCustomer(0+1).Id).Id == GetCustomer(GetCustomer(1).Id).Id";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test015()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(min(0,.Id(GetCustomer(0+1))))) == 1";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(min(0, GetCustomer(0+1).Id)).Id == 1";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }


        [Test]
        public void Test016()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(min(0, 2 - .Id(GetCustomer(0+1)) - 1))) == 1";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(min(0, 2 - GetCustomer(0+1).Id - 1)).Id == 1";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }



        [Test]
        public void Test017()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Id(GetCustomer(min(0, 5 - .Id(GetCustomer(0+1))))) == 1";
            var exp1 = m_Vm.ParseExpression(normalInput, false);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(min(0, 5 - GetCustomer(0+1).Id)).Id == 1";
            var exp2 = m_Vm.ParseExpression(input, true);

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test018()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Method1(.Account(GetCustomer(0)));";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Number);
            Assert.IsTrue(exp1.Instructions.Count == 4);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).Account.Method1();";
            var exp2 = m_Vm.ParseExpression(input, true);
            Trace.WriteLine("Exp2:Done!");

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test019()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Method2(5, .Account(GetCustomer(1)));";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Number);
            
            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(1).Account.Method2(5);";
            var exp2 = m_Vm.ParseExpression(input, true);
            Trace.WriteLine("Exp2:Done!");

            AssertEquality(exp1, exp2);
        }

        [Test]
        public void Test020()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Method3(3, 5, .Account(GetCustomer(1)));";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Number);
            var context = new ScriptExecutionContext(null, new LocalState());
            context.Vm = m_Vm;

            Trace.WriteLine("Eval A");
            var r1 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(exp1, context);
            Assert.IsTrue(r1 == 2);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(1).Account.Method3(5, 3);";
            var exp2 = m_Vm.ParseExpression(input, true);
            Trace.WriteLine("Exp2:Done!");

            AssertEquality(exp1, exp2,2);
        }

        [Test]
        public void Test021()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Method3(3+1, 5, .Account(GetCustomer(1)));";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Number);
            var context = new ScriptExecutionContext(null,new LocalState());
            context.Vm = m_Vm;

            Trace.WriteLine("Eval A");
            var r1 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(exp1, context);
            Assert.IsTrue(r1 == 1);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(1).Account.Method3(5, 3+1);";
            var exp2 = m_Vm.ParseExpression(input, true);
            Trace.WriteLine("Exp2:Done!");

            AssertEquality(exp1, exp2, 1);
        }

        [Test]
        public void Test022()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Method3(.Id(GetCustomer(1)), 5, .Account(GetCustomer(1)));";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Number);
            var context = new ScriptExecutionContext(null, new LocalState());
            context.Vm = m_Vm;

            Trace.WriteLine("Eval A");
            var r1 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(exp1, context);
            Assert.IsTrue(r1 == 4);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(1).Account.Method3(5, GetCustomer(1).Id);";
            var exp2 = m_Vm.ParseExpression(input, true);
            Trace.WriteLine("Exp2:Done!");

            AssertEquality(exp1, exp2, 4);
        }

        [Test]
        public void Test023()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".TestMethod001(GetCustomer(1)) - 1;";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Number);
            var context = new ScriptExecutionContext(null,new LocalState());
            context.Vm = m_Vm;

            Trace.WriteLine("Eval A");
            var r1 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(exp1, context);
            Assert.IsTrue(r1 == 0);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(1).TestMethod001() - 1;";
            var exp2 = m_Vm.ParseExpression(input, true);
            Trace.WriteLine("Exp2:Done!");

            AssertEquality(exp1, exp2, 0);
        }

        [Test]
        public void Test024()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".Method3(.Id(GetCustomer(1)), 5, .Account(GetCustomer(1))) - 1;";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Number);
            var context = new ScriptExecutionContext(null, new LocalState());
            context.Vm = m_Vm;

            Trace.WriteLine("Eval A");
            var r1 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(exp1, context);
            Assert.IsTrue(r1 == 3);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(1).Account.Method3(5, GetCustomer(1).Id) - 1;";
            var exp2 = m_Vm.ParseExpression(input, true);
            Trace.WriteLine("Exp2:Done!");

            AssertEquality(exp1, exp2, 3);
        }

        [Test]
        public void Test025()
        {
            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".TestMethod001(GetCustomer(0))";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Number);
            var context = new ScriptExecutionContext( null,new LocalState());
            context.Vm = m_Vm;

            Trace.WriteLine("Eval A");
            var r1 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(exp1, context);
            Assert.IsTrue(r1 == 1);

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).TestMethod001()";
            var exp2 = m_Vm.ParseExpression(input, true);
            Trace.WriteLine("Exp2:Done!");

            AssertEquality(exp1, exp2, 1);
        }

        [Test]
        [ExpectedException(typeof(GossipScriptRuntimeException))]
        public void Test026()
        {

            // Mixed fix
            Trace.WriteLine("Exp2");
            var input = @"GetCustomer(0).CustomAction();";
            var exp2 = m_Vm.ParseExpression(input, true);
            Assert.IsTrue(exp2.ReturnType == GossipType.Action);
            Trace.WriteLine("Exp2:Done!");


            // Allready Normalized
            Trace.WriteLine("Exp1");
            var normalInput = @".CustomAction(GetCustomer(0))";
            var exp1 = m_Vm.ParseExpression(normalInput, false);
            Trace.WriteLine("Exp1:Done!");

            Assert.IsTrue(exp1.ReturnType == GossipType.Action);
            var context = new ScriptExecutionContext( null, new LocalState());
            context.Vm = m_Vm;

            Trace.WriteLine("Eval A");
            var r1 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateExpression(exp1, context);
            Assert.IsTrue(r1 == GetCustomer(0));
        }


      

        public void AssertEquality(Expression exp1, Expression exp2, Nullable<double> expectedResult = null)
        {
            Trace.WriteLine("");
            Assert.IsTrue(exp1.ReturnType == exp2.ReturnType);

            for (int i = 0; i < exp1.Instructions.Count; i++)
            {
                Trace.WriteLine(exp1.Instructions[i].TokenType);
            }
            
            Trace.WriteLine("");

            for (int i = 0; i < exp2.Instructions.Count; i++)
            {
                Trace.WriteLine(exp2.Instructions[i].TokenType);
            }
            Assert.IsTrue(exp1.Instructions.Count == exp2.Instructions.Count);

            for (int i = 0; i < exp2.Instructions.Count; i++)
            {
                Trace.WriteLine(String.Format("Comp: {0} to {1}",exp1.Instructions[i].TokenType,exp2.Instructions[i].TokenType));

                Assert.IsTrue(exp1.Instructions[i].TokenType == exp2.Instructions[i].TokenType);
                //Assert.IsTrue(exp1.Instructions[i].TokenValue == exp2.Instructions[i].TokenValue);
            }

            var context = new ScriptExecutionContext(null ,new LocalState());
            context.Vm = m_Vm;

            Trace.WriteLine("Eval A");
            var r1 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(exp1, context);

            Trace.WriteLine("Eval B");
            var r2 = m_Vm.HostBridge.ExpressionEvaluator.EvaluateDouble(exp1, context);

            Assert.IsTrue(r1 == r2);

            if (expectedResult != null)
                Assert.IsTrue(r1 == expectedResult.Value);
        }
    }
}
