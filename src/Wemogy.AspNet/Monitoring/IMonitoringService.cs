namespace Wemogy.AspNet.Monitoring
{
    public interface IMonitoringService
    {
        /// <summary>
        /// Tracks the occurrence of an event.
        /// </summary>
        /// <param name="eventName">Name of the event.</param>
        /// <param name="eventDescription">Description about the event.</param>
        void TrackEvent(string eventName, string eventDescription = "");
    }
}
