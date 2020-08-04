using System;
using System.Collections.Generic;
using System.Linq;

namespace Interpreter.Abstractions {
        public class SourceCodeTorus : SourceCode {
                private readonly Dictionary<DirectionOfTravel, VectorBundle> MovementVectors =
                        new Dictionary<DirectionOfTravel, VectorBundle> {
                                {
                                        DirectionOfTravel.Left, new VectorBundle {
                                                Vector = new Tuple<int, int>(-1, 0),
                                                Bounder = (c, l) => {
                                                        if (c.X < 0) c.X = l.X - 1;
                                                }
                                        }
                                }, {
                                        DirectionOfTravel.Right, new VectorBundle {
                                                Vector = new Tuple<int, int>(1, 0),
                                                Bounder = (c, l) => {
                                                        if (c.X == l.X - 1) c.X = 0;
                                                }
                                        }
                                }, {
                                        DirectionOfTravel.Up, new VectorBundle {
                                                Vector = new Tuple<int, int>(0, -1),
                                                Bounder = (c, l) => {
                                                        if (c.Y < 0) c.Y = l.Y - 1;
                                                }
                                        }
                                }, {
                                        DirectionOfTravel.Down, new VectorBundle {
                                                Vector = new Tuple<int, int>(0, 1),
                                                Bounder = (c, l) => {
                                                        if (c.Y == l.Y - 1) c.Y = 0;
                                                }
                                        }
                                }
                        };

                // Columns x Rows
                private MutableTuple<int> InitialSize = new MutableTuple<int>();

                public SourceCodeTorus()
                        : this(null) { }

                public SourceCodeTorus(MutableTuple<int> size = null) {
                        Size = size ?? new MutableTuple<int>();
                        Direction = DirectionOfTravel.Right;
                }

                public override List<string> Content {
                        get => base.Content;
                        set {
                                ExecutionSupport.AssertNotNull(value,
                                        src => "Attempt to create torus with null content");
                                if (Content == null || !Content.Any()) ResizeFromContent();

                                MergeContent(value);
                        }
                }

                public MutableTuple<int> Size {
                        get => InitialSize;
                        set {
                                ExecutionSupport.AssertNotNull(value, t => "Cannot specify a null torus size");
                                InitialSize = value;
                                Resize();
                        }
                }

                public char this[MutableTuple<int> position] {
                        get => CurrentCharacter(position);
                        set {
                                // TODO: Inefficient; consider converting internal representation to List<char[]>
                                var arr = Content[position.Y].ToArray();
                                arr[position.X] = value;
                                Content[position.Y] = new string(arr);
                        }
                }

                public DirectionOfTravel Direction { get; set; }

                public bool CompletionSignalled { get; set; }

                public override bool Advance() {
                        ExecutionSupport.Assert(MovementVectors.ContainsKey(Direction),
                                string.Concat("Direction unknown ", Direction));
                        Fire();
                        var bundle = MovementVectors[Direction];
                        SourcePosition.X += bundle.Vector.Item1;
                        SourcePosition.Y += bundle.Vector.Item2;
                        bundle.Bounder(SourcePosition, Size);
                        return More();
                }

                public override bool More() => !CompletionSignalled;

                private void Resize() {
                        if (Content == null) base.Content = new List<string>();

                        Enumerable.Range(0, Size.Y).ToList().ForEach(i =>
                                Content.Add(new string(Enumerable.Repeat(' ', Size.X).ToArray())));
                }

                private void ResizeFromContent() =>
                        Size = new MutableTuple<int>(Content == null ? 0 : Content.Max(s => s.Length),
                                Content == null ? 0 : Content.Count);

                private void MergeContent(List<string> src) {
                        var i = 0;
                        src.ForEach(s => {
                                Content[i] = string.Concat(s, Content[i].Substring(s.Length));
                                i++;
                        });
                }

                private class VectorBundle {
                        internal Tuple<int, int> Vector { get; set; }

                        // Position, Limit
                        internal Action<MutableTuple<int>, MutableTuple<int>> Bounder { get; set; }
                }
        }
}