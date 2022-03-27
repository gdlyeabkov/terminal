using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

using System.Speech.Synthesis;

namespace Terminal
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public SpeechSynthesizer debugger;
        public int commandCursor = 0;
        List<string> executedCommands;

        public MainWindow()
        {
            InitializeComponent();

            Initialize();
        
        }

        public void Initialize()
        {
            debugger = new SpeechSynthesizer();
            executedCommands = new List<string>();
            this.KeyUp += GlobalKeyUpHandler;
        }

        public void GlobalKeyUpHandler(object sender, KeyEventArgs e)
        {
            HandleInput(e);
        }

        public void HandleInput(KeyEventArgs e)
        {
            Key key = e.Key;
            bool isEnter = key == Key.Enter;
            bool isUpArrow = key == Key.Up;
            bool isDownArrow = key == Key.Down;
            if (isEnter)
            {
                string inputString = input.Text;
                try
                {
                    Process process = new Process();
                    process.StartInfo.RedirectStandardInput = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.FileName = "cmd.exe";
                    string executedCommand = "/C " + inputString + "& exit";
                    process.StartInfo.Arguments = executedCommand;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    StreamReader reader = process.StandardOutput;
                    string output = reader.ReadToEnd();
                    process.Close();
                    foreach (string outputLine in output.Split(new Char[] { '\n' }))
                    {
                        TextBlock outputString = new TextBlock();
                        outputString.Height = 25;
                        outputString.Foreground = System.Windows.Media.Brushes.White;
                        outputString.Margin = new Thickness(15, 0, 15, 0);
                        outputString.Text = outputLine;
                        int countExecutedCommands = terminal.Children.Count;
                        int insertedCommandIndex = countExecutedCommands - 2;
                        terminal.Children.Insert(insertedCommandIndex, outputString);
                        terminalWrap.ScrollToBottom();
                    }
                    executedCommands.Add(inputString);
                    commandCursor++;
                    input.Text = "";
                }
                catch (System.ComponentModel.Win32Exception)
                {
                    debugger.Speak("ошибка ввода");
                }
            }
            else if (isDownArrow)
            {
                if (commandCursor < executedCommands.Count - 1)
                {
                    commandCursor++;
                    input.Text = executedCommands[commandCursor];
                }
            } else if (isUpArrow)
            {
                if (commandCursor > 0)
                {
                    commandCursor--;
                    input.Text = executedCommands[commandCursor];
                }
            }
        }

    }
}
