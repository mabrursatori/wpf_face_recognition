using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Reg;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace Face_Rec
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private VideoCapture _capture;
        private CascadeClassifier _cascadeClassifier;
        private DispatcherTimer _timer;
        public MainWindow()
        {
            InitializeComponent();


            Console.WriteLine("TEST");
            runDetector();
        }

        async void runDetector()
        {
            await Task.Run(() =>
            {
              
                _capture = new VideoCapture(0);
              
                this.Dispatcher.Invoke(() =>
                {

                    loading.Visibility = Visibility.Collapsed;
                    string appFolderPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\haarcascade_frontalface_alt2.xml";
                   
                    _cascadeClassifier = new CascadeClassifier(appFolderPath);

                   
                    _timer = new DispatcherTimer();
                    _timer.Tick += new EventHandler(Timer_Tick);
                    _timer.Interval = new TimeSpan(0, 0, 0, 0, 1);
                    _timer.Start();
                });
               
                
            });
           
        }

        
        private void Timer_Tick(object sender, EventArgs e)
        {
            var mat = _capture.QueryFrame();
            if(mat != null)
            {
                Image<Bgr, byte> nextFrame = mat.ToImage<Bgr, Byte>();
                Image<Gray, byte> grayframe = nextFrame.Convert<Gray, byte>();
                var faces = _cascadeClassifier.DetectMultiScale(grayframe, 1.1, 10, System.Drawing.Size.Empty);
                if(faces != null && faces.Length > 0)
                {
                    result.Content = "ADA PENAMPAKAN!!";
                }
                else
                {
                    result.Content = "-----";
                }
                foreach (var face in faces)
                {
                    nextFrame.Draw(face, new Bgr(System.Drawing.Color.BurlyWood), 3);

                }

                imageBox.Source = ToBitmapSource(nextFrame);
            }
            
        }

        public BitmapSource ToBitmapSource(Image<Bgr, byte> image)
        {
            using (Bitmap source = image.ToBitmap())
            {
                IntPtr ptr = source.GetHbitmap();
                BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                    ptr,
                    IntPtr.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                DeleteObject(ptr);
                return bs;
            }
        }

        [DllImport("gdi32")]
        private static extern int DeleteObject(IntPtr o);

    }
}
