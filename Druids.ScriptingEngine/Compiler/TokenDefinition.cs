using System.Text.RegularExpressions;
using Druids.ScriptingEngine.Enums;

namespace Druids.ScriptingEngine.Compiler
{
    public class TokenDefinition
    {
        private readonly TokenDiscardPolicy m_DiscardPolicy;
        private readonly OperationType m_OperationType;

        public TokenDefinition(TokenType type, OperationType operationType, Regex regex)
            : this(type, operationType, regex, TokenDiscardPolicy.Keep)
        {
        }

        public TokenDefinition(TokenType type, OperationType operationType, Regex regex,
                               TokenDiscardPolicy discardPolicy)
        {
            m_OperationType = operationType;
            m_DiscardPolicy = discardPolicy;
            Type = type;
            Regex = regex;
        }

        public Regex Regex { get; private set; }
        public TokenType Type { get; private set; }

        public OperationType OperationType
        {
            get { return m_OperationType; }
        }

        public TokenDiscardPolicy DiscardPolicy
        {
            get { return m_DiscardPolicy; }
        }
    }
}