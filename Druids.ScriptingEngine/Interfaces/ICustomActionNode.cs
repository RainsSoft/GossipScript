using Druids.ScriptingEngine;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine.Interfaces
{
    public interface ICustomActionNode : IActionNode
    {
        CustomActionContext CustomActionContext { get; set; }
    }
}