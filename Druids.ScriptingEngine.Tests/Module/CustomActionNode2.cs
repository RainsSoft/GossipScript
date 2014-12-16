using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;

namespace JRPG.ScriptingEngine.Tests.Module
{
    class CustomActionNode2 : ICustomActionNode
    {
        public CustomActionNode2()
        {
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            var customer = CustomActionContext.ActionTarget as Customer;
            if (customer == null)
            {
                Console.WriteLine("Target is not a customer");
        //        return ScriptResult.Continue;
            }

            var parameter = CustomActionContext.Parameters[0] as String;

            Console.WriteLine("Update Custom Action");
            if (customer != null)
            {
                Console.WriteLine("Source :" + customer.Name);
            }
            Console.WriteLine("Parameter :" + parameter);
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
