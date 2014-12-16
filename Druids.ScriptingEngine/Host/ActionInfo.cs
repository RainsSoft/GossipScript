using System;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine.Host
{
    public class ActionInfo
    {
        public ActionInfo(String functionName, String prefix,
                          Func<GossipActionParameter, ScriptResult> onUpdate,
                          Action<GossipActionParameter> onEnter,
                          Action<GossipActionParameter> onExit)
        {
            FunctionName = functionName;
            OnUpdate = onUpdate;
            OnEnter = onEnter;
            OnExit = onExit;
            TokenName = String.Format("{0}.{1}", prefix, functionName);
        }

        public ActionInfo(String functionName,
                          Func<GossipActionParameter, ScriptResult> onUpdate,
                          Action<GossipActionParameter> onEnter,
                          Action<GossipActionParameter> onExit)
        {
            FunctionName = functionName;
            OnUpdate = onUpdate;
            OnEnter = onEnter;
            OnExit = onExit;
            TokenName = functionName;
        }

        public ActionInfo(String functionName,
                          Func<GossipActionParameter, ScriptResult> onUpdate,
                          Action<GossipActionParameter> onEnter)
        {
            FunctionName = functionName;
            OnUpdate = onUpdate;
            OnEnter = onEnter;
            TokenName = functionName;
        }

        public ActionInfo(String functionName,
            Func<GossipActionParameter, ScriptResult> onUpdate)
        {
            FunctionName = functionName;
            OnUpdate = onUpdate;
            TokenName = functionName;
        }

        public String FunctionName { get; set; }

        public string TokenName { get; set; }

        public Func<GossipActionParameter, ScriptResult> OnUpdate { get; set; }

        public Action<GossipActionParameter> OnEnter { get; set; }

        public Action<GossipActionParameter> OnExit { get; set; }
    }
}