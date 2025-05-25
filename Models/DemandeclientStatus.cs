namespace Hirfa.Web.Models
{
    public enum DemandeclientStatus
    {
        Pending,      // Awaiting action
        Matching,    // In progress of matching
        Complete,    // Successfully completed
        Canceled,    // Canceled by client
        NotFound     // Demand not found
    }
}
