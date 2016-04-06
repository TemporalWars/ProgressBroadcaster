using System;
using System.Diagnostics;
using System.Windows.Controls;
using ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces;
using ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces.EventArgs;

namespace ImageNexus.BenScharbach.Common.ProgressBar.Simple.Receiver
{
    /// <summary>
    /// The <see cref="ProgressBarReceiver"/> WPF user-control is the Progress-Receiver for the <see cref="IProgressBroadcasting"/> component.
    /// </summary>
    public partial class ProgressBarReceiver : UserControl
    {
        private readonly ProgressAnimation _progressAnimation;
        private IProgressBroadcasting _progressBroadcasting;

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressBarReceiver()
        {
            InitializeComponent();

            // create instance of progress-animation
            _progressAnimation = new ProgressAnimation(PbReceiver);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Connects this receiver to the <see cref="IProgressBroadcasting"/> component.
        /// </summary>
        /// <param name="progressBroadcasting">Instance of the <see cref="IProgressBroadcasting"/></param>
        public void ConnectToProgressBroadcaster(IProgressBroadcasting progressBroadcasting)
        {
            if (progressBroadcasting == null) throw new ArgumentNullException("progressBroadcasting");

            // save ref.
            _progressBroadcasting = progressBroadcasting;

            // connect to Broadcasting event messages.
            _progressBroadcasting.ProgressIteration += ProgressBroadcasting_ProgressIteration;
        }

        /// <summary>
        /// Reset internal values to zero.
        /// </summary>
        public void Reset()
        {
            _progressAnimation.Reset();
        }

        #endregion
        
        #region Private Methods

        /// <summary>
        /// Occurs when the <see cref="IProgressBroadcasting"/> component sends out a progress-iteration message.
        /// </summary> 
        private void ProgressBroadcasting_ProgressIteration(object sender, ProgressEventArgs e)
        {
            // Testing:
            Debug.WriteLine("Progress-Iteration is {0}:{1},{2}", e.Count, e.Total, e.Header);

            // 4/2/2016 - test progress-animation updates
            _progressAnimation.Increment(e.Total);
        }

        #endregion

    }
}
