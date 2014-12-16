using System.Collections.Generic;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine.Compiler
{
    public class Precompiler
    {
        public CompilationContext CompilationContext { get; set; }


        public List<Token> ApplyPrecompileSemantic(List<Token> tokenlist)
        {
            var rv = new List<Token>();

            // Determine object setters
            for (int i = 0; i < tokenlist.Count; i++)
            {
                Token token = tokenlist[i];

                if (token.Type == TokenType.ReferencePropertyGet)
                {
                    if (i + 1 < tokenlist.Count && tokenlist[i + 1].Type == TokenType.Assignment)
                        token.Type = TokenType.ReferencePropertySet;
                }

                if (token.Type == TokenType.ModulePropertyGet)
                {
                    if (i + 1 < tokenlist.Count && tokenlist[i + 1].Type == TokenType.Assignment)
                        token.Type = TokenType.ModulePropertySet;
                }

                rv.Add(token);
            }

            return rv;
        }
    }
}