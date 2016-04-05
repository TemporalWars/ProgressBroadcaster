using System;
using ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces;
using ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces.EventArgs;

namespace ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster
{
    /// <summary>
    /// This <see cref="ProgressBroadcasting"/> class is used to broadcast progress-iteration messages
    /// back to the Progress-Animation component.
    /// </summary>
    public class ProgressBroadcasting : IProgressBroadcasting
    {
        #region Events
       
        /// <summary>
        /// Occurs for each Progress iteration.
        /// </summary>
        public event EventHandler<ProgressEventArgs> ProgressIteration;

        #endregion


        #region Public Methods

        /// <summary>
        /// Broadcast progress iteration message.
        /// </summary>
        public void BroadcastProgressMessage(object sender, int count, int total, ref string fileName)
        {
            if (ProgressIteration != null)
                ProgressIteration(sender, new ProgressEventArgs(count, total, fileName));
        }

        #endregion
    }
}
