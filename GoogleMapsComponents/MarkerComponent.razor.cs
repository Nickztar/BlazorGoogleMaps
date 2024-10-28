using System;
using System.Threading.Tasks;
using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace GoogleMapsComponents;

public partial class MarkerComponent
{
    private readonly Guid _id;
    private readonly string _componentId;
    public MarkerComponent()
    {
        _id = Guid.NewGuid();
        _componentId = $"marker_{_id}";
    }
    [Inject] 
    private IJSRuntime JS { get; set;  }

    protected override bool ShouldRender() => false;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && Map != null && !hasrender)
        {
            await JS.InvokeAsync<string?>("blazorGoogleMaps.objectManager.addAdvancedComponent", _id, new Options()
            {
                Position = new LatLngLiteral(Lat, Lng),
                MapId = Map.Guid
            });
            hasrender = true;
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private bool hasrender = false;
    protected override async Task OnParametersSetAsync()
    {
        if (Map != null && !hasrender)
        {
            await JS.InvokeAsync<string?>("blazorGoogleMaps.objectManager.addAdvancedComponent", _id, new Options()
            {
                Position = new LatLngLiteral(Lat, Lng),
                MapId = Map.Guid
            });
            hasrender = true;
        }

        await base.OnParametersSetAsync();
    }

    [CascadingParameter(Name = "Map")]
    private Map? Map { get; set; }
    
    [Parameter]
    public RenderFragment ChildContent { get; set;  }
    
    [Parameter]
    public double Lat { get; set;  }
    
    [Parameter]
    public double Lng { get; set;  }

    internal class Options
    {
        public Guid? MapId { get; set; }
        public required LatLngLiteral Position { get; set; }
    }
}