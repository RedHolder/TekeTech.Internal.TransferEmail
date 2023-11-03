using EmailService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace EmailService.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : ControllerBase
    {
        public string FromEmail = "BekirsEmailAddress@hotmail.com";
        public string FromEmailPassword = "Password";

        private readonly DatabaseContext _context;
        private readonly ILogger<EmailController> _logger;

        public EmailController(DatabaseContext context, ILogger<EmailController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet(Name = "GetEmail")]
        public async Task<IActionResult> SendEmailAsync(int campaignID, string receiverEmail, string OrderID, string parameters)
        {
            try
            {
                // Get the email campaign by campaignID from the database
                var emailCampaign = await _context.EmailCampaign.FirstOrDefaultAsync(ec => ec.CampaignId == campaignID);

                if (emailCampaign == null)
                {
                    // Handle the case where the campaign is not found.
                    return NotFound("Email campaign with ID " + campaignID + " not found.");
                }

                // Compose and send the email asynchronously with parameter replacement
                await SendEmailAsync(emailCampaign, receiverEmail, parameters);

                // Store information related to the sent email in your database
                var emailModel = new EmailModel
                {
                    ReceiverID = receiverEmail,
                    Campaign = emailCampaign,
                    SendDate = DateTime.UtcNow,
                    Status = 1, // Assuming a status code for success
                };

                // Save the email model to the database
                _context.EmailModel.Add(emailModel);
                await _context.SaveChangesAsync();

                // Keep the OrderID for further use
                // ...

                return Ok("Email sent successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending the email.");
                return StatusCode((int)HttpStatusCode.InternalServerError, "An error occurred while sending the email.");
            }
        }

        private async Task SendEmailAsync(EmailCampaign campaign, string receiverEmail, string parameters)
        {
            using (var message = new MailMessage(new MailAddress(FromEmail, campaign.CampaignSender), new MailAddress(receiverEmail)))
            {
                message.Subject = campaign.CampaignSubject;

                // Replace placeholders in the HTML content with values from parameters
                var body = ReplaceParametersInHtml(campaign.CampaignBody, parameters);

                message.Body = body;
                message.IsBodyHtml = true;

                using (var smtp = new SmtpClient("smtp.office365.com"))
                {
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;

                    var credentials = new NetworkCredential
                    {
                        UserName = FromEmail,
                        Password = FromEmailPassword
                    };

                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = credentials;

                    await smtp.SendMailAsync(message);
                }
            }
        }

        private string ReplaceParametersInHtml(string html, string parameters)
        {
            // Split the parameters string into individual parameter=value pairs
            var paramPairs = parameters.Split(',').Select(param => param.Trim());

            // Replace placeholders in the HTML content with their corresponding values
            foreach (var paramPair in paramPairs)
            {
                var keyValue = paramPair.Split('=');
                if (keyValue.Length == 2)
                {
                    var parameterName = "@" + keyValue[0] + "@"; // Assuming placeholders are in the format @ParameterName@
                    var parameterValue = keyValue[1];
                    html = html.Replace(parameterName, parameterValue);
                }
            }

            return html;
        }
    }
}
