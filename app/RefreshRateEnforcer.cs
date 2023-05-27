using GHelper;
using System.Diagnostics;
using System.Security.Principal;

public class RefreshRateEnforcer
{
    private static int enforcedRefreshRate 
    {
        get { return AppConfig.getConfig("enforced_rr", 0); }
    }

    public static bool IsEnforced()
    {
        return enforcedRefreshRate != 0;
    }

    public static void Enable(int toRefreshRate)
    {
        AppConfig.setConfig("enforced_rr", toRefreshRate);
    }

    public static void Disable()
    {
        AppConfig.setConfig("enforced_rr", 0);
    }

    public static void PowerModeChangeHook()
    {
        System.Threading.Thread thread = new System.Threading.Thread(WaitAndSetRefreshRate);
        thread.Start();
    }

    private static void WaitAndSetRefreshRate()
    {
        Stopwatch sw = new Stopwatch();
        sw.Start();

        while (NativeMethods.GetRefreshRate() == enforcedRefreshRate) {
            if (sw.ElapsedMilliseconds > 15000) {break;}
        }

        NativeMethods.SetRefreshRate(enforcedRefreshRate);
        Logger.WriteLine("Enforced Refresh Rate: "+ NativeMethods.GetRefreshRate().ToString());
    }
}
