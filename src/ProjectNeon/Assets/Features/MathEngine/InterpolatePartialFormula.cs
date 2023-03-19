using System;

[Serializable]
public class InterpolatePartialFormula
{
    public bool IsSupplied => EvaluationPartialFormula.Length > 0 || Prefix.Length > 0 || Suffix.Length > 0 || PrefixTerm.Length > 0 || SuffixTerm.Length > 0;
    
    public string EvaluationPartialFormula = "";
    public string Prefix = "";
    public string Suffix = "";
    public string PrefixTerm = "";
    public string SuffixTerm = "";

    public string LocalizedPrefixTerm => string.IsNullOrWhiteSpace(PrefixTerm) ? "" : $"CardInterpolations/Prefix-{PrefixTerm}".ToLocalized();
    public string LocalizedSuffixTerm => string.IsNullOrWhiteSpace(SuffixTerm) ? "" : $"CardInterpolations/Suffix-{SuffixTerm}".ToLocalized();
}