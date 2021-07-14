using Eso.API.Editor.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Collections.Generic;

namespace Eso.Blazor.SPA.Client.Services {
        public interface ILanguageService {

                HttpClient Client { get; set; }

                string Language { get; set; }

                bool UnderstandsEditing { get; }

                ILanguageService Using(HttpClient http) {
                        Client = http;
                        return this;
                }

                ILanguageService ForLanguage(string lang) {
                        Language = lang;
                        return this;
                }

                Task<LanguageCommand[]> Commands();

                Task<Language> Save(Language l);

                Task<DocumentSet> Documentation(string name);

                Task<IEnumerable<ExampleProgram>> Examples();
        }
}