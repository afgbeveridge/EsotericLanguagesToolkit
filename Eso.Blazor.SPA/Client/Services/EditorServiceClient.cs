using Eso.API.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Eso.Blazor.SPA.Client.Services {

        public class EditorServiceClient {
                private HttpClient Client { get; }

                public EditorServiceClient(HttpClient client) => Client = client;

                public async Task<IEnumerable<string>> GetLanguages()
                        => (await Client.GetFromJsonAsync<Language[]>("languages")).Select(l => l.Name).ToArray();

        }
}
