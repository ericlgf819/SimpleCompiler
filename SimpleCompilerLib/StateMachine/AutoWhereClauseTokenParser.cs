using System.Collections.Generic;

namespace SimpleCompilerLib.StateMachine
{
    public class AutoWhereClauseTokenParser : TokenParserBase
    {
        private List<PatternUnit> _patternlist = null;
        protected override List<PatternUnit> PatternList
        {
            get
            {
                if (null == this._patternlist) {
                    this._patternlist = new List<PatternUnit>() {
                        // Column Name Pattern
                        new PatternUnit() {
                            TriggerByAny = true,
                            PatternToMatchAny = true,
                            Terminators = new string[] { "[" },
                            InvalidTokens = new string[] { "]", "|" }
                        },
                        // Interal Operator
                        new PatternUnit() {
                            Triggers = new string[] { "[" },
                            PatternToMatchAny = true,
                            Terminators = new string[] { "]" },
                            InvalidTokens = new string[] { "[", "|", "]" }
                        }
                        // 
                        // External Operator
                        
                    };
                }
                return this._patternlist;
            }
        }
    }
}
