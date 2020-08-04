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

namespace Interpreter.Abstractions {
        public class MutableTuple<T> {
                public MutableTuple() { }

                public MutableTuple(T x, T y, object ctx = null) {
                        X = x;
                        Y = y;
                        Context = ctx;
                }

                public MutableTuple(MutableTuple<T> obj) {
                        X = obj.X;
                        Y = obj.Y;
                }

                public T X { get; set; }

                public T Y { get; set; }

                public object Context { get; set; }

                public MutableTuple<T> Copy() => new MutableTuple<T>(X, Y, Context);

                public override string ToString() => string.Concat("x = ", X, ", y = ", Y);
        }
}