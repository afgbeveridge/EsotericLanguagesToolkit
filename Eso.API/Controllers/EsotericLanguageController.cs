using System.Collections.Generic;
using System.Linq;
using Common;
using Eso.API.Models;
using Eso.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Eso.API.Controllers {
        [Produces("application/json")]
        [Route("esoteric")]
        public class EsotericLanguageController : Controller {
                public EsotericLanguageController(IPluginService svc) => Service = svc;

                private IPluginService Service { get; }

                [HttpGet("languages")]
                public IEnumerable<LanguageMetadata> SupportedLanguages() =>
                        Service
                                .RegisteredInterpreters
                                .Select(CreateMetadata);

                [HttpGet("languages/{language}")]
                public IActionResult LanguageDetails(string language) {
                        var interp = Service.InterpreterFor(language);
                        return interp != null ? new ObjectResult(CreateMetadata(interp)) : (IActionResult) NotFound();
                }

                [HttpPost("languages/{language}/execute")]
                public IActionResult ExecuteSimple(string language, [FromBody] RawRequest src) {
                        var interp = Service.InterpreterFor(language);
                        IActionResult result = NotFound();
                        if (interp != null) {
                                var wrapper = new SelfContainedWrapper(src.Input);
                                interp.Interpret(wrapper, new[] {src.Source});
                                result = new ObjectResult(wrapper.Sink);
                        }

                        return result;
                }

                private LanguageMetadata CreateMetadata(IEsotericInterpreter l) =>
                        LanguageMetadata.Create(l.Language, l.Summary, l.Url);
        }
}