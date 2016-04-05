using System;

namespace ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces.EventArgs
{
    /// <summary>
    /// The <see cref="ProgressEventArgs"/>.
    /// </summary>
    public class ProgressEventArgs : System.EventArgs, IProgressEventArgs
    {
        private readonly int _count;
        private readonly int _total;
        private readonly string _header;

        #region Properties

        /// <summary>
        /// Gets the current count value.
        /// </summary>
        public int Count
        {
            get { return _count; }
        }

        /// <summary>
        /// Gets the total count value.
        /// </summary>
        public int Total
        {
            get { return _total; }
        }

        /// <summary>
        /// Gets the header value.
        /// </summary>
        public string Header
        {
            get { return _header; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new custom <see cref="EventArgs"/> for the progress iteration.
        /// </summary>
        /// <param name="count">Count iteration.</param>
        /// <param name="total">Total value.</param>
        /// <param name="header">Header value.</param>
        public ProgressEventArgs(int count, int total, string header)
        {
            _count = count;
            _total = total;
            _header = header;
        }

        #endregion
        
       

    }
}