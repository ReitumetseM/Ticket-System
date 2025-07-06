namespace OmnitakSupportHub.Models.ViewModels
{
    public class TicketDto
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int StatusID { get; set; }
        public int PriorityID { get; set; }
        public int CategoryID { get; set; }
        public int CreatedBy { get; set; }
        public int? AssignedTo { get; set; }
        public int? TeamID { get; set; }
    }
}
