using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace VisualFFmpeg
{
    public class PercentageEventArgs : EventArgs
    {
        public double percentage { set; get; }
        public PercentageEventArgs(double percentage)
        {
            this.percentage = percentage;
        }

        public override string ToString()
        {
            return percentage.ToString();
        }
    }
    

    class ffmpegHandeler
    {
        public string FFmpegPath { set; get; }
        public string Arguments { set; get; }
        public string InputFile { set; get; }
        public string OutputFIle { set; get; }
        private static TimeSpan duration;     
        

        public TimeSpan getduration()
        {
            return duration;
        }


        public event EventHandler PercantageChange; 


        static TimeSpan convertRegexToSpan(string input)
        {
            var splitted = input.Split(":");
            int[] ints = Array.ConvertAll(splitted, int.Parse);
            TimeSpan Duration = new TimeSpan(ints[0], ints[1], ints[2]);
            return Duration;
        }
        private  void MyProcOutputHandler(object sendingProcess,
           DataReceivedEventArgs e)
        {
            // Collect the sort command output. 
            if (!String.IsNullOrEmpty(e.Data))
            {
                Regex getDuration = new Regex(@"Duration: ((?:\d{2}(?:\:|)){3})", RegexOptions.IgnoreCase | RegexOptions.IgnoreCase);
                Regex proccedTime = new Regex(@"time=((?:\d{2}(?:\:|)){3})", RegexOptions.IgnoreCase | RegexOptions.IgnoreCase);
                Console.WriteLine(e.Data);
                MatchCollection matches = getDuration.Matches(e.Data);
                if (matches.Count != 0)
                {
                    Console.WriteLine("Duration of File is {0}", matches[0].Groups[1]);
                    duration = convertRegexToSpan(matches[0].Groups[1].ToString());

                }
                matches = proccedTime.Matches(e.Data);
                if (matches.Count != 0)
                {
                    Console.WriteLine("Procced Time {0}", matches[0].Groups[1]);
                    TimeSpan elapsed = convertRegexToSpan(matches[0].Groups[1].ToString());
                    double done = (elapsed.TotalSeconds / duration.TotalSeconds) * 100;
                    Console.WriteLine("{0:N2}% Done", done);
                    OnProgressUpdate(done);
                    



                }
            }
        }
        public ffmpegHandeler(string ffmpegpath,string inputFilePath,string outputFilePath)
        {
            this.FFmpegPath = ffmpegpath;
            this.InputFile = inputFilePath;
            this.OutputFIle = outputFilePath;

            
        }

        protected virtual void OnProgressUpdate(double percentage)
        {
            if(PercantageChange != null) {
                PercantageChange(this, new PercentageEventArgs(percentage));
            }
        }

        
        public void StartConverion(){

            string argument = "-i " + InputFile + " " + OutputFIle; 
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = FFmpegPath,
                    Arguments = argument,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardInput = true
                }
            };
            process.ErrorDataReceived += new DataReceivedEventHandler(MyProcOutputHandler);
            process.Start();
            Console.WriteLine("Started Coversion");
            process.BeginErrorReadLine();
            process.WaitForExit();
            process.Close();
        }
    }
}
