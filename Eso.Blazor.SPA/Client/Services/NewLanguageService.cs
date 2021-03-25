using Eso.API.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Eso.Blazor.SPA.Client.Services {
        public class NewLanguageService : ILanguageService {
                public HttpClient Client { get; set; }
                public string Language { get; set; }

                public bool UnderstandsEditing => false;

                public async Task<LanguageCommand[]> Commands() =>
                        await Client.GetFromJsonAsync<LanguageCommand[]>("EsotericLanguageEditor/languages/example/language");

                public async Task<HttpResponseMessage> Save(Language l) =>
                        await Client.PostAsJsonAsync("EsotericLanguageEditor/languages", l);

        }
}
