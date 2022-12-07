using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Win32;
using PythonEditor.ViewModels;

namespace PythonEditor.Commands
{
    class OpenCodeCommand : CommandBase
    {
        EditorViewModel EditorViewModel;
        public OpenCodeCommand(EditorViewModel editorViewModel)
        {
            this.EditorViewModel = editorViewModel;
        }
        public override void Execute(object? parameter)
        {
            OpenFileDialog open = new OpenFileDialog()
            {
                Title = "Open your code",
                Filter = " Scripts (*.apy) | *.apy",
                FileName = " "
            };
            if ((bool)open.ShowDialog())
            {
                EditorViewModel.ActualFilePath = open.FileName;
                using (StreamReader reader = new StreamReader(open.OpenFile()))
                {
                    this.EditorViewModel.CodeText = reader.ReadToEnd();
                }
            }
        }
    }
}
