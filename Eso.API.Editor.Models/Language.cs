using System;
using System.Collections.Generic;

namespace Eso.API.Editor.Models {

        public class Language {

                public int Id { get; set; }

                public string Name { get; set; }

                public string Hash { get; set; }

                public DateTime DateCreated { get; set; }

                public bool IsNativelySupported { get; set; }

                public ICollection<LanguageCommand> Commands { get; set; }

                public string Documentation { get; set; }

                public string ProcessedDocumentation { get; set; }

        }

}
