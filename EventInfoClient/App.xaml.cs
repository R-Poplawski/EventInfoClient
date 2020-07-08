using System.Windows;

namespace EventInfoClient
{
    /// <summary>
    /// Logika interakcji dla klasy App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static EventInfoAPI ServiceClient { get; private set; }

        public App()
        {
            EventInfoAPI.InitializeClient();
        }
    }
}
