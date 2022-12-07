using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PythonEditor.Lox.Exceptions;
using PythonEditor.Lox.Interp;
using PythonEditor.Lox.Lex;
using PythonEditor.Lox.Par;
using PythonEditor.Lox.Par.Model;
using PythonEditor.ViewModels;

namespace PythonEditor.Model
{

    class LoxModel
    {
        private static Lexer Lexer;
        private static Parser Parser;
        private static Interpreter Interpreter;

        public static void Run(EditorViewModel model, string code)
        {
            Task.Run(() =>
            {
                try
                {
                    Lexer = new Lexer(code);
                    Parser = new Parser(Lexer.GetTokens());
                    Interpreter = new Interpreter(Parser.Parse(), model);
                    Interpreter.Run();
                }
                catch (ParserException ex)
                {
                    MessageBox.Show(ex.Message, "Parser Exception");
                }
                catch (InterpreterException ex)
                {
                    MessageBox.Show(ex.Message, "Interpreter Exception");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unknown Exception");
                }
            }
            );


        }
    }
}
