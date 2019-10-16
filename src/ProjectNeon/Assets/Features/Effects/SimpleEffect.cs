    using System;

    public sealed class SimpleEffect : Effect
    {
        private readonly Action<Member, Target> _apply;

        public SimpleEffect(Action<Target> apply) : this((s, t) => apply(t)) {}
        public SimpleEffect(Action<Member, Target> apply) => _apply = apply;

        public void Apply(Member source, Target target) => _apply(source, target);
    }
