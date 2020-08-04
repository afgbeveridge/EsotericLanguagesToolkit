namespace Eso.API.Models {
        public class LanguageMetadata {
                public string Name { get; set; }

                public string Summary { get; set; }

                public string DetailsUrl { get; set; }

                public static LanguageMetadata Create(string name, string summary, string url = null) =>
                        new LanguageMetadata {
                                Name = name,
                                Summary = summary,
                                DetailsUrl = url
                        };
        }
}