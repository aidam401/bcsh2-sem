using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using PythonEditor.Commands;

namespace PythonEditor.ViewModels
{
    public class EditorViewModel : ViewModelBase
    {
        public ICommand RunCommand { get; }
        public ICommand SaveAsCommand { get; }
        public ICommand OpenCommand { get; }
        public InputCommand InputCommand { get; }

        public string ActualFilePath { get; set; }

        private string inputText;
        public string InputText
        {
            get
            {
                return inputText;
            }
            set
            {
                inputText = value;
                OnPropertyChanged(nameof(InputText));
            }
        }

        private string codeText;
        public string CodeText
        {
            get
            {
                return codeText;
            }
            set
            {
                codeText = value;
                OnPropertyChanged(nameof(CodeText));
            }
        }

        private string consoleText;
        public string ConsoleText
        {
            get
            {
                return consoleText;
            }
            set
            {
                consoleText = value;
                OnPropertyChanged(nameof(ConsoleText));
            }
        }
        private string lineText;
        public string LineText
        {
            get
            {
                return lineText;
            }
            set
            {
                lineText = value;
                OnPropertyChanged(nameof(LineText));
            }
        }

        public EditorViewModel()
        {
            PropertyChanged += OnPropertyChanged;

            RunCommand = new RunProgramCommand(this);
            SaveAsCommand = new SaveAsCodeCommand(this);
            OpenCommand = new OpenCodeCommand(this);
            InputCommand = new InputCommand(this);

            LineText = "1";
        }


        protected void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CodeText))
            {
                UpdateLines();
                AutoSave();
            }

        }

        private void AutoSave()
        {
            if(!string.IsNullOrEmpty(ActualFilePath))
                try {
                    File.WriteAllText(ActualFilePath, CodeText);
                }catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
        }

        private void UpdateLines()
        {
            string[] strTemp = codeText.Split(new string[] { "\n" }, StringSplitOptions.None);
            if (strTemp.Length > 0)
            {
                lineText = string.Empty;
                for (int i = 1; i <= strTemp.Length; i++)
                {
                    LineText += i + "\n";
                }
            }
        }





    }
}
