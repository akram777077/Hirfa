namespace Hirfa.Web.Models
{
    public enum DemandeclientStatus
    {
        Pending,      // Awaiting action
        Valide,       // Accepted by service
        NonValide,    // Rejected by service
        FinalValide,  // Final acceptance (after prestataire validation)
        FinalNonValide // Final rejection (after prestataire validation)
    }
}
