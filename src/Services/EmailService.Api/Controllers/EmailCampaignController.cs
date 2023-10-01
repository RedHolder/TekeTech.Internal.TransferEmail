using EmailService.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmailService.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailCampaignController : ControllerBase
    {
        private readonly DatabaseContext _context;

        public EmailCampaignController(DatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmailCampaigns()
        {
            try
            {
                var campaigns = await _context.EmailCampaign.ToListAsync();
                return Ok(campaigns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving email campaigns.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEmailCampaign(int id)
        {
            try
            {
                var campaign = await _context.EmailCampaign.FindAsync(id);

                if (campaign == null)
                {
                    return NotFound("Email campaign not found.");
                }

                return Ok(campaign);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while retrieving the email campaign.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmailCampaign([FromBody] EmailCampaign campaign)
        {
            try
            {
                if (campaign == null)
                {
                    return BadRequest("Invalid campaign data.");
                }

                _context.EmailCampaign.Add(campaign);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEmailCampaign), new { id = campaign.CampaignId }, campaign);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the email campaign.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmailCampaign(int id, [FromBody] EmailCampaign campaign)
        {
            try
            {
                var existingCampaign = await _context.EmailCampaign.FindAsync(id);

                if (existingCampaign == null)
                {
                    return NotFound("Email campaign not found.");
                }

                existingCampaign.CampaignName = campaign.CampaignName;
                existingCampaign.CampaignSender = campaign.CampaignSender;
                existingCampaign.CampaignSubject = campaign.CampaignSubject;
                existingCampaign.CampaignBody = campaign.CampaignBody;
                existingCampaign.Parameters = campaign.Parameters;

                await _context.SaveChangesAsync();

                return Ok(existingCampaign);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while updating the email campaign.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmailCampaign(int id)
        {
            try
            {
                var campaign = await _context.EmailCampaign.FindAsync(id);

                if (campaign == null)
                {
                    return NotFound("Email campaign not found.");
                }

                _context.EmailCampaign.Remove(campaign);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while deleting the email campaign.");
            }
        }
    }
}
