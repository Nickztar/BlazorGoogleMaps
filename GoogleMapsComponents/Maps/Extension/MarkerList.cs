using Microsoft.JSInterop;
using OneOf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoogleMapsComponents.Maps.Extension;

/// <summary>
/// <para>
/// A class able to manage a lot of Marker objects and get / set their properties at the same time, eventually with different values
/// </para>
/// <para>
/// Main concept is that for each Marker to be distinguished from other ones, it needs
/// to have a "unique key" with an "external world meaning", so not necessarily a GUID.
/// </para>
/// <para>
/// In real cases Markers are likely to be linked to places, activities, transit stops and so on -> So, what better way to choose as Marker "unique key" the "id" of the object each marker is related to?
/// A string key has been selected as type due to its implicit versatility.
/// </para>
/// <para>
/// To create Markers, simply call <c>MarkerList.CreateAsync</c> with a Dictionary of desired Marker keys and MarkerOptions values.
/// After that, a new instance of MarkerList class will be returned with its Markers dictionary member populated with the corresponding results
/// </para>
/// <para>
/// At run-time is possible to:<br/>
/// <list type="number">
/// <item><description>add Marker to the same MarkerList class using <c>AddMultipleAsync</c> method (only keys not matching with existent Marker keys will be created)<br/>
/// Markers dictionary will contain "union distinct" of existent Marker's keys and new keys</description>
/// </item>
/// <item><description>remove Marker from the MarkerList class (only Marker having keys matching with existent keys will be removed)<br/>
/// Markers dictionary will contain "original - required and found" Marker's keys (eventually any is all Marker are removed)</description>
/// </item>
/// </list>
/// </para>
/// <para>
/// Each definer getter properties can be used as follows:<br/>
/// a) without parameter -> all eventually defined markers related property will be returned (if any)<br/>
/// b) with a <c>List&lt;string&gt;</c> of keys -> all eventually matching keys with Markers Dictionary keys produces related markers property extraction (if any defined)
/// </para>
/// <para>
/// Each setter properties can be used as follows:<br/>
/// With a <c>Dictionary&lt;string, {property type}&gt;</c> indicating for each Marker (related to that key) the corresponding related property value.
/// </para>
/// </summary>
public class MarkerList : ListableEntityListBase<Marker, MarkerOptions>
{
    public Dictionary<string, Marker> Markers => BaseListableEntities;

    /// <summary>
    /// Create markers list
    /// </summary>
    /// <param name="jsRuntime"></param>
    /// <param name="opts">Dictionary of desired Marker keys and MarkerOptions values. Key as any type unique key. Not necessarily Guid</param>
    /// <returns>New instance of MarkerList class will be returned with its Markers dictionary member populated with the corresponding results</returns>
    public static async Task<MarkerList> CreateAsync(IJSRuntime jsRuntime, Dictionary<string, MarkerOptions> opts)
    {
        var jsObjectRef = new JsObjectRef(jsRuntime, Guid.NewGuid());

        Dictionary<string, JsObjectRef> jsObjectRefs = await JsObjectRef.CreateMultipleAsync(
            jsRuntime,
            "google.maps.Marker",
            opts.ToDictionary(e => e.Key, e => (object)e.Value));

        Dictionary<string, Marker> objs = jsObjectRefs.ToDictionary(e => e.Key, e => new Marker(e.Value));
        var obj = new MarkerList(jsObjectRef, objs);

        return obj;
    }

    /// <summary>
    /// Sync list over lifetime: Create and remove list depending on entity count; 
    /// entities will be removed, added or changed to mirror the given set.
    /// </summary>
    /// <param name="list">
    /// The list to manage. May be null.
    /// </param>
    /// <param name="jsRuntime"></param>
    /// <param name="opts"></param>
    /// <param name="clickCallback"></param>
    /// <returns>
    /// The managed list. Assign to the variable you used as parameter.
    /// </returns>
    public static async Task<MarkerList?> SyncAsync(MarkerList? list,
        IJSRuntime jsRuntime,
        Dictionary<string, MarkerOptions> opts,
        Action<MouseEvent, string, Marker>? clickCallback = null)
    {
        if (opts.Count == 0)
        {
            if (list == null) return list;
            await list.SetMultipleAsync(opts);
            list = null;
        }
        else
        {
            if (list == null)
            {
                list = await CreateAsync(jsRuntime, new Dictionary<string, MarkerOptions>());
                if (clickCallback != null)
                {
                    list.EntityClicked += (_, e) =>
                    {
                        clickCallback(e.MouseEvent, e.Key, e.Entity);
                    };
                }
            }
            await list.SetMultipleAsync(opts);
        }
        return list;
    }

    private MarkerList(JsObjectRef jsObjectRef, Dictionary<string, Marker> markers)
        : base(jsObjectRef, markers, js => new Marker(js))
    {
    }

    /// <summary>
    /// Set the set of entities; entities will be removed, added or changed to mirror the given set.
    /// </summary>
    /// <param name="opts"></param>
    /// <returns></returns>
    public async Task SetMultipleAsync(Dictionary<string, MarkerOptions> opts)
    {
        await base.SetMultipleAsync(opts, "google.maps.Marker");
    }

    /// <summary>
    /// Only keys not matching with existent Marker keys will be created
    /// </summary>
    /// <returns></returns>
    public async Task AddMultipleAsync(Dictionary<string, MarkerOptions> opts)
    {
        await base.AddMultipleAsync(opts, "google.maps.Marker");
    }
    
    public Task<Dictionary<string, Animation>> GetAnimations(List<string>? filterKeys = null)
    {
        return GetKeysAsync<string, Animation>("getAnimation", Helper.ToEnum<Animation>, filterKeys);
    }

    public Task<Dictionary<string, bool>> GetClickables(List<string>? filterKeys = null)
    {
        return GetKeysAsync<bool, bool>("getClickable", r => r, filterKeys);
    }

    public Task<Dictionary<string, string>> GetCursors(List<string>? filterKeys = null)
    {
        return GetKeysAsync<string, string>("getCursor", r => r, filterKeys);
    }

    public Task<Dictionary<string, OneOf<string, Icon, Symbol>>> GetIcons(List<string>? filterKeys = null)
    {
        return GetKeysAsync<OneOf<string, Icon, Symbol>, OneOf<string, Icon, Symbol>>("getIcon", r => r, filterKeys);
    }

    public Task<Dictionary<string, string>> GetLabels(List<string>? filterKeys = null)
    {
        return GetKeysAsync<string, string>("getLabel", r => r, filterKeys);
    }

    public Task<Dictionary<string, LatLngLiteral>> GetPositions(List<string>? filterKeys = null)
    {
        return GetKeysAsync<LatLngLiteral, LatLngLiteral>("getPosition", r => r, filterKeys);
    }

    public Task<Dictionary<string, MarkerShape>> GetShapes(List<string>? filterKeys = null)
    {
        return GetKeysAsync<MarkerShape, MarkerShape>("getShape", r => r, filterKeys);
    }

    public Task<Dictionary<string, string>> GetTitles(List<string>? filterKeys = null)
    {
        return GetKeysAsync<string, string>("getTitle", r => r, filterKeys);
    }

    public Task<Dictionary<string, int>> GetZIndexes(List<string>? filterKeys = null)
    {
        return GetKeysAsync<int, int>("getZIndex", r => r, filterKeys);
    }

    /// <summary>
    /// Start an animation. 
    /// Any ongoing animation will be cancelled. 
    /// Currently supported animations are: BOUNCE, DROP. 
    /// Passing in null will cause any animation to stop.
    /// </summary>
    /// <param name="animations"></param>
    public Task SetAnimations(Dictionary<string, Animation?> animations)
    {
        var dictArgs = animations.ToDictionary(e => Markers[e.Key].Guid, e => (object?)GetAnimationCode(e.Value));
        return _jsObjectRef.InvokeMultipleAsync("setAnimation", dictArgs);
    }

    public int? GetAnimationCode(Animation? animation)
    {
        switch (animation)
        {
            case null: return null;
            case Animation.Bounce: return 1;
            case Animation.Drop: return 2;
            default: return 0;
        }
    }

    /// <summary>
    /// Sets the Clickable flag of one or more Markers to match a dictionary of marker keys and flag values.
    /// </summary>
    /// <param name="flags"></param>
    /// <returns></returns>
    public Task SetClickables(Dictionary<string, bool> flags)
    {
        return _jsObjectRef.InvokeMultipleAsync("setClickable", ToJsDictionary(flags));
    }

    public Task SetCursors(Dictionary<string, string> cursors)
    {
        return _jsObjectRef.InvokeMultipleAsync("setCursor", ToJsDictionary(cursors));
    }

    /// <summary>
    /// Set Icon on each Marker matching a param dictionary key to the param value with single JSInterop call.
    /// </summary>
    /// <param name="icons"></param>
    /// <returns></returns>
    public Task SetIcons(Dictionary<string, OneOf<string, Icon, Symbol>> icons)
    {
        return _jsObjectRef.InvokeMultipleAsync(
            "setIcon",
            ToJsDictionary(icons));
    }

    /// <inheritdoc cref="SetIcons(Dictionary{string, OneOf{string, Icon, Symbol}})"/>
    public Task SetIcons(Dictionary<string, string> icons)
    {
        return _jsObjectRef.InvokeMultipleAsync("setIcon", ToJsDictionary(icons));
    }

    /// <inheritdoc cref="SetIcons(Dictionary{string, OneOf{string, Icon, Symbol}})"/>
    public Task SetIcons(Dictionary<string, Icon> icons)
    {
        return _jsObjectRef.InvokeMultipleAsync("setIcon", ToJsDictionary(icons));
    }

    /// <summary>
    /// Set Label on each Marker matching a param dictionary key to the param value with single JSInterop call.
    /// </summary>
    /// <param name="labels"></param>
    /// <returns></returns>
    public Task SetLabels(Dictionary<string, OneOf<string, MarkerLabel>> labels)
    {
        return _jsObjectRef.InvokeMultipleAsync(
            "setLabel",
            ToJsDictionary(labels));
    }

    /// <inheritdoc cref="SetLabels(Dictionary{string, OneOf{string, MarkerLabel}})"/>
    public Task SetLabels(Dictionary<string, string> labels)
    {
        return _jsObjectRef.InvokeMultipleAsync(
            "setLabel",
            ToJsDictionary(labels));
    }

    /// <inheritdoc cref="SetLabels(Dictionary{string, OneOf{string, MarkerLabel}})"/>
    public Task SetLabels(Dictionary<string, MarkerLabel> labels)
    {
        return _jsObjectRef.InvokeMultipleAsync(
            "setLabel",
            ToJsDictionary(labels));
    }

    public Task SetOpacities(Dictionary<string, float> opacities)
    {
        return _jsObjectRef.InvokeMultipleAsync(
            "setOpacity",
            ToJsDictionary(opacities));
    }

    public Task SetPositions(Dictionary<string, LatLngLiteral> latLngs)
    {
        return _jsObjectRef.InvokeMultipleAsync("setPosition", ToJsDictionary(latLngs));
    }

    public Task SetShapes(Dictionary<string, MarkerShape> shapes)
    {
        return _jsObjectRef.InvokeMultipleAsync("setShape", ToJsDictionary(shapes));
    }

    public Task SetTitles(Dictionary<string, string> titles)
    {
        return _jsObjectRef.InvokeMultipleAsync("setTitle", ToJsDictionary(titles));
    }

    public Task SetZIndexes(Dictionary<string, int> zIndexes)
    {
        return _jsObjectRef.InvokeMultipleAsync("setZIndex", ToJsDictionary(zIndexes));
    }
}