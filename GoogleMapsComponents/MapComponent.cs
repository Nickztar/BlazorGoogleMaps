using GoogleMapsComponents.Maps;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace GoogleMapsComponents;

public class MapComponent : ComponentBase, IDisposable, IAsyncDisposable
{
    private bool _isDisposed;

    [Inject]
    public IJSRuntime JsRuntime { get; protected set; } = default!;
    [Inject]
    public IServiceProvider ServiceProvider { get; protected set; } = default!;

    private IBlazorGoogleMapsKeyService? _keyService;

    protected override void OnInitialized()
    {
        // get the service from the provider instead of with [Inject] in case no 
        // service was registered. e.g. when the user loads the api with a script tag.
        _keyService = ServiceProvider.GetService<IBlazorGoogleMapsKeyService>();
        base.OnInitialized();
    }

    private Map? _interopObject { get; set; }
    public Map InteropObject => _interopObject ?? default!;

    public async Task InitAsync(ElementReference element, MapOptions? options = null)
    {
        if (options?.ApiLoadOptions == null && _keyService != null && !_keyService.IsApiInitialized)
        {
            _keyService.IsApiInitialized = true;
            options ??= new MapOptions();
            options.ApiLoadOptions = await _keyService.GetApiOptions();
        }

        _interopObject = await Map.CreateAsync(JsRuntime, element, options);
    }

    public async ValueTask DisposeAsync()
    {
        // Perform async cleanup.
        await DisposeAsyncCore();

        // Dispose of unmanaged resources.
        Dispose(false);

        // Suppress finalization.
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (_interopObject is not null)
        {
            try
            {
                await InteropObject.DisposeAsync();
                _interopObject = null;
            }
            catch (Exception ex)
            {
                var isPossibleRefreshError = ex.HasInnerExceptionsOfType<TaskCanceledException>();
                isPossibleRefreshError |= ex.HasInnerExceptionsOfType<ObjectDisposedException>();
                //Unfortunately, JSDisconnectedException is available in dotnet >= 6.0, and not in dotnet standard.
                isPossibleRefreshError |= true;
                //If we get an exception here, we can assume that the page was refreshed. So assentialy, we swallow all exception here...
                //isPossibleRefreshError = isPossibleRefreshError || ex.HasInnerExceptionsOfType<JSDisconnectedException>();


                if (!isPossibleRefreshError)
                {
                    throw;
                }
            }
        }
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            if (disposing)
            {
                InteropObject?.Dispose();
            }

            // TODO: free unmanaged resources (unmanaged objects) and override finalizer
            // TODO: set large fields to null
            _isDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}