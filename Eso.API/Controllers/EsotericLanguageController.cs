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
                public IEnumerable<LanguageMetadata> NativelySupportedLanguages() =>
                        Service
                                .RegisteredInterpreters
                                .Select(CreateMetadata);


                [HttpPost("languages/{language}/execute")]
                public IActionResult ExecuteSimple(string language, [FromBody] RawRequest src) {
                        var interp = Service.InterpreterFor(language);
                        IActionResult result = NotFound();
                        if (interp != null) {
                                var wrapper = new SelfContainedWrapper(src.Input);
                                interp.InterpretAsync(wrapper, new[] { src.Source });
                                result = new ObjectResult(wrapper.Sink);
                        }

                        return result;
                }

                private LanguageMetadata CreateMetadata(IEsotericInterpreter l) =>
                        LanguageMetadata.Create(l.Language, l.Summary, l.Url);
        }
}