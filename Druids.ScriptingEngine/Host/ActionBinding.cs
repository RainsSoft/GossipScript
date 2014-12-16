using System;
using System.Reflection;
using Druids.ScriptingEngine.Compiler;
using Druids.ScriptingEngine.Enums;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Helper;
using Druids.ScriptingEngine.Interfaces;

namespace Druids.ScriptingEngine.Host
{
    public class ActionBinding
    {
        private readonly Type m_ActionType;

        private readonly Int32 m_NumParameters;

        private readonly TypeAdapter[] m_ParameterInfo;

        public ActionBinding(Int32 id, String name, Type actionType, MethodInfo method)
        {
            Id = id;
            Name = name;
            m_ActionType = actionType;

            if (!method.IsStatic)
                throw new GossipScriptException("Method must be static");

            if (method.ReturnType != typeof (void))
                throw new GossipScriptException("Method must return IActionNode");

            ParameterInfo[] parameters = method.GetParameters();
            m_NumParameters = parameters.Length;
            m_ParameterInfo = new TypeAdapter[NumParameters];
            for (int i = 0; i < ParameterInfo.Length; i++)
            {
                GossipType convertedType = TypeConverter.ConvertHostType(parameters[i].ParameterType);
                ParameterInfo[i] = new TypeAdapter(convertedType, parameters[i].ParameterType);
            }
        }

        public int Id { get; set; }

        public string Name { get; set; }

        public TypeAdapter[] ParameterInfo
        {
            get { return m_ParameterInfo; }
        }

        public int NumParameters
        {
            get { return m_NumParameters; }
        }

        public ICustomActionNode CreateActionNode()
        {
            return (ICustomActionNode) Activator.CreateInstance(m_ActionType);
        }
    }
}