using Eso.API.Editor.Models;
using System.Collections.Generic;

namespace Eso.API.Editor.Services {
        public interface IExampleGenerator {
                IEnumerable<ExampleProgram> ProcessTemplates(string language, IEnumerable<string> paths);

                DocumentSet ProcessGeneralTemplate(string language, string path);

                DocumentSet ProcessGeneralTemplate(Language lang);
        }
}