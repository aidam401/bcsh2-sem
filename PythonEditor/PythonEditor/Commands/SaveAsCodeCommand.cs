using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using PythonEditor.ViewModels;

namespace PythonEditor.Commands
{
    public class SaveAsCodeCommand : CommandBase
    {
        EditorViewModel EditorViewModel;
        public SaveAsCodeCommand(EditorViewModel editorViewModel)
        {
            EditorViewModel= editorViewModel;
            this.EditorViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        public override void Execute(object? parameter)
        {

            SaveFileDialog save = new SaveFileDialog()
            {
                Title = "Save your code",
                Filter = " Scripts (*.apy) | *.apy",
                FileName = " "
            };
            if ((bool)save.ShowDialog())
            {
                EditorViewModel.ActualFilePath = save.FileName;
                File.WriteAllText(save.FileName, EditorViewModel.CodeText);
            }
        }
        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(EditorViewModel.CodeText) && base.CanExecute(parameter);
        }
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditorViewModel.CodeText))
            {
                OnCanExecuteChanged();
            }
        }
    }
}
