using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Eso.API.Editor.Models {

        public static class LanguageExtensions {

                public static string CalculateHash(this Language lang) {
                        var hash = default(string);
                        using (var sha = new SHA512Managed()) {
                                var lexicon = string.Join(",", lang.Commands.Select(c => $"{c.Keyword}-{c.Concept}"));
                                var data = Encoding.UTF8.GetBytes(lexicon);
                                hash = Encoding.UTF8.GetString(sha.ComputeHash(data));
                        }
                        return hash;
                }
        }

}
