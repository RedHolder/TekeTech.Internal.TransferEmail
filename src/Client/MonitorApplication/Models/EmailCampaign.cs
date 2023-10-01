using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmailService.Api.Models
{
    public class EmailCampaign
    {
        [Key] // This sets CampaignId as the primary key
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // This makes CampaignId auto-incremental
        public int CampaignId { get; set; }

        [Required]
        public string CampaignName { get; set; }

        [Required]
        public string CampaignSender { get; set; }

        [Required]
        public string CampaignSubject { get; set; }

        [Required]
        public string CampaignBody { get; set; }

        public string Parameters { get; set; }
    }
}
