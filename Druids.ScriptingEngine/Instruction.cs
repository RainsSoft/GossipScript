using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine
{
    public class Instruction
    {
        public double Data;
        public TokenType TokenType;

        public bool IsNumber()
        {
            return (TokenType == TokenType.DecimalLiteral);
        }
    }
}