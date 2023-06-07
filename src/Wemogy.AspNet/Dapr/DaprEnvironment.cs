namespace Wemogy.AspNet.Dapr
{
    public class DaprEnvironment
    {
        public bool UseCloudEvents { get; private set; }

        public DaprEnvironment()
        {
        }

        public DaprEnvironment WithCloudEvents()
        {
            UseCloudEvents = true;
            return this;
        }
    }
}
