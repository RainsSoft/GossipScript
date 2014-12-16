using System;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;

namespace JRPG.ScriptingEngine.Tests.Module
{
    public class CustomActionNode3 : ICustomActionNode
    {
        public CustomActionNode3()
        {
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            var customer = CustomActionContext.ActionTarget as Customer;
            if (customer == null)
            {
                Console.WriteLine("Target is not a customer");
                return ScriptResult.Continue;
            }

            var parameter = CustomActionContext.Parameters[0] as String;
            var parameter2 = CustomActionContext.Parameters[1];

            Console.WriteLine("Update Custom Action");
            Console.WriteLine("Source :" + customer.Name);
            Console.WriteLine("Parameter 1 :" + parameter);
            Console.WriteLine("Parameter 2 :" + parameter2);
            return ScriptResult.Continue;
        }

        public void OnEnter(ScriptExecutionContext context)
        {
            Console.WriteLine("Enter Custom Action");
        }

        public void OnExit(ScriptExecutionContext context)
        {
            Console.WriteLine("Exit Custom Action");
        }

        public IActionNode Next { get; set; }

        public string CommandName { get { return "Custom Action"; }}
        
        public bool Recall { get; private set; }

        public CustomActionContext CustomActionContext { get; set; }
    }
}

