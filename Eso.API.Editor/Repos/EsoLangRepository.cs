using Eso.API.Editor.Models;
using Eso.API.Shared;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Eso.API.Editor.Repos {

        public class EsoLangRepository : IEsoLangRepository {

                private const string QueueName = "LangUpdate";

                private EditorDBContext Context { get; }

                private IQueueSink Sink { get; }

                private IConfiguration Config { get; }

                public EsoLangRepository(EditorDBContext ctx, IQueueSink sink, IConfiguration cfg) {
                        Context = ctx;
                        Sink = sink;
                        Config = cfg;
                }

                public IEnumerable<Language> All => Context.Languages;

                public Language Get(string language, Action<Language> postFetch = null) {
                        var l = Context
                        .Languages
                        .Include(l => l.Commands)
                        .FirstOrDefault(l => l.Name == language);
                        if (l != null) postFetch?.Invoke(l);
                        return l;
                }

                public async Task<Language> Update(Language language) {
                        var l = Get(language.Name);
                        if (l == null) throw new ApplicationException($"No such language {language.Name}");
                        CheckHash(language, 
                                  $"Language {language.Name} has a hash conflict",
                                  l => l.Hash == language.Hash && language.Name != l.Name);
                        Context.RemoveRange(l.Commands);
                        language.Commands.ToList().ForEach(c => c.Id = 0);
                        l.Commands = language.Commands;
                        await Context.SaveChangesAsync();
                        await NotifyUpdateOrAddition(l, true);
                        return l;
                }

                public async Task Create(Language lang) {
                        lang.Hash = lang.CalculateHash();
                        CheckHash(lang, 
                                $"Language {lang.Name} already exists or has an identical lexicon to another language",
                                l => l.Hash == lang.Hash || lang.Name == l.Name);
                        Context.Languages.Add(lang);
                        await Context.SaveChangesAsync();
                        await NotifyUpdateOrAddition(lang);
                }

                private void CheckHash(Language lang, string msg, Expression<Func<Language, bool>> test) {
                        lang.Hash = lang.CalculateHash();
                        if (Context.Languages.Any(test))
                                throw new ApplicationException(msg);
                }

                private async Task NotifyUpdateOrAddition(Language language, bool update = false) {
                        var payload = new  {
                                Name = language.Name,
                                Url = "Unknown",
                                Turing_complete = true,
                                IsCustom = true
                                
                        };
                        // Note camel case property name
                        var body = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
                        await Sink.UsingConfiguration(Config).PublishToQueueAsync(QueueName, body);
                }
        }
}
