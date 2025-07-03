using System.ComponentModel.DataAnnotations;

namespace OmnitakSupportHub.Models
{
    public class RoutingRule
    {
        [Key]
        public int RuleID { get; set; }

        public int CategoryID { get; set; }
        public required virtual Category Category { get; set; }

        public int TeamID { get; set; }
        public required virtual SupportTeam Team { get; set; }

        public required string Conditions { get; set; }

        public bool IsActive { get; set; }
    }
}
