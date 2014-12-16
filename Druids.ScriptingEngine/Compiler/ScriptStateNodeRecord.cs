using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Druids.ScriptingEngine.Compiler
{
    public class ScriptStateNodeRecord
    {
        public String Name { get; set; }

        public String Text { get; set; }

        public ScriptStateNodeRecord()
        {
            
        }

        public ScriptStateNodeRecord(String name, String text)
        {
            Name = name;
            Text = text;
        }
    }
}
