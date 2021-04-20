using Eso.API.Editor.Models;
using Eso.Blazor.SPA.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Eso.Blazor.SPA.Client.Services {
        public class StatisticsServiceClient {
                private HttpClient Client { get; }

                public StatisticsServiceClient(HttpClient client) => Client = client;

                public async Task<BasicStatistics> GetStatisticsFor(string lang) => await Client.GetFromJsonAsync<BasicStatistics>($"stats/{lang}");
        }
}
