//============================================================================================================================================================================
// Copyright (c) 2011-2013 Tony Beveridge
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software 
// without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit 
// persons to whom the Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//============================================================================================================================================================================

using System;
using System.Collections.Generic;

namespace Interpreter.Abstractions {
        #region Source code implementations

        public abstract class SourceCode {
                protected SourceCode() => SourcePosition = new MutableTuple<int>();

                public virtual List<string> Content { get; set; }

                public MutableTuple<int> SourcePosition { get; set; }

                public bool CachingEnabled { get; set; }

                public event EventHandler<SourceCodeEventArgs<SourceCode>> SourceEvent;

                public string Current() => Content[SourcePosition.Y].Substring(SourcePosition.X, 1);

                public string Peek() {
                        string res = null;
                        if (More() && Advance()) {
                                res = Current();
                                Backup();
                        }

                        return res;
                }

                public virtual char CurrentCharacter() => CurrentCharacter(SourcePosition);

                protected virtual char CurrentCharacter(MutableTuple<int> position) => Content[position.Y][position.X];

                public abstract bool Advance();

                public virtual bool Backup() => throw new NotImplementedException("This type does not Backup()");

                public string AdvanceAndReturn() {
                        var more = Advance();
                        return more ? Current() : string.Empty;
                }

                public abstract bool More();

                protected void Fire() =>
                        SourceEvent?.Invoke(this, new SourceCodeEventArgs<SourceCode> { Source = this });
        }

        #endregion
}