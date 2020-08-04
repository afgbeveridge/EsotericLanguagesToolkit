namespace Interpreter.Abstractions {
        public class BaseCommand<T> : BaseObject {
                public BaseCommand(T cmd, string keyWord) {
                        Command = cmd;
                        KeyWord = keyWord;
                }

                public string KeyWord { get; }

                public T Command { get; set; }

                protected void Record() => Statistics.Increment(KeyWord);

                public override string ToString() => GetType().Name + " - command " + KeyWord;
        }
}