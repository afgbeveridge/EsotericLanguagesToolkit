using System;

namespace General.Language {

        [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
        public class CommandNatureAttribute : Attribute {

                public LanguageCommandGroup Nature { get; }

                public CommandNatureAttribute(LanguageCommandGroup grp) => Nature = grp;

        }
}
