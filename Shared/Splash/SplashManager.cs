using System;
using System.Threading;
using System.Windows.Forms;

namespace Pulsar.Shared.Splash;

public class SplashManager
{
    public static SplashManager Instance = null;
    public float BarValue => splash.IsDisposed ? float.NaN : splash.BarValue;

    private readonly ManualResetEventSlim ready = new();
    private readonly Thread thread;
    private SplashScreen splash;

    public SplashManager()
    {
        thread = new Thread(() =>
        {
            splash = new SplashScreen();
            ready.Set();
            Application.Run(splash);
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.IsBackground = true;
        thread.Start();
        ready.Wait();
    }

    private void SafeInvoke(Action action)
    {
        try
        {
            if (splash.IsDisposed) return;
            splash.Invoke(action);
        }
        catch (ObjectDisposedException) { }
        catch (InvalidOperationException) { }
    }

    public void SetText(string msg) => SafeInvoke(() => splash.SetText(msg));

    public void SetBarValue(float ratio = float.NaN) =>
        SafeInvoke(() => splash.SetBarValue(ratio));

    public void SetTitle(string title) => SafeInvoke(() => splash.Text = title);

    public void Delete()
    {
        Instance = null;
        SafeInvoke(splash.Delete);
    }
}
