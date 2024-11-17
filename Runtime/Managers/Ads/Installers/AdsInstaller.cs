namespace Game.Ads.Installers
{
public class AdsInstaller
{
    public static IAdsManager Plug() => new AdsPlug();
}
}