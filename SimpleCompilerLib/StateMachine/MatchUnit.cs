using System.Collections.Generic;

namespace SimpleCompilerLib.StateMachine
{
    public class MatchUnit
    {
        public string[] MatchTargets = null;
        
        public bool IsRunning = false;
        public bool IsMatch = false;

        private List<string> MatchCandidates = new List<string>();
        private int iStep = 0;

        public void Reset()
        {
            this.iStep = 0;
            this.IsRunning = this.IsMatch = false;
            this.MatchCandidates.Clear();
        }

        public void Step(string Input, int InputIndex)
        {
            if (!this.IsRunning && !this.IsMatch)
                this.MatchCandidates.AddRange(this.MatchTargets);

            this.IsRunning = true;

            // running
            List<string> FailureCandidates = new List<string>();
            foreach (var candidate in this.MatchCandidates) {
                if (candidate.Length == this.iStep + 1 
                        && candidate[this.iStep] == Input[InputIndex]) {
                    this.IsMatch = true;
                    this.IsRunning = false;
                    return;
                }

                if (this.iStep < candidate.Length && candidate[this.iStep] != Input[InputIndex]) {
                    FailureCandidates.Add(candidate);
                }
            }

            // Remove unnecessary candidates
            this.MatchCandidates.RemoveAll((item) => {
                foreach (var fail in FailureCandidates) {
                    if (fail == item)
                        return true;
                }
                return false;
            });

            // failure
            if (0 == this.MatchCandidates.Count) {
                this.IsMatch = false;
                this.IsRunning = false;
                return;
            }

            // increase step
            this.iStep += 1;
        }
    }
}
