using Eso.API.Editor.Models;
using System.Collections.Generic;
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

                public async Task<Language> Save(Language l) =>
                        await (await Client.PostAsJsonAsync("EsotericLanguageEditor/languages", l)).Content.ReadFromJsonAsync<Language>();

                public async Task<DocumentSet> Documentation(string name) => await Client.GetFromJsonAsync<DocumentSet>($"EsotericLanguageEditor/languages/example/documentation?language={name}");

                public Task<IEnumerable<ExampleProgram>> Examples() => Task.FromResult(default(IEnumerable<ExampleProgram>));

        }
}
