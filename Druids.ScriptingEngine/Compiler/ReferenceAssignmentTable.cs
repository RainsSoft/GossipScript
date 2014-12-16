using System;
using System.Collections.Generic;
using Druids.ScriptingEngine.Exceptions;
using Druids.ScriptingEngine.Exceptions;

namespace Druids.ScriptingEngine.Compiler
{
    public class ReferenceAssignment
    {
        public ReferenceAssignment(Type type)
        {
            Type = type;
        }

        public Type Type { get; set; }
    }

    /// <summary>
    ///     Keeps track of assignments made to ref[x] locals during compilation.
    /// </summary>
    public class ReferenceAssignmentTable
    {
        private readonly Dictionary<Int32, ReferenceAssignment> m_ReferenceAssignments =
            new Dictionary<int, ReferenceAssignment>();

        public void TrackReference(Int32 id, Type type)
        {
            if (m_ReferenceAssignments.ContainsKey(id))
            {
                // Check types are the same
                ReferenceAssignment assignment = m_ReferenceAssignments[id];
                if (type != assignment.Type)
                    throw new GossipScriptException("Reference cannot be re-assigned a different type");
            }
            else
            {
                m_ReferenceAssignments.Add(id, new ReferenceAssignment(type));
            }
        }

        public ReferenceAssignment GetAssignment(int id)
        {
            return m_ReferenceAssignments[id];
        }

        public bool HasAssignment(int id)
        {
            return m_ReferenceAssignments.ContainsKey(id);
        }
    }
}