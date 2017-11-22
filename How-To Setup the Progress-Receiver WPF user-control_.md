To use the **Progress-Receiver** components, simply follow the instructions below;

**#1.** For the project/assembly which you want to receive broadcast messages, include reference to the following assembly at the top of the XAML page:  

{{
xmlns:progressReceiver="clr-namespace:ImageNexus.BenScharbach.Common.ProgressBar.Simple.Receiver;assembly=ImageNexus.BenScharbach.Common.ProgressBar.Simple.Receiver";
}}

**#2.** Drop a copy of the ProgressBarReceiver XAML control into your window;

{{ 
<progressReceiver:ProgressBarReceiver Grid.Row="0" Height="10" HorizontalAlignment="Left" Margin="0" Name="ProgressBarItem" VerticalAlignment="Top" Width="{Binding ElementName=dgSearchResults, Path=ActualWidth}" />
}}

**#3.** Now connect to the ProgressBroadcasting component in your other projects by calling the ConnectToProgressBroadcaster([IProgressBroadcasting](IProgressBroadcasting)) method and passing in your interface for IProgressBroadcasting;

{{
public IProgressBroadcasting ProgressBroadcasting
{
// Connect to the ProgressBroadcaster's connection port.                         ProgressBarItem.ConnectToProgressBroadcaster(otherAssembly.ProgressBroadcasting);
}
}}

**#4.** Call Reset() method to restart a new progress-receiver iteration;

{{
// Reset progress-animation for resuse.
ProgressBarItem.Reset();
}}

**#5.** The iteration work or call to the broadcasting assembly MUST be run inside some Threading operation; otherwise, the UI will be blocked.
{{
// Run progress intense operation in Task thread;
Task.Factory.StartNew(ProgressAction);

private void ProgressAction()
{
// Call Reset()
ProgressBarItem.Reset();

// Run long running progress-bar intense operation which uses the ProgressBroadcaster
otherAssembly.SomeLongRunningOp();

// Any UI calls from inside this Task context must be sent via the Dispatcher call.
}

}}

**#6.** To catch the messages sent from the broadcasting assembly, use the following code to subscribe to the ProgressIteration event.
{{
// Subscribe to IProgressBroadcaster.ProgressIteration event.
otherAssembly.ProgressBroadcasting.ProgressIteration += ProgressBroadcasting_ProgressIteration;

// Example code for event-handler;
private void ProgressBroadcasting_ProgressIteration(object sender, ProgressEventArgs e)
{
    var message = e.Header;
    TextBlockControl.Text = message;    
}
}}