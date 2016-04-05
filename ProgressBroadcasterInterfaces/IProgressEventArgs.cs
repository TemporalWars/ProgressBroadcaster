namespace ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces
{
    /// <summary>
    /// The <see cref="IProgressEventArgs"/> interface used in the Event ProgressIteration.
    /// </summary>
    public interface IProgressEventArgs
    {
        int Count { get; }
        int Total { get; }
        string Header { get; }
    }
}