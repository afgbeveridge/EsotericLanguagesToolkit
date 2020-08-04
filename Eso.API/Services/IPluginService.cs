using System.Collections.Generic;
using Common;

namespace Eso.API.Services {
        public interface IPluginService {
                IEnumerable<IEsotericInterpreter> RegisteredInterpreters { get; }
                IEsotericInterpreter InterpreterFor(string language);
        }
}