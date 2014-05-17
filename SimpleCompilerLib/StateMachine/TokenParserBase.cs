using System.Collections.Generic;

namespace SimpleCompilerLib.StateMachine
{
    public abstract class TokenParserBase
    {
        public List<TokenUnit> GrammarList = new List<TokenUnit>();

        protected virtual List<PatternUnit> PatternList { get; set; }

        public void Parse(string Input)
        {
            for (int i = 0; i < Input.Length; ++i) {
                foreach (var pattern in this.PatternList) {
                    pattern.Step(Input, i);
                    switch (pattern.Status) {
                        case PatternUnit.UnitStatus.FAILAURE: {
                            pattern.Reset();
                            break;
                        }
                        case PatternUnit.UnitStatus.SUCCESS: {
                            this.GrammarList.Add(new TokenUnit() { 
                                Token = pattern.Result.ToString(), 
                                iStartIndex = pattern.iStartIndex 
                            });
                            pattern.Reset();
                            break;
                        }
                    }
                }
            }
        }
    }

    public struct TokenUnit
    {
        public string Token;
        public int iStartIndex;
    }
}
