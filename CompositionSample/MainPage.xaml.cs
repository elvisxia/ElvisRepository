using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Composition;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Hosting;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace CompositionSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Compositor _compositor;
        CanvasDevice _canvasDevice;
        CompositionGraphicsDevice _graphicsDevice;
        public MainPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this._compositor = ElementCompositionPreview.GetElementVisual(this).Compositor;
            base.OnNavigatedTo(e);
        }

        private void myBtn_Click(object sender, RoutedEventArgs e)
        {
            CreateDevice();
        }

        private void CreateDevice()
        {
            if (_compositor != null)
            {
                if (_canvasDevice == null)
                {
                    _canvasDevice = CanvasDevice.GetSharedDevice();
                    _canvasDevice.DeviceLost += DeviceLost; ;
                }

                if (_graphicsDevice == null)
                {
                    _graphicsDevice = CanvasComposition.CreateCompositionGraphicsDevice(_compositor, _canvasDevice);
                    _graphicsDevice.RenderingDeviceReplaced += RenderingDeviceReplaced; ;
                }
            }
        }

        //DeviceLost Handler
        private void DeviceLost(CanvasDevice sender, object args)
        {
            Debug.WriteLine("CompositionImageLoader - Canvas Device Lost");
            sender.DeviceLost -= DeviceLost;

            _canvasDevice = CanvasDevice.GetSharedDevice();
            _canvasDevice.DeviceLost += DeviceLost;

            CanvasComposition.SetCanvasDevice(_graphicsDevice, _canvasDevice);
        }

        private void RenderingDeviceReplaced(CompositionGraphicsDevice sender, RenderingDeviceReplacedEventArgs args)
        {
            //It is on this event that you should redraw/recreate any of your resources created by that device.
        }


        private async void DrewWithWin2D()
        {
            String uri = "ms-appx:///Assets/road.jpg";
            var surface = _graphicsDevice.CreateDrawingSurface(new Size(0, 0), Windows.Graphics.DirectX.DirectXPixelFormat.B8G8R8A8UIntNormalized, Windows.Graphics.DirectX.DirectXAlphaMode.Premultiplied);
            using (var canvasBitmap = await CanvasBitmap.LoadAsync(_canvasDevice, uri))
            {
                CanvasComposition.Resize(surface, canvasBitmap.Size);
                using (var session = CanvasComposition.CreateDrawingSession(surface))
                {
                    session.Clear(Color.FromArgb(0, 0, 0, 0));
                    Rect rect = new Rect(0, 0, canvasBitmap.Size.Width, canvasBitmap.Size.Height);
                    session.DrawImage(canvasBitmap, rect, rect);
                    myCanvas = canvasBitmap;
                }
            }
        }
    }
}
