using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonEditor.Model;
using PythonEditor.ViewModels;

namespace PythonEditor.Commands
{
    class RunProgramCommand : CommandBase
    {
        private EditorViewModel EditorViewModel;
        public RunProgramCommand(EditorViewModel editorViewModel)
        {
            this.EditorViewModel = editorViewModel;
            this.EditorViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }
        public override void Execute(object? parameter)
        {
            EditorViewModel.ConsoleText = "";
            LoxModel.Run(EditorViewModel, EditorViewModel.CodeText);
        }
        public override bool CanExecute(object? parameter)
        {
            return !string.IsNullOrEmpty(EditorViewModel.CodeText) && base.CanExecute(parameter);
        }
        private void OnViewModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName== nameof(EditorViewModel.CodeText)) {
                OnCanExecuteChanged();
            }
        }
    }
}
