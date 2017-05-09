namespace AdditionalJoyObjects {
  /// <summary> The height this object is on a wall, used for conflicts </summary>
  public enum WallHeight {
    /// <summary> Data is missing </summary>
    None = 0,
    /// <summary> Interferes with vents </summary>
    Low = 1,
    /// <summary> Interferes with windows </summary>
    High = 2
  }
}
