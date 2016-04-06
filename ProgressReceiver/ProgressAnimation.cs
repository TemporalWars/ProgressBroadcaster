using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ImageNexus.BenScharbach.Common.ProgressBar.Simple.Receiver
{
    // 4/2/2016; 4/5/2016 - Moved from AutoSecure to this new ProgressReceiver project.
    /// <summary>
    /// This <see cref="ProgressAnimation"/> is going to do the progress as an animation of parts, rather than by the exact iterative value. - I believe this is my writing (Ben Scharbach).
    /// </summary>
    internal sealed class ProgressAnimation
    {
        private readonly ManualResetEvent _mreUpdateProgressTask = new ManualResetEvent(false);
        private readonly ManualResetEvent _mreUpdateProgressPartTask = new ManualResetEvent(false);
        private readonly ManualResetEvent _mreIncrement = new ManualResetEvent(false);
        private readonly ManualResetEvent _mreIncrement2 = new ManualResetEvent(false);
        private readonly object _setCollectionTotalCountLock = new object();
        private readonly Dispatcher _dispatcher;
        private readonly System.Windows.Controls.ProgressBar _progressBar;
        private const float ProgressTotalParts = 2500;
        private const float ProgressForOnePart = (int)(ProgressTotalParts / 100); // 1 iteration part = ProgressTotalParts / 100; example = 10000/100 = 100.

        private volatile float _collectionTotalCount; // example: collection of 10 items
        private volatile float _collectionIncrementForOnePart; // exmaple: onePart = 1 / 10 or 10 percent.
        private volatile float _collectionCurrentCount;
        private volatile int _priorCheckPercent;

        // items for the Stop task worker.
        private readonly Stopwatch _stopwatch = new Stopwatch();
        private volatile float _stopwatchAccumPercentPart;
        private volatile float _stopwatchAveragePercentPart;
        private volatile int _stopwatchStopCounter;
         

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public ProgressAnimation(System.Windows.Controls.ProgressBar progressBar, float total = 0)
        {
            if (progressBar == null) throw new ArgumentNullException("progressBar");

            // save
            _dispatcher = progressBar.Dispatcher;
            _progressBar = progressBar;

            // set the progressBar WPF item's Max/Value.
            _progressBar.Maximum = ProgressTotalParts;
            _progressBar.LargeChange = 1.0;
            _progressBar.Value = 0;

            // store the caller's collection count values.
            if (total > 0) SetCollectionTotalCount(total);

            // start a worker thread for updating progress-bar WPF item.
            Task.Factory.StartNew(UpdateProgressTask);
            // start a worker thread for tracking the average time between percent part updates.
            Task.Factory.StartNew(UpdateProgressPartTask);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Increments by 1 item.
        /// </summary>
        public void Increment()
        {
            _collectionCurrentCount += 1;

            // let the Task know an increment occurred.
            _mreIncrement.Set();
            _mreIncrement2.Set();
        }

        /// <summary>
        /// Increments by 1 item.
        /// </summary>
        public void Increment(float total)
        {
            SetCollectionTotalCount(total);
            Increment();
        }

        /// <summary>
        /// Reset internal values to zero.
        /// </summary>
        public void Reset()
        {
            // set the progressBar WPF item's Max/Value.
            _progressBar.Maximum = ProgressTotalParts;
            _progressBar.LargeChange = 1.0;
            _progressBar.Value = 0;

            // reset current count.
            _collectionTotalCount = 0;
            _collectionIncrementForOnePart = 0;
            _collectionCurrentCount = 0;
            _priorCheckPercent = 0;

            // reset time values
            _stopwatch.Reset();
            _stopwatchAccumPercentPart = 0;
            _stopwatchAveragePercentPart = 0;
            _stopwatchStopCounter = 0;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Sets the total collection count value.
        /// </summary>
        private void SetCollectionTotalCount(float total)
        {
            if (Math.Abs(total - 0) < float.Epsilon)
                throw new ArgumentOutOfRangeException("total", "Total cannot be less than 1.");

            // if already set, skip calculations.
            if (Math.Abs(total - _collectionTotalCount) < float.Epsilon) return;

            // thread lock crital section.
            lock (_setCollectionTotalCountLock)
            {
                // store the caller's collection count values.
                //Interlocked.Add(ref _collectionTotalCount, total);
                _collectionTotalCount = total;
                _collectionIncrementForOnePart = total / 100;
            }
        }

        /// <summary>
        /// The Task worker which will track the average times for progress-bar Part iteration updates.
        /// </summary>
        private void UpdateProgressPartTask()
        {
            while (!_mreUpdateProgressPartTask.WaitOne(1))
            {
                // wait until an increment occurs.
                while (!_mreIncrement2.WaitOne(1))
                {
                    Thread.Sleep(0);
                }
                _mreIncrement2.Reset();

                // check percent; example: 1 / 10 = .10 percent * 100 = 10 percent parts.
                // Idea: To get the average time per 1-percent part.  So, if 10 percent parts passed in 2 seconds, then this would be 2000 ms / 10 = 200 ms.
                //       Then the 200 ms would be the sleep time to give to the other Worker thread.  This sleep time of 200 ms would then be divided by the ProgressForOnePart value
                //       to come up with the Sleep() time call between animation updates.
                var checkPercent = _collectionCurrentCount / _collectionTotalCount * 100;

                // check if the stopwatch was started.
                if (!_stopwatch.IsRunning)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();

                    // to stop the stopwatch, check if at-least one percent has passed.
                    if (checkPercent < 1) continue;
                }

                // to stop the stopwatch, check if at-least one percent has passed.
                if (checkPercent < 1) continue;

                // ok, so at-least one percent has passed, so stop the stopwatch
                _stopwatch.Stop();

                // calculate an average-time based on the amount of time for the percent parts passed.
                var averageTime = _stopwatch.ElapsedMilliseconds/checkPercent;
                _stopwatchAccumPercentPart += averageTime;
                _stopwatchStopCounter++;
                _stopwatchAveragePercentPart = _stopwatchAccumPercentPart / _stopwatchStopCounter;

            }
        }

        /// <summary>
        /// The Task worker which will do the parts-animation update to a progress-bar WPF item for each iterative update.
        /// </summary>
        private void UpdateProgressTask()
        {
            while (!_mreUpdateProgressTask.WaitOne(1))
            {
                // wait until an increment occurs.
                while (!_mreIncrement.WaitOne(1))
                {
                    Thread.Sleep(0);
                }
                _mreIncrement.Reset();
                
                // check percent; example: 1 / 10 = .10 percent * 100 = 10 percent parts.
                var checkPercent = (int)(_collectionCurrentCount/_collectionTotalCount * 100);

                // check if percent is less than 1 percent; only continue to animation if greater than 1 percent part.
                if (checkPercent < 1) continue;

                // check if current percent is greater than prior percent before doing another animation.
                if (checkPercent < _priorCheckPercent) continue;

                // iterate the overall checkPercent parts and do N pieces per part...
                var sleepTime = (int)Math.Round(_stopwatchAveragePercentPart);
                for (var i = _priorCheckPercent; i < checkPercent; i++)
                {
                    // update the progresbar animation by N pieces per part.
                    for (var j = 0; j < ProgressForOnePart; j++)
                    {
                        UpdateProgressBarItem();
                        Thread.Sleep(sleepTime);
                    }
                } // endFor

                // store the prior checkPercent so it is not repeated!
                _priorCheckPercent = checkPercent;
            }
        }

        /// <summary>
        /// Manually updates the ProgressBarItem WPF by one increment through the Dispatcher object to get back to the GUI thread.
        /// </summary>
        private void UpdateProgressBarItem()
        {
            // manually pass the work to the GUI thread.
            _dispatcher.Invoke(new Action(() =>
                {
                    _progressBar.Value += 1;
                }), DispatcherPriority.Send);
        }

        #endregion

    }
}
