
public class InterpolateFriendlyFormula
{
    public string FullFormula;
    public InterpolatePartialFormula InterpolatePartialFormula;
    
    public bool ShouldUsePartialFormula => InterpolatePartialFormula != null && InterpolatePartialFormula.IsSupplied;
}