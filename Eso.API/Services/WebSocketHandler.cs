﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Common;
using General.Language;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Diagnostics;
using Eso.API.Models;
using Eso.API.Shared;

namespace Eso.API.Services {
        public class WebSocketHandler : IWebSocketHandler {

                private const string Generalized = "*";

                private const string ExchangeName = "amq.fanout";

                public WebSocketHandler(IPluginService pluginService, IQueueSink sink, IConfiguration cfg) {
                        PluginHandler = pluginService;
                        Sink = sink;
                        Config = cfg;
                }

                private WebSocket Channel { get; set; }
                private CancellationTokenSource LocalToken { get; set; }
                private IPluginService PluginHandler { get; }
                private string Language { get; set; }

                private IConfiguration Config { get; }

                private IQueueSink Sink { get; }

                private Stopwatch Timer { get; } = new Stopwatch(); 

                public Task<IWebSocketHandler> Host(WebSocket socket) {
                        Channel = socket;
                        LocalToken = new CancellationTokenSource();
                        return Task.FromResult((IWebSocketHandler) this);
                }

                public async Task Process() {
                        var wrapper = new LocalIOWrapper(Channel, LocalToken);
                        var bundle = await GetLanguagSourceAndPossiblyCommands(wrapper);
                        var processor = LocateProcessor(bundle);
                        Timer.Start();
                        await processor.InterpretAsync(wrapper, new[] { bundle.Source });
                        Timer.Stop();
                        await Channel.CloseAsync(WebSocketCloseStatus.NormalClosure, "Execution complete",
                                LocalToken.Token);
                        await NotifyExecution(bundle);
                }

                private async Task<SourceBundle> GetLanguagSourceAndPossiblyCommands(LocalIOWrapper wrapper) {
                        var src = await wrapper.Receive(false);
                        // TODO: Assert
                        var end = src.IndexOf(src.First(), 1);
                        var cmds = src.IndexOf(src.First(), end + 1) + 1;
                        return new SourceBundle {
                                Language = src[1..end],
                                Source = src[(end + 1)..(cmds - 1)],
                                Commands = CreateCommands(cmds >= src.Length - 1 ? null : src[cmds..])
                        };
                }

                private Dictionary<KnownConcept, string> CreateCommands(string commands) {
                        var result = default(Dictionary<KnownConcept, string>);
                        if (commands != null) {
                                // Expect [concept=char<space>]+
                                result = commands.Split(' ')
                                           .Select(s => {
                                                   var comps = s.Split('\\');
                                                   return new { Concept = comps.First(), Keyword = comps[1] }; 
                                           })
                                           .ToDictionary(a => Enum.Parse<KnownConcept>(a.Concept), a => a.Keyword);
                        }
                        return result;
                }

                private IEsotericInterpreter LocateProcessor(SourceBundle bundle) {
                        var general = bundle.Commands != null;
                        var processor = PluginHandler.InterpreterFor(general ? Generalized : bundle.Language);
                        if (general) {
                                var interp = processor as IGeneralizedInterpreter;
                                interp.UseBindings(bundle.Commands);
                        }
                        return processor;
                }

                private async Task NotifyExecution(SourceBundle bundle) {
                        var stats = new StatisticsResult { 
                                Name = bundle.Language,
                                SourceCodeLength = (bundle.Source ?? string.Empty).Length,
                                ExecutionTimeMilliseconds = Timer.ElapsedMilliseconds
                        };
                        // Note camel case property name
                        var payload = JsonSerializer.Serialize(stats, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }); 
                        await Sink.UsingConfiguration(Config).PublishToExchangeAsync(ExchangeName, payload);
                }

                private class SourceBundle {
                        internal string Language { get; set; }
                        internal string Source { get; set; }
                        internal Dictionary<KnownConcept, string> Commands { get; set; } // Will be null/missing if natively supported
                }
        }
}