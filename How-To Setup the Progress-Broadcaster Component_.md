To use the **Progress-Broadcaster** components, simply follow the instructions below;

**#1.** For the project/assembly which you want to broadcast progress information for, include reference to the following assemblies:  

{{
ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster.Interfaces;
ImageNexus.BenScharbach.Common.ProgressBar.Simple.Broadcaster;
}}

**#2.** Create an instance of the _IProgressBroadcasting_ component in the project/assembly to broadcast;

{{ 
private readonly IProgressBroadcasting _progressBroadcasting = new ProgressBroadcasting(); 
}}

**#3.** Create a Property to expose the new _IProgressBroadcasting_ component;

{{
public IProgressBroadcasting ProgressBroadcasting
{
     get { return _progressBroadcasting; }
}
}}

**#4.** To send some progress iteration message, use the following method call;

{{
_progressBroadcasting.BroadcastProgressMessage(this, {"[count](count), [total-count](total-count), ref [header-name](header-name)"});
Thread.Sleep(1); // (3.3.17): Thread.Sleep call is important to allow the message to move towards the receiving assembly.
}}
