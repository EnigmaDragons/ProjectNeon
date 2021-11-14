using System;
using System.Linq;
using NUnit.Framework;

public class CutsceneSegmentCreationTests
{
    [Test]
    public void CutsceneSegments_CanCreateAll()
    {
        var types = Enum.GetValues(typeof(CutsceneSegmentType)).Cast<CutsceneSegmentType>().Select(t => new CutsceneSegmentData { SegmentType = t});

        foreach (var segmentData in types)
        {
            try
            {
                Assert.IsNotNull(AllCutsceneSegments.Create(segmentData), $"Could not create Effect of Type '{segmentData.SegmentType.ToString()}'");
            }
            catch (Exception e)
            {
                Assert.Fail($"Could not create Effect of Type '{segmentData.SegmentType.ToString()}' - {e.Message}");
            }
        }
    }
}
