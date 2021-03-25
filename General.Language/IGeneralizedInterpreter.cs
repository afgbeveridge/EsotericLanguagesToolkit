using System;
using System.Collections.Generic;
using System.Text;

namespace General.Language {
        public interface IGeneralizedInterpreter {
                void UseBindings(Dictionary<KnownConcept, string> bindings);
        }
}
