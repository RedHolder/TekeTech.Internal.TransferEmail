using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using EmailService.Api.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

namespace MonitorApplication.Pages
{
    public class IndexModel : PageModel
    {
        private readonly HttpClient _httpClient;

        public List<EmailService.Api.Models.EmailCampaign> Campaigns { get; set; }

        public IndexModel(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(); // You might need to configure the HttpClient in Startup.cs
        }

        public async Task OnGetAsync()
        {
            try
            {
                // Replace "your-api-url" with the actual API endpoint to retrieve EmailCampaigns
                var response = await _httpClient.GetAsync("https://localhost:7221/api/EmailCampaign\r\n");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    Campaigns = JsonConvert.DeserializeObject<List<EmailService.Api.Models.EmailCampaign>>(content);
                }
                else
                {
                    // Handle the error scenario
                    Campaigns = new List<EmailService.Api.Models.EmailCampaign>();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions
                Campaigns = new List<EmailService.Api.Models.EmailCampaign>();
            }
        }
    }
}
