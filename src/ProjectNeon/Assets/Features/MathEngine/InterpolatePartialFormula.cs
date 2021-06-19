using System;

[Serializable]
public class InterpolatePartialFormula
{
    public bool IsSupplied => EvaluationPartialFormula.Length > 0;
    
    public string EvaluationPartialFormula = "";
    public string Prefix = "";
    public string Suffix = "";
}