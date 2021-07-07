using Eso.API.Editor.Models;
using General.Language;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Eso.API.Editor.Repos;
using System.Threading.Tasks;
using Eso.API.Editor.Services;

namespace Eso.API.Editor.Controllers {

        [ApiController]
        [Route("[controller]")]
        public class EsotericLanguageEditorController : ControllerBase {

                private IEsoLangRepository Repo { get; }

                private IExampleGenerator Generator { get; }

                public EsotericLanguageEditorController(IEsoLangRepository repo, IExampleGenerator generator) {
                        Repo = repo;
                        Generator = generator;
                }

                [HttpGet("languages")]
                public IEnumerable<object> GetAllKnownLanguages() => Repo.All.Select(l => new { Name = l.Name }).OrderBy(a => a.Name);

                [HttpGet("languages/{language}", Name = "GetKnownLanguage")]
                public Language GetKnownLanguage([FromRoute] string language)
                        => Repo.Get(language, CreateAcceptablePayload); // Avoid cyclic dependency (EF -> System.Text.Json)

                [HttpPut("languages/{language}", Name = "UpdateKnownLanguage")]
                public async Task<Language> UpdateKnownLanguage([FromBody] Language language)
                        => await Repo.Update(language); // Avoid cyclic dependency (EF -> System.Text.Json)

                [HttpPost("languages")]
                public async Task<IActionResult> CreateLanguage([FromBody] Language lang) {
                        await Repo.Create(lang);
                        CreateAcceptablePayload(lang);
                        return CreatedAtRoute("GetKnownLanguage", new { language = lang.Name }, lang);
                }

                [HttpGet("languages/example/language")]
                public IEnumerable<Models.LanguageCommand> Get() =>
                        Constants
                        .DefaultKeywordBindings
                        .Select(kvp =>
                                new Models.LanguageCommand {
                                        Keyword = kvp.Value,
                                        Concept = kvp.Key.ToString(),
                                        Nature = FindAttribute(kvp.Key)?.Nature.ToString() ?? string.Empty
                                });

                [HttpGet("languages/example/documentation")]
                public IActionResult GetDocTemplate() => new FileStreamResult(System.IO.File.OpenRead("Templates/DocTemplate.md"), "application/markdown");

                [HttpGet("languages/example/programs")]
                public IEnumerable<ExampleProgram> GetExamplePrograms([FromQuery] string language) => 
                        Generator.ProcessTemplates(language, new[] { @"Templates\HelloWorld.txt", @"Templates\ReverseString.txt" });

                private void CreateAcceptablePayload(Language l) {
                        l.Commands
                                .ToList()
                                .ForEach(c => {
                                        c.Language = null; // Avoid cyclic dependency (EF -> System.Text.Json)
                                        c.Nature = FindAttribute(c.Concept)?.Nature.ToString() ?? string.Empty;
                                 }); 
                }

                private static CommandNatureAttribute FindAttribute(KnownConcept name) => FindAttribute(name.ToString());

                private static CommandNatureAttribute FindAttribute(string name) =>
                        typeof(KnownConcept).GetField(name).GetCustomAttributes<CommandNatureAttribute>().FirstOrDefault();
        }

}
