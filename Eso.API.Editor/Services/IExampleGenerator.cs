using Eso.API.Editor.Models;
using System.Collections.Generic;

namespace Eso.API.Editor.Services {
        public interface IExampleGenerator {
                IEnumerable<ExampleProgram> ProcessTemplates(string language, IEnumerable<string> paths);

                string ProcessGeneralTemplate(string language, string path);
        }
}