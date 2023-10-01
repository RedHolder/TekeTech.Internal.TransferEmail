using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EmailService.Api.Models
{
    public class EmailModel
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmailID { get; set; }

        public string ReceiverID { get; set; }

        public EmailCampaign Campaign { get; set; }

        public DateTime SendDate { get; set; }

        public int Status { get; set; }

    }
}
