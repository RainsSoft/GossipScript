using System;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine.Interfaces
{
    public interface IActionNode
    {
        IActionNode Next { get; set; }

        String CommandName { get; }

        /// <summary>
        ///     Some command nodes may alter the Next command depending on events within the node.
        ///     (Such as the conditional node.)
        ///     If this flag is true the original Next ICommand node will be pushed onto a stack and then poped by the runtime
        /// </summary>
        bool Recall { get; }

        /// <summary>
        ///     Returns 1 if execution has completed. 0 if call is blocking.
        /// </summary>
        ScriptResult Update(ScriptExecutionContext context);

        /// <summary>
        ///     Called when the command node is first activated
        /// </summary>
        void OnEnter(ScriptExecutionContext context);

        /// <summary>
        ///     Called when the command node is finished
        /// </summary>
        void OnExit(ScriptExecutionContext context);
    }
}