using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PythonEditor.ViewModels;

namespace PythonEditor.Commands
{
    public class InputCommand : CommandBase
    {
        private EditorViewModel EditorViewModel;
        public bool IsReady { get; set; } = false;
        public bool IsReaded { get; set; } = false;

        private volatile bool isEnabled = true;
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            } set
            {
                isEnabled = value;
                //OnCanExecuteChanged();
            } 
        }

        public InputCommand(EditorViewModel editorViewModel)
        {
            this.EditorViewModel = editorViewModel;
        }

        public override void Execute(object? parameter)
        {
            IsReady = true;
            while (!IsReaded)
            {
                Thread.Sleep(1);
            }
            IsReaded = false;
            //IsEnabled = false;
        }
        public override bool CanExecute(object? parameter)
        {
            return IsEnabled && base.CanExecute(parameter);
        }
    }
}
