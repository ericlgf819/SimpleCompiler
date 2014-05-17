using System.Text;

namespace SimpleCompilerLib.StateMachine
{
    public class PatternUnit
    {
        public StringBuilder Result = new StringBuilder();
        public int iStartIndex = -1;

        public string Pattern = string.Empty;
        public bool PatternToMatchAny = false;

        public string[] Triggers = new string[] { };
        public bool TriggerByAny = false;

        public string[] Terminators = new string[] { };
        public bool TerminateByAny = false;
        public bool TerminateByEOF = false;

        public string[] InvalidTokens = new string[] { };

        public UnitStatus Status = UnitStatus.TRIGGERING;

        public enum UnitStatus
        {
            TRIGGERING,
            RUNNING,
            TERMINATING,
            SUCCESS,
            FAILAURE
        }

        private MatchUnit TriggerMatchUnit = new MatchUnit();
        private MatchUnit TerminatedMatchUnit = new MatchUnit();
        private MatchUnit RunningMatchUnit = new MatchUnit();
        private MatchUnit InvalidMatchUnit = new MatchUnit();

        public void Reset()
        {
            this.Status = UnitStatus.TRIGGERING;
            this.Result.Clear();

            this.TriggerMatchUnit.Reset();
            this.TerminatedMatchUnit.Reset();
            this.RunningMatchUnit.Reset();
            this.InvalidMatchUnit.Reset();
        }

        public PatternUnit Init()
        {
            this.TriggerMatchUnit.MatchTargets = this.Triggers;
            this.TerminatedMatchUnit.MatchTargets = this.Terminators;
            this.RunningMatchUnit.MatchTargets = new string[] { this.Pattern };
            this.InvalidMatchUnit.MatchTargets = this.InvalidTokens;
            return this;
        }

        public void Step(string Input, int InputIndex)
        {
            this.InvalidTokenScan(Input, InputIndex);
            switch (this.Status) {
                case UnitStatus.TRIGGERING: {
                    this.TriggerStage(Input, InputIndex);
                    break;
                }
                case UnitStatus.RUNNING: {
                    this.RunningStage(Input, InputIndex);
                    break;
                }
                case UnitStatus.TERMINATING: {
                    this.TerminatingStage(Input, InputIndex);
                    break;
                }
            }
        }

        private void TriggerStage(string Input, int InputIndex)
        {
            if (this.TriggerByAny) {
                this.iStartIndex = InputIndex;
                this.Status = UnitStatus.RUNNING;
                this.RunningStage(Input, InputIndex);
                return;
            }

            this.TriggerMatchUnit.Step(Input, InputIndex);

            if (this.TriggerMatchUnit.IsMatch) {
                this.iStartIndex = InputIndex + 1;
                this.Status = UnitStatus.RUNNING;
            }

            if (!this.TriggerMatchUnit.IsRunning && !this.TriggerMatchUnit.IsMatch) {
                this.Status = UnitStatus.FAILAURE;
            }
        }

        private void RunningStage(string Input, int InputIndex)
        {
            if (this.PatternToMatchAny && this.TerminateByAny) {
                this.Result.Append(Input[InputIndex]);
                this.Status = UnitStatus.SUCCESS;
                return;
            }

            if (this.PatternToMatchAny) {
                this.TerminatedMatchUnit.Step(Input, InputIndex);

                if (!this.TerminatedMatchUnit.IsRunning && !this.TerminatedMatchUnit.IsMatch) {
                    this.Result.Append(Input[InputIndex]);
                    this.TerminatedMatchUnit.Reset();
                }

                if (this.TerminatedMatchUnit.IsRunning) {
                    this.Status = UnitStatus.TERMINATING;
                }

                if (this.TerminatedMatchUnit.IsMatch) {
                    this.Status = UnitStatus.SUCCESS;
                }

                if (this.TerminateByEOF && InputIndex == Input.Length - 1) {
                    this.Status = UnitStatus.SUCCESS;
                }
                return;
            }

            this.RunningMatchUnit.Step(Input, InputIndex);
            if (this.RunningMatchUnit.IsRunning || this.RunningMatchUnit.IsMatch) {
                this.Result.Append(Input[InputIndex]);
            }

            if (this.RunningMatchUnit.IsMatch) {
                this.Status = UnitStatus.TERMINATING;

                if (this.TerminateByAny) {
                    this.Status = UnitStatus.SUCCESS;
                }
            }

            if (!this.RunningMatchUnit.IsRunning && !this.RunningMatchUnit.IsMatch) {
                this.Status = UnitStatus.FAILAURE;
            }
        }

        private void TerminatingStage(string Input, int InputIndex)
        {
            if (this.TerminateByAny) {
                this.Status = UnitStatus.SUCCESS;
                return;
            }

            if (this.TerminateByEOF) {
                if (InputIndex != Input.Length - 1)
                    this.Status = UnitStatus.FAILAURE;
                else
                    this.Status = UnitStatus.SUCCESS;
                return;
            }

            this.TerminatedMatchUnit.Step(Input, InputIndex);

            if (this.TerminatedMatchUnit.IsMatch) {
                this.Status = UnitStatus.SUCCESS;
            }

            if (!this.TerminatedMatchUnit.IsRunning && !this.TerminatedMatchUnit.IsMatch) {
                this.Status = UnitStatus.FAILAURE;
            }
        }

        private void InvalidTokenScan(string Input, int InputIndex)
        {
            this.InvalidMatchUnit.Step(Input, InputIndex);
            if (this.InvalidMatchUnit.IsMatch) {
                this.Status = UnitStatus.FAILAURE;
            }
            if (!this.InvalidMatchUnit.IsRunning && !this.InvalidMatchUnit.IsMatch) {
                this.InvalidMatchUnit.Reset();
            }
        }
    }
}
