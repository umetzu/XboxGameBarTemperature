using Microsoft.Gaming.XboxGameBar;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace XboxGameBarTemperature
{
    public sealed partial class Widget1 : Page
    {
        DispatcherTimer dispatcherTimer;
        readonly HardwareInfoService hardwareInfoService = new();
        private XboxGameBarWidget widget = null;

        public Widget1()
        {
            InitializeComponent();
            BackgroundGrid.Opacity = 0;
            DispatcherTimerSetup();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            widget = e.Parameter as XboxGameBarWidget;
            widget.MaxWindowSize = new Windows.Foundation.Size(148, 18);
            widget.MinWindowSize = new Windows.Foundation.Size(148, 18);
        }
        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
            dispatcherTimer.Start();
        }

        async void DispatcherTimerTick(object sender, object e)
        {
            float? temperature = await hardwareInfoService.ReadGpuTemperature();

            await myTextBlock.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                myTextBlock.Text = $"GPU {temperature} °C";
            });
        }        
    }
}
