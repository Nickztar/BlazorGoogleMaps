using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace GoogleMapsComponents.Maps;

public class Projection : IDisposable
{
    protected readonly JsObjectRef _jsObjectRef;

    public Guid Guid => _jsObjectRef.Guid;

    public Projection(IJSRuntime jsRuntime, Guid id)
    {
        _jsObjectRef = new JsObjectRef(jsRuntime, id);
    }

    public async Task<Point?> FromLatLngToPoint(LatLngLiteral literal)
    {
        return await _jsObjectRef.InvokeAsync<Point>("fromLatLngToPoint", literal);
    }

    public async Task<LatLngLiteral?> FromPointToLatLng(Point point)
    {
        return await _jsObjectRef.InvokeAsync<LatLngLiteral>("fromPointToLatLng", point);
    }

    public void Dispose()
    {
        _jsObjectRef?.Dispose();
    }
}