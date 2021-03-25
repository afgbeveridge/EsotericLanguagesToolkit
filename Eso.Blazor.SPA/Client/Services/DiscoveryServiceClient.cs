using Eso.API.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Eso.Blazor.SPA.Client.Services {

        public class DiscoveryServiceClient {
                private HttpClient Client { get; }

                public DiscoveryServiceClient(HttpClient client) => Client = client;

                public async Task<IEnumerable<Language>> GetLanguages() => (await Client.GetFromJsonAsync<Language[]>("languages")).ToArray();
                
        }
}
