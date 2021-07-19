using Eso.API.Editor.Models;
using System.Collections.Generic;

namespace Eso.API.Editor.Services {
        public interface IExampleGenerator {
                IEnumerable<ExampleProgram> ProcessTemplates(string language, IEnumerable<string> paths, bool retainEOL = false);

                DocumentSet ProcessGeneralTemplate(string language, string path, bool retainEOL = false);

                DocumentSet ProcessGeneralTemplate(Language lang, bool retainEOL = false);
        }
}