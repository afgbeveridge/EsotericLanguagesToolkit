using Eso.API.Editor.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;

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

                Task<HttpResponseMessage> Save(Language l);
        }
}