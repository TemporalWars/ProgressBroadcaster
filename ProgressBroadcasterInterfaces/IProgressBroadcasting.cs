using System;
using ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces.EventArgs;

namespace ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces
{
    public interface IProgressBroadcasting
    {
        /// <summary>
        /// Occurs for each Progress iteration.
        /// </summary>
        event EventHandler<ProgressEventArgs> ProgressIteration;

        /// <summary>
        /// Broadcast progress iteration message.
        /// </summary>
        void BroadcastProgressMessage(object sender, int count, int total, ref string fileName);
    }
}