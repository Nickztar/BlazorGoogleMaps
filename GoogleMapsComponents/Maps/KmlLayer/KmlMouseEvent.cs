namespace GoogleMapsComponents.Maps.KmlLayer;

public class KmlMouseEvent
{
    /// <summary>
    /// A KmlFeatureData object, containing information about the clicked feature.
    /// </summary>
    public KmlFeatureData FeatureData { get; set; } = default!;
    /// <summary>
    /// The position at which to anchor an infowindow on the clicked feature.
    /// </summary>
    public LatLngLiteral LatLng { get; set; } = default!;
    /// <summary>
    /// The offset to apply to an infowindow anchored on the clicked feature.
    /// </summary>
    public Size Size { get; set; } = default!;
}