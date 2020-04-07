using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace VisualFFmpeg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string SavePath = "";       
        public MainWindow()
        {
            InitializeComponent();

        }
        private void loadffmpeg_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadFile = new OpenFileDialog();
            loadFile.Title = "Select FFmpeg Path";
            loadFile.Filter = "ffmpeg |ffmpeg.exe";
            loadFile.InitialDirectory = "D:\\";
            loadFile.ShowDialog();
            ffmpegPathLabel.Content = loadFile.FileName;
        }

        private void loadFile_btn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog loadFile = new OpenFileDialog();
            loadFile.Title = "Select FFmpeg Path";
            loadFile.InitialDirectory = @"D:\ffmpeg\bin";
            loadFile.ShowDialog();
            loadFile_lbl.Content = loadFile.FileName;
        }

        private void saveFile_btn_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Title = "Save As";
            saveFile.Filter = "mp3 audio (*.mp3)|*.mp3 |mp4 video (*.mp4)|*.mp4|All files (*.*)|*.*";
            saveFile.ShowDialog();      
            SavePath = saveFile.FileName;
            MessageBox.Show(SavePath);

        }

        private void convert_btn_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string> {
                {"ffmpegpath", ffmpegPathLabel.Content.ToString() },
                {"inputFilePath",loadFile_lbl.Content.ToString() },
                {"outputFilePath",SavePath }
            };
            convert_btn.IsEnabled = false;
            Thread ffmpeg = new Thread(ffmpegAction);
            ffmpeg.Start(keyValuePairs);
        }

        private void ffmpegAction(object args)
        {
            Dictionary<string,string> parameters = (Dictionary<string, string>)args;
           
            ffmpegHandeler ffmpeg = new ffmpegHandeler(parameters["ffmpegpath"],parameters["inputFilePath"], parameters["outputFilePath"]);
            ffmpeg.PercantageChange += update_load;
            ffmpeg.StartConverion();
            convert_btn.Dispatcher.BeginInvoke((Action)(() => convert_btn.IsEnabled = true));
            
        }



        private void update_load(object sender, EventArgs e)
        {

            percent_num_lbl.Dispatcher.BeginInvoke((Action)(() => percent_num_lbl.Content = Math.Round(Convert.ToDouble(e.ToString()), 2)));
            percent_bar.Dispatcher.BeginInvoke((Action)(() => percent_bar.Value = Convert.ToDouble(e.ToString())));

        }
    }


}
