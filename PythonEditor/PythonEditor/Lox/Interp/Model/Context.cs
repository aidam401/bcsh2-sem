using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonEditor.Lox.Exceptions;
using PythonEditor.Lox.Par.Model;
using PythonEditor.Lox.Par.Model.Interfaces;

namespace PythonEditor.Lox.Interp.Model
{
    public class Context
    {
        static readonly List<Function> Functions = new List<Function>();
        static readonly Stack<List<IdentExpression>> ContextIdents = new Stack<List<IdentExpression>>();
        private readonly Visitors Visit = Visitors.GetInstance();
        public Context()
        {
            CreateNewContext();
            AddNativeFunctions();
        }
        public void ReturnToLastContext()
        {
            try { ContextIdents.Pop(); }
            catch (InvalidOperationException) { throw new InterpreterException("No upper context."); };
        }
        public void CreateNewContext() { ContextIdents.Push(new List<IdentExpression>()); }
        public void AddIdent(IdentExpression newIdent)
        {
            foreach (var identList in ContextIdents)
                for (int i = 0; i < identList.Count(); i++)
                    if (identList[i].Name == newIdent.Name)
                    {
                        identList[i] = newIdent;
                        return;
                    }
            try { ContextIdents.Peek().Add(newIdent); }
            catch (InvalidOperationException) { throw new InterpreterException("No context"); }
        }

        public IdentExpression? GetIdent(IdentExpression searchIdent)
        {
            foreach (var identList in ContextIdents)
                foreach (var ident in identList)
                    if (ident.Name == searchIdent.Name)
                        return ident;
            return null;
        }
        public void AddFunction(Function newFunction)
        {

            foreach (var func in Functions)
                if (func.Name == newFunction.Name && func.Args == newFunction.Args)
                    throw new InterpreterException("Function already exist.");
            Functions.Add(newFunction);
        }
        public PossibleReturn ExecuteFunction(FunctionCall functionCall)
        {

            foreach (var func in Functions)
            {

                if (func.Name != functionCall.Name || func.Args.Count() != functionCall.ArgEvals.Count())
                    continue;
                var evaluedArgs = new object[func.Args.Count()];
                for (int i = 0; i < func.Args.Count(); i++)
                {
                    evaluedArgs[i] = Visit.Eval(functionCall.ArgEvals[i]);
                    if (func.Args[i].Type != evaluedArgs[i].GetType())
                        goto OuterLoop;
                }

                CreateNewContext();
                for (int i = 0; i < func.Args.Count(); i++)
                {
                    var identExp = new IdentExpression();
                    identExp.Name = func.Args[i].Name;
                    identExp.Value = new LiteralExpression() { Value = Visit.Eval(functionCall.ArgEvals[i]) };

                    AddIdent(identExp);
                }
                foreach (var stat in func.Statements)
                {
                    var ret = Visit.Exec(stat);
                    if (!ret.IsVoid && ret.Value == null)
                        continue;
                    if (func.ReturnType != ret.Value.GetType())
                        throw new InterpreterException("Function has unexpected return type.");
                    ReturnToLastContext();
                    return ret;
                }
                if (func.ReturnType != null)
                    throw new InterpreterException("Return type expected.");
                ReturnToLastContext();
                return new PossibleReturn();

            OuterLoop:
                continue;
            }
            throw new InterpreterException("Function not in context.");
        }
        private void AddNativeFunctions()
        {
            ///PRINT
            //print(toPrint:str) -> None
            Functions.Add(
                new Function()
                {
                    Name = "print",
                    Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(int) } },
                    Statements = new List<IExecutable>() {
                        new PrintStatement() {
                            Evaluable = new IdentExpression(){Name="else"}
                        },
                    }
                }
                );
            //print(toPrint:str) -> None
            Functions.Add(
                new Function()
                {
                    Name = "print",
                    Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(double) } },
                    Statements = new List<IExecutable>() {
                        new PrintStatement() {
                            Evaluable = new IdentExpression(){Name="else"}
                        },
                    }
                }
                );
            //print(toPrint:str) -> None
            Functions.Add(
                new Function()
                {
                    Name = "print",
                    Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(string) } },
                    Statements = new List<IExecutable>() {
                        new PrintStatement() {
                            Evaluable = new IdentExpression(){Name="else"}
                        },
                    }
                }
                );

            //CONVERT
            //int(toConvert:double) -> int
            Functions.Add(
               new Function()
               {
                   Name = "int",
                   Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(double) } },
                   Statements = new List<IExecutable>()
                   {
                       new ReturnStatement()
                       {
                           Expr=new ConvertExpression(){
                               Expr=new IdentExpression{Name="else"},
                               NewType=typeof(int)
                           }

                       }
               },
                   ReturnType = typeof(int)
               }
               );
            //int(toConvert:string) -> int
            Functions.Add(
                new Function()
                {
                    Name = "int",
                    Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(string) } },
                    Statements = new List<IExecutable>()
                    {
                       new ReturnStatement()
                       {
                           Expr=new ConvertExpression(){
                               Expr=new IdentExpression{Name="else"},
                               NewType=typeof(int)
                           }

                       }
                },
                    ReturnType = typeof(int)
                }
                );
            //double(toConvert:int) -> double
            Functions.Add(
                new Function()
                {
                    Name = "double",
                    Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(int) } },
                    Statements = new List<IExecutable>()
                    {
                       new ReturnStatement()
                       {
                           Expr=new ConvertExpression(){
                               Expr=new IdentExpression{Name="else"},
                               NewType=typeof(double)
                           }

                       }
                },
                    ReturnType = typeof(double)
                }
                );
            //double(toConvert:str) -> double
            Functions.Add(
                 new Function()
                 {
                     Name = "double",
                     Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(string) } },
                     Statements = new List<IExecutable>()
                     {
                       new ReturnStatement()
                       {
                           Expr=new ConvertExpression(){
                               Expr=new IdentExpression{Name="else"},
                               NewType=typeof(double)
                           }

                       }
                 },
                     ReturnType = typeof(double)
                 }
                 );
            //str(toConvert:int) -> str
            Functions.Add(
                  new Function()
                  {
                      Name = "str",
                      Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(int) } },
                      Statements = new List<IExecutable>()
                      {
                       new ReturnStatement()
                       {
                           Expr=new ConvertExpression(){
                               Expr=new IdentExpression{Name="else"},
                               NewType=typeof(string)
                           }

                       }
                  },
                      ReturnType = typeof(string)
                  }
                  );
            //str(toConvert:double) -> str
            Functions.Add(
                  new Function()
                  {
                      Name = "str",
                      Args = new List<Arg>() { new Arg() { Name = "else", Type = typeof(double) } },
                      Statements = new List<IExecutable>()
                      {
                       new ReturnStatement()
                       {
                           Expr=new ConvertExpression(){
                               Expr=new IdentExpression{Name="else"},
                               NewType=typeof(string)
                           }

                       }
                  },
                      ReturnType = typeof(string)
                  }
                  );

            ///INPUT
            //input() -> str
            Functions.Add(
                new Function()
                {
                    Name = "input",
                    Statements = new List<IExecutable>
                    {
                        new ReturnStatement()
                        {
                            Expr = new InputExpression()
                        }
                    },
                    ReturnType = typeof(string)
                }) ;


        }
    }
}
