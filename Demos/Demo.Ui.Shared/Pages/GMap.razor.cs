﻿using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Components;
#if DEBUG
#endif

namespace Demo.Ui.Shared.Pages;

public partial class GMap
{
    [Parameter]
    public string Width { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await InitAsync(Element, Options);
        //Debug.WriteLine("Init finished");
        await OnAfterInit.InvokeAsync(null);
    }

    protected override void OnAfterRender(bool firstRender)
    {
        base.OnAfterRender(firstRender);
    }

    public MarkerOptions MeMarkerOptions;
    public async Task<Marker> AddMeMarker()
    {
        return meMarker = await AddMarker(MeMarkerOptions);
    }

    public async Task<Marker> AddMeMarker(double Lat, double Long, string Label, bool Draggable = false)
    {
        if (meMarker != null)
        {
            MeMarkerOptions.Label = Label;
            MeMarkerOptions.Position = new LatLngLiteral(Lat, Long);
            await meMarker.SetMap(InteropObject);
        }
        else
        {
            MeMarkerOptions = new MarkerOptions { Map = InteropObject, Label = Label, Draggable = Draggable, Position = new LatLngLiteral(Lat, Long) };
            meMarker = await AddMarker(MeMarkerOptions);
        }
        if (Draggable)
        {
            await meMarker.AddListener<MouseEvent>("dragend", async e => await OnMakerDragEnd(meMarker, e));
        }

        return meMarker;
    }

    private async Task OnMakerDragEnd(Marker meMarker, MouseEvent e)
    {
        MeMarkerOptions.Position = e.LatLng;
        await MeDragEnd.InvokeAsync(e);
    }

    private Marker meMarker;
    /// <summary>
    /// Where I am
    /// </summary>
    public Marker MeMarker => meMarker;

    /// <summary>
    /// Where I want to go.
    /// </summary>
    public Marker DestMarker;
    /// <summary>
    /// Where the truck is.
    /// </summary>
    public Marker TruckMarker;

    public GMap()
    {
        Options = new MapOptions() { Zoom = 10, Center = new LatLngLiteral(54D, -105D), MapTypeId = MapTypeId.Roadmap };
    }

    protected async Task<Marker> AddMarker(MarkerOptions TheMarkerOptions)
    {
        return await Marker.CreateAsync(JsRuntime, TheMarkerOptions);
    }

    protected async Task RemoveMarker(Marker TheMarker)
    {
        await TheMarker.SetMap(null);
    }

    public async Task MapInitialized()
    {
#if DEBUG
        Console.WriteLine("GMap Initialized:");
        Console.WriteLine($"\t Height: {Height}");
        if (Options != null)
        {
            Console.WriteLine($"\t Options: {Options}");

            Console.WriteLine($"\t Zoom: {Options.Zoom}");
            if (Options.Center.HasValue)
            {
                Console.WriteLine($"\t Center: ({Options.Center.Value.Lat}, {Options.Center.Value.Lng})");
            }
        }
#endif
        if (MeMarker != null)
        {
#if DEBUG
            var pos = await MeMarker.GetPosition();
            Console.WriteLine($"\t Me: ({pos.Lat}, {pos.Lng})");
#endif
            await AddMeMarker();
        }
    }

    /// <summary>
    /// Let me know when the drag of the Me Marker is done.
    /// </summary>
    [Parameter]
    public EventCallback<MouseEvent> MeDragEnd { get; set; }

    /// <summary>
    /// Change the center of the map.
    /// </summary>
    /// <param name="TheCenter">Location of center</param>
    /// <returns>A Task to wait on.</returns>
    public async Task CenterMap(LatLngLiteral TheCenter)
    {
        Options.Center = TheCenter;
        if (InteropObject != null)
        {
            await InteropObject.SetCenter(TheCenter);
        }
    }

    /// <summary>
    /// Change the center of the map.
    /// </summary>
    /// <param name="Lat">Latitude of new center.</param>
    /// <param name="Long">Longitude of new center.</param>
    /// <returns>A Task to wait on.</returns>
    public async Task CenterMap(double Lat, double Long)
    {
        await CenterMap(new LatLngLiteral(Lat, Long));
    }
}