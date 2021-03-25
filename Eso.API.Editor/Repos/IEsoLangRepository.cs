using Eso.API.Editor.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Eso.API.Editor.Repos {
        public interface IEsoLangRepository {
                IEnumerable<Language> All { get; }

                Task Create(Language lang);

                Language Get(string language, Action<Language> postFetch = null);

                Task<Language> Update(Language language);
        }
}