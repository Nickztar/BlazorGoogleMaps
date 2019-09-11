# BlazorGoogleMaps
Blazor interop for GoogleMap library

## Usage
1. Add google map script tag to wwwroot/index.html in Client side or _Host.cshtml in Server Side
```
<script type="text/javascript" src="https://maps.googleapis.com/maps/api/js?key=YOUR_KEY_GOES_HERE&v=3"></script>
```
For servers side also needed to link to recourse manually in preview8. This could change in future releases. Add this line into _Host.cshtml
```
<script src="_content/BlazorGoogleMaps/objectManager.js"></script>
```

2. Use component in client and server side same
```
@page "/map"
@using GoogleMapsComponents
@using GoogleMapsComponents.Maps

<h1>Google Map</h1>

<GoogleMap @ref:suppressField @ref="@map1" Id="map1" Options="@mapOptions"></GoogleMap>

@functions {
	private GoogleMap map1;
	private MapOptions mapOptions;	

	protected override void OnInitialized()
	{
		mapOptions = new MapOptions()
		{
			Zoom = 13,
			Center = new LatLngLiteral()
			{
				Lat = 13.505892,
				Lng = 100.8162
			},
			MapTypeId = MapTypeId.Roadmap
		};
	}		
}

```
## Current status
* Map
* Marker
* InfoWindow
* Polygon, LineString, Rectangle, Circle

## Todo
* Data 
* StreetView
* Places
* Routes