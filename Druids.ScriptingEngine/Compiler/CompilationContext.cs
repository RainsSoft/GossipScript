using System;
using Druids.ScriptingEngine.Host;
using Druids.ScriptingEngine.Host;

namespace Druids.ScriptingEngine.Compiler
{
    public class CompilationContext
    {
        private readonly EnumDefineTable m_EnumTable;
        private readonly EventBindingTable m_EventBindingTable;
        private readonly HostCallTable m_HostCallTable;

        private readonly ReferenceAssignmentTable m_ReferenceAssignmentTable = new ReferenceAssignmentTable();
                                                  // Keeps track of ref assignments

        private readonly StringTable m_StringTable;
        private readonly TypeBindingTable m_TypeBindingTable;

        public CompilationContext(HostCallTable hostCallTable,
                                  StringTable stringTable,
                                  EnumDefineTable enumTable,
                                  TypeBindingTable typeBindingTable,
                                  EventBindingTable eventBindingTable)
        {
            m_HostCallTable = hostCallTable;
            m_StringTable = stringTable;
            m_EnumTable = enumTable;
            m_TypeBindingTable = typeBindingTable;
            m_EventBindingTable = eventBindingTable;
        }

        public HostCallTable HostCallTable
        {
            get { return m_HostCallTable; }
        }

        public StringTable StringTable
        {
            get { return m_StringTable; }
        }

        public EnumDefineTable EnumTable
        {
            get { return m_EnumTable; }
        }

        public Type ScriptOwnerType { get; set; }

        public Type EventType { get; set; }

        public TypeBindingTable TypeBindingTable
        {
            get { return m_TypeBindingTable; }
        }

        public ReferenceAssignmentTable ReferenceAssignmentTable
        {
            get { return m_ReferenceAssignmentTable; }
        }

        public EventBindingTable EventBindingTable
        {
            get { return m_EventBindingTable; }
        }
    }
}