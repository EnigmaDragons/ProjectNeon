
public sealed class RecurrentEffect :  Effect
{
    private int _repetitions;
    private int _count;
    private Effect _effect;

    public RecurrentEffect(Effect origin, int times)
    {
        _effect = origin;
        _repetitions = times;
        _count = 0;
    }

    public void Apply(Member source, Target target)
    {
        if (_count < _repetitions)
        {
            _effect.Apply(source, target);
            _count++;
        }
        /**
         * @todo #378:30min Remove Effect from Effect stack once it reaches its repetition number.
         *  Currently RecurrentEffect just does not remove effect from stack: it continues to be executed, 
         *  but only the Aplly method is checked. An effect should be removed from the character once it expires
         *  (i.e it was executed _repetition times)
         */
    }
}
