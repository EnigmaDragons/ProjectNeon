using System;

/**
 * Wraps the execution of an effect so it is only executed a certain number of times.
 */

[Obsolete]
public sealed class Recurrent :  Effect
{
    /**
     * Number of times the effect will be executed
     */
    private int _repetitions;

    /**
     * Tracks how many times the effect has already been executed.
     */
    private int _count;

    /**
     * The wrapped effect.
     */
    private Effect _effect;

    public Recurrent(Effect origin, int times)
    {
        _effect = origin;
        _repetitions = times;
        _count = 0;
    }

    public Recurrent(Effect origin) : this(origin, 1)
    {

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
