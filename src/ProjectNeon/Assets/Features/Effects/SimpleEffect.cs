    using System;

    public sealed class SimpleEffect : Effect
    {
        private readonly Action<Member, Target> _apply;

        public SimpleEffect(Action<Member, MemberState> applyToOne) : this((src, t) => t.ApplyToAll(member => applyToOne(src, member))) { }
        public SimpleEffect(Action<MemberState> applyToOne) : this((src, t) => t.ApplyToAll(applyToOne)) {}
        public SimpleEffect(Action<Target> apply) : this((src, t) => apply(t)) {}
        public SimpleEffect(Action<Member, Target> apply) => _apply = apply;

        public void Apply(Member source, Target target) => _apply(source, target);
    }
