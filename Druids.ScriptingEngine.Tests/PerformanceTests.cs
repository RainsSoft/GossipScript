using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Exceptions;
using JRPG.ScriptingEngine.Tests.Module;
using NUnit.Framework;

namespace JRPG.ScriptingEngine.Tests
{
    public class PerfTestObject
    {
        public Int32 A { get; set; }
        public Int32 B { get; set; }
        public Int32 C { get; set; }
    }

    [TestFixture]
    public class PerformanceTest
    {
        private readonly GossipVM m_Vm;
        private PerfTestObject obj = new PerfTestObject();
        private Customer m_Customer1 = new Customer();
        private Customer m_Customer2 = new Customer();

        const int n = 100000;

        public PerformanceTest()
        {
            m_Vm = new GossipVM();
            m_Vm.RegisterGlobalFunction("GetCustomer", new Func<Int32, Customer>(GetCustomer));

            m_Vm.RegisterType(typeof(Customer));
            m_Vm.RegisterType(typeof(Account));
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
        public void True()
        {
            Assert.True(true);
        }

        [Test]
        public void PerformanceTest000_Base()
        {
            const string input = @"localVar[0] = 4";
            var result = m_Vm.CompileScript(input);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(result);
            }
            sw.Stop();

            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PerformanceTest000_Base_warm()
        {
            const string input = @"localVar[0] = 4";
            var result = m_Vm.CompileScript(input);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(result);
            }
            sw.Stop();

            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PerformanceTest001()
        {
            const string input = @"localVar[0] = 4*5/3+1+cos(12)";
            var result = m_Vm.CompileScript(input);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(result);
            }
            sw.Stop();
            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms",sw.ElapsedMilliseconds);
        }

        [Test]
        public void PerformanceTest002()
        {
            const string input = @"localVar[0] = 4*5/3+1+cos(12);
                                   localVar[1] = 4*5/3+1+cos(12); 
                                   localVar[2] = localVar[0] + localVar[1];";
            var result = m_Vm.CompileScript(input);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(result);
            }
            sw.Stop();
            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PerformanceTest003()
        {
            const string input = @"localVar[0] = 4*5/3+1+cos(12)+exp(2)+cos(2)*cos(2)*cos(2)";
            var output = m_Vm.CompileScript(input);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(output);
            }
            sw.Stop();
            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PerformanceTest005()
        {
            const string input = @"localVar[0] = 4*5; if (localVar[0] >= 0) { eval(2+5); } else { eval(2+6); }";
            var output = m_Vm.CompileScript(input);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(output);
            }
            sw.Stop();

            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms", sw.ElapsedMilliseconds);
        }

        [Test]
        public void PerformanceTest006()
        {
            const string input = @"localVar[0] = 4*5";
            var result = m_Vm.CompileScript(input);

            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(result);
            }
            sw.Stop();
            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms", sw.ElapsedMilliseconds);
        }

  

        [Test]
        public void PerformanceTest007()
        {
            const string input = @"
                    ref[0] = GetCustomer(0);
                    ref[1] = GetCustomer(1);
                    if (ref[0] != ref[1]) {
                        system.Print(""success"");
                        localVar[0] = 222;
                    }
                ";

            var result = m_Vm.CompileScript(input);
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(result);
            }
            sw.Stop();
            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms", sw.ElapsedMilliseconds);

        }

        [Test]
        public void PreformanceTest008()
        {
            const string input = @"localVar[0] = 2* GetCustomer(1).Account.Method3(5+2, GetCustomer(1).Id) - 1;";
            var result = m_Vm.CompileScript(input);
            var sw = new Stopwatch();
            sw.Start();
            for (int i = 0; i < n; i++)
            {
                m_Vm.RunScript(result);
            }
            sw.Stop();
            Console.WriteLine("Number of Iterations: {0}", n);
            Console.WriteLine("Time elapsed: {0}ms", sw.ElapsedMilliseconds);
        }
    }
}
