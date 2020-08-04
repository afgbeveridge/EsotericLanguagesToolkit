using System;
using System.Collections.Generic;

namespace Interpreter.Abstractions {
        public class SimpleSourceCode : SourceCode {
                public SimpleSourceCode() => SeekCache = new Dictionary<string, MutableTuple<int>>();

                private Dictionary<string, MutableTuple<int>> SeekCache { get; }

                private string CurrentCacheKey { get; set; }

                public override bool Advance() {
                        SourcePosition.X += 1;
                        while (SourcePosition.Y < Content.Count &&
                               SourcePosition.X >= Content[SourcePosition.Y].Length) {
                                SourcePosition.X = 0;
                                SourcePosition.Y += 1;
                        }

                        Fire();
                        return More();
                }

                public override bool More() =>
                        SourcePosition.Y < Content.Count &&
                        (SourcePosition.X < Content[SourcePosition.Y].Length ||
                         SourcePosition.Y < Content.Count - 1);

                public override bool Backup() {
                        SourcePosition.X -= 1;
                        if (SourcePosition.X < 0) {
                                if (SourcePosition.Y > 0 && SourcePosition.Y > 0) SourcePosition.Y -= 1;

                                SourcePosition.X = Content[SourcePosition.Y].Length - 1;
                        }

                        Fire();
                        return SourcePosition.X >= 0;
                }

                public void Seek(char targetToken, SeekDirection direction = SeekDirection.Forward,
                        char? recurseToken = null, int depth = 0) {
                        var proceed = direction == SeekDirection.Forward
                                ? () => More()
                                : (Func<bool>) (() => SourcePosition.X > 0 || SourcePosition.Y > 0);
                        var onProceed = direction == SeekDirection.Forward
                                ? () => Advance()
                                : (Func<bool>) (() => Backup());
                        if (depth++ == 0) {
                                if (CachingEnabled) {
                                        CurrentCacheKey = string.Concat(SourcePosition.X, SourcePosition.Y, targetToken,
                                                direction, recurseToken);
                                        var location = SeekCache.ContainsKey(CurrentCacheKey)
                                                ? SeekCache[CurrentCacheKey]
                                                : null;
                                        if (location != null) {
                                                SourcePosition = new MutableTuple<int>(location);
                                                return;
                                        }
                                }

                                onProceed();
                        }

                        while (proceed() && CurrentCharacter() != targetToken) {
                                if (recurseToken != null && proceed() && CurrentCharacter() == recurseToken.Value) {
                                        onProceed();
                                        Seek(targetToken, direction, recurseToken, depth);
                                }

                                onProceed();
                        }

                        if (--depth == 0 && CachingEnabled && !SeekCache.ContainsKey(CurrentCacheKey))
                                SeekCache[CurrentCacheKey] = new MutableTuple<int>(SourcePosition);
                }
        }
}