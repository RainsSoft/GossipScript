using System;
using System.Collections.Generic;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine;

namespace Druids.ScriptingEngine
{
    public class Expression
    {
        public Expression()
        {
            EvaluationStack = new Stack<Double>(16);
            Instructions = new List<Instruction>(16);
            StringEvaluationStack = new Stack<string>(16);
            ReferenceObjectEvaluationStack = new Stack<object>(16);
        }

        /// <summary>
        ///     A sequence of instructions to execute in RPN form
        /// </summary>
        public List<Instruction> Instructions { get; set; }

        // TODO Could convert semantic tokens to instructions for more lightweight

        /// <summary>
        ///     The stack used by the evaluation process
        /// </summary>
        public Stack<Double> EvaluationStack { get; set; }

        /// <summary>
        ///     String stack used by the evaluation process
        /// </summary>
        public Stack<String> StringEvaluationStack { get; set; }

        /// <summary>
        ///     Stack for ref types used by the evaluation process
        /// </summary>
        public Stack<Object> ReferenceObjectEvaluationStack { get; set; }

        /// <summary>
        ///     The type of value returned from this expression
        /// </summary>
        public GossipType ReturnType { get; set; }

        /// <summary>
        ///     The type as expressed in the host language
        /// </summary>
        public Type HostReturnType { get; set; }

        /// <summary>
        ///     Additonal Data field if required
        /// </summary>
        public int HostData { get; set; }

        /// <summary>
        ///     Target field, if required
        /// </summary>
        public int Target { get; set; }
    }
}