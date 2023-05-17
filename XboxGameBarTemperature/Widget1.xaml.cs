using Microsoft.Gaming.XboxGameBar;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace XboxGameBarTemperature
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
            widget.MaxWindowSize = new Windows.Foundation.Size(80, 18);
            widget.MinWindowSize = new Windows.Foundation.Size(80, 18);
        }
        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimerTick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 500);
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
