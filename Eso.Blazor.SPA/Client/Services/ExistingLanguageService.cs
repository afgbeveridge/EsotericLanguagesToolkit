﻿using Eso.API.Editor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Eso.Blazor.SPA.Client.Services {
        public class ExistingLanguageService : ILanguageService {

                public HttpClient Client { get; set; }
                public string Language { get; set; }

                public bool UnderstandsEditing => true;

                public async Task<LanguageCommand[]> Commands() =>
                        (await Client.GetFromJsonAsync<Language>($"EsotericLanguageEditor/languages/{Language}"))?.Commands.ToArray();

                public async Task<HttpResponseMessage> Save(Language l) =>
                        await Client.PutAsJsonAsync($"EsotericLanguageEditor/languages/{l.Name}", l);

        }
}
