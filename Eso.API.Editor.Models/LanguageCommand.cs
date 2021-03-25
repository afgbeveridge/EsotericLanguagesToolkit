
using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Eso.API.Editor.Models {

        public class LanguageCommand {

                public int Id { get; set; }

                public string Keyword { get; set; }

                public string Concept { get; set; }

                public string Nature { get; set; }

                public DateTime DateCreated { get; set; }

                public Language Language { get; set; }

        }
}
