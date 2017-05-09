using Verse;

namespace AdditionalJoyObjects {
  /// <summary>
  /// Supplies CompHanger with hanging information
  /// </summary>
  public class CompProperties_Hanger : CompProperties {

    /// <summary>
    /// What is this object hanging on?
    /// </summary>
    public HangingType hangingType = HangingType.None;

    /// <summary>
    /// If this is on a wall, how high is it?
    /// </summary>
    public WallHeight wallHeight = WallHeight.None;

    /// <summary></summary>
    public CompProperties_Hanger() {
      compClass = typeof(CompHanger);
    }
  }
}
