namespace JsonTestAdapter.Helpers
{
    using Microsoft.VisualStudio.Shell.Interop;

    public enum SolutionChangedReason
    {
        None,
        Load,
        Unload,
    }


    public class SolutionEventsListenerEventArgs : System.EventArgs
    {
        public IVsProject Project { get; private set; }
        public SolutionChangedReason ChangedReason { get; private set; }

        public SolutionEventsListenerEventArgs(IVsProject project, SolutionChangedReason reason)
        {
            this.Project = project;
            this.ChangedReason = reason;
        }
    }
}