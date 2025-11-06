using System;
using System.Collections.Generic;
using System.Linq;
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

namespace AioStudy.UI.Views
{
    /// <summary>
    /// Interaction logic for PomodoroView.xaml
    /// </summary>
    public partial class PomodoroView : UserControl
    {
        public PomodoroView()
        {
            InitializeComponent();
            //Loaded += MainWindow_Loaded;
        }


//        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
//        {
//            try
//            {
//                await DotwaveView.EnsureCoreWebView2Async();

//                DotwaveView.DefaultBackgroundColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);

//                DotwaveView.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;

//                DotwaveView.CoreWebView2.Settings.IsStatusBarEnabled = false;

//                DotwaveView.CoreWebView2.Settings.IsZoomControlEnabled = false;

//                DotwaveView.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;

//                DotwaveView.CoreWebView2.Settings.AreBrowserAcceleratorKeysEnabled = false;

//                string html = @"
//<!DOCTYPE html>
//<html lang='en'>
//<head>
//<meta charset='UTF-8'>
//<style>
//  html, body {
//    margin: 0;
//    padding: 0;
//    width: 100%;
//    height: 100%;
//    background: transparent;
//    overflow: hidden;
//  }
//  canvas {
//    display: block;
//  }
//</style>
//</head>
//<body>
//<canvas id='wave'></canvas>
//<script>
//const canvas = document.getElementById('wave');
//const ctx = canvas.getContext('2d');

//let w, h;
//let points = [];
//const cols = 90;
//const rows = 45;
//const spacing = 25;
//const amplitude = 25;
//const speed = 0.005;
//const perspective = 0.7; // Schrägansicht
//let t = 0;

//function resize() {
//  w = canvas.width = window.innerWidth;
//  h = canvas.height = window.innerHeight;
//  points = [];
//  for (let y = 0; y < rows; y++) {
//    for (let x = 0; x < cols; x++) {
//      points.push({
//        x: (x - cols / 2) * spacing,
//        y: (y - rows / 2) * spacing
//      });
//    }
//  }
//}
//window.addEventListener('resize', resize);
//resize();

//function draw() {
//  ctx.clearRect(0, 0, w, h);
//  ctx.save();
//  ctx.translate(w / 2, h / 2 + 100);
//  ctx.rotate(-0.2); // leicht gekippt
//  ctx.fillStyle = 'white'; // weiße Punkte

//  for (const p of points) {
//    const z = Math.sin((p.x * 0.05) + t) * Math.cos((p.y * 0.05) + t * 0.5) * amplitude;
//    const screenX = p.x;
//    const screenY = p.y * Math.cos(perspective) + z * Math.sin(perspective);

//    // Tiefe durch Skalierung und Alpha
//    const depth = 1 - (z / (amplitude * 2));
//    const alpha = 0.6 + (1 - depth) * 0.4;

//    ctx.beginPath();
//    ctx.globalAlpha = alpha;
//    ctx.arc(screenX, screenY, 2.2 * depth, 0, Math.PI * 2);
//    ctx.fill();
//  }

//  ctx.restore();
//  ctx.globalAlpha = 1;
//  t += speed;
//  requestAnimationFrame(draw);
//}
//draw();
//</script>
//</body>
//</html>";




//                DotwaveView.NavigateToString(html);
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show("Fehler beim Laden des WebView2: " + ex.Message);
//            }
//        }
    }
}
