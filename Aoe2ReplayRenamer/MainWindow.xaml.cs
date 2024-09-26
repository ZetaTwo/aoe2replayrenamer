using Aoe2BasicReplayParser;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static Aoe2BasicReplayParser.Aoe2ReplayHeaderBasic;

namespace Aoe2ReplayRenamer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly string DEFAULT_REPLAYS_BASE_DIRECTORY = System.IO.Path.Combine(["%USERPROFILE%", "Games", "Age of Empires 2 DE"]);
        private static readonly List<string> FILTER_DIRS = ["logs", "metadata", "preview", "testharness", "0"];


        private static string? FindAccountDirectory()
        {
            string base_path = Environment.ExpandEnvironmentVariables(DEFAULT_REPLAYS_BASE_DIRECTORY);
            foreach (string accountPath in System.IO.Directory.GetDirectories(base_path))
            {
                string accountDir = accountPath.Split(System.IO.Path.DirectorySeparatorChar).Last();
                if (FILTER_DIRS.Contains(accountDir))
                {
                    continue;
                }
                if (accountDir.All(x => x >= '0' && x <= '9'))
                {
                    return accountDir;
                }
            }
            return null;
        }

        public MainWindow()
        {
            InitializeComponent();

            string? maybeAccountDirectory = FindAccountDirectory();

            if (Properties.Settings.Default.ReplaysDirectory == "")
            {
                if (maybeAccountDirectory != null)
                {
                    Properties.Settings.Default.ReplaysDirectory = System.IO.Path.Combine([DEFAULT_REPLAYS_BASE_DIRECTORY, maybeAccountDirectory, "savegame"]);
                }

            }

            if (Properties.Settings.Default.OutputDirectory == "")
            {
                if (maybeAccountDirectory != null)
                {
                    Properties.Settings.Default.OutputDirectory = System.IO.Path.Combine([DEFAULT_REPLAYS_BASE_DIRECTORY, maybeAccountDirectory, "savegame", "renamed"]);
                }

            }
        }

        private static string ReplaceInvalidChars(string filename)
        {
            return string.Join("_", filename.Split(System.IO.Path.GetInvalidFileNameChars()));
        }

        private static void RenameReplay(string replayPath, string outputDirectory)
        {
            Aoe2ReplayHeaderBasic replay = Aoe2RecordParser.BasicParse(replayPath);
            if (replay.GameType == GameType.Campaign || replay.GameType == GameType.CoOpCampaign)
            {
                return;
            }

            string[][] teams = replay.GetTeams();
            string teamString = ReplaceInvalidChars(String.Join("_vs_", teams.Select(t => t[0])));


            DateTime replayDatetime = replay.Datetime.HasValue ? replay.Datetime.Value : System.IO.File.GetCreationTime(replayPath);
            int attempt = 0;
            while (true)
            {
                string attempt_string = attempt == 0 ? String.Empty : String.Format(" ({0})", attempt);
                string newReplayFilename = String.Format("{0:yyyyMMdd_HHmmss}-{1}{2}.aoe2record", replayDatetime, teamString, attempt_string);
                string destinationPath = System.IO.Path.Combine(outputDirectory, newReplayFilename);
                try
                {
                    System.IO.File.Copy(replayPath, destinationPath);
                }
                catch (IOException)
                {
                    if (File.Exists(destinationPath))
                    {
                        attempt++;
                        continue;
                    }

                }
                break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void Button_replays_directory_change_click(object sender, RoutedEventArgs e)
        {

        }


        private void Button_rename_once_click(object sender, RoutedEventArgs e)
        {
            string replaysDirectory = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.ReplaysDirectory);
            string outputDirectory = Environment.ExpandEnvironmentVariables(Properties.Settings.Default.OutputDirectory);
            Directory.CreateDirectory(outputDirectory);
            foreach (string replayPath in Directory.GetFiles(replaysDirectory, "*.aoe2record"))
            {
                RenameReplay(replayPath, outputDirectory);
            }
        }

        private void Button_output_directory_change_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_start_monitoring_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}