using System.Collections.Generic;

namespace Eso.API.Models {
        public class RawRequest {
                public string Source { get; set; }

                public IEnumerable<string> Input { get; set; }
        }
}