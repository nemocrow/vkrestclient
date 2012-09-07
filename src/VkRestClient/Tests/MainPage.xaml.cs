using Microsoft.Phone.Controls;
using Microsoft.Silverlight.Testing;

namespace Tests
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();

            var settings = UnitTestSystem.CreateDefaultSettings();
            //settings.StartRunImmediately = true;
            //settings.ShowTagExpressionEditor = false;

            Content = UnitTestSystem.CreateTestPage(settings);
            var p = (IMobileTestPage)Content;
            BackKeyPress += (x, xe) => { xe.Cancel = p.NavigateBack(); };
        }
    }
}