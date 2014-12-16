using System;
using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Interfaces;

namespace JRPG.ScriptingEngine.Tests.Module
{
    public class CustomActionNode : ICustomActionNode
    {
        public CustomActionNode()
        {
        }

        public ScriptResult Update(ScriptExecutionContext context)
        {
            var customer = CustomActionContext.ActionTarget as Customer;

            Console.WriteLine("Update Custom Action");
            if (customer == null)
            {
                Console.WriteLine("Target is not a customer");   
            }
            else
            {
                Console.WriteLine("Source :" + customer.Name);
            }
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
