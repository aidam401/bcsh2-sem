using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PythonEditor.Lox.Par.Model;

namespace PythonEditor.Lox.Interp.Model.Interfaces
{
    public interface IExecutableVisitor
    {
        PossibleReturn VisitProgram(Program program);
        PossibleReturn VisitBlock(Block block);
        PossibleReturn VisitExecFunctionCall(FunctionCall functionCall);
        PossibleReturn VisitSetStatement(SetStatement setStatement);
        PossibleReturn VisitIfStatement(IfStatement ifStatement);
        PossibleReturn VisitWhileStatement(WhileStatement whileStatement);
        PossibleReturn VisitForStatement(ForStatement forStatement);
        PossibleReturn VisitReturnStatement(ReturnStatement returnStatement);
        PossibleReturn VisitPrintStatement(PrintStatement printStatement);
        
    }

    public interface IEvaluableVisitor
    {
        void VisitFunction(Function function);
        object VisitUnaryExpression(UnaryExpression unaryExpression);
        object VisitBinaryExpression(BinaryExpression binaryExpression);
        object VisitIdentExpression(IdentExpression identExpression);
        object VisitLiteralExpression(LiteralExpression literalExpression);
        bool VisitBinaryCondition(BinaryCondition binaryCondition);
        bool VisitNotCondition(NotCondition oddCondition);
        object VisitAcceptFunctionCall(FunctionCall functionCall);
        object VisitConvertExpression(ConvertExpression convertStatement);
        object VisitInputExpression(InputExpression inputExpression);
    }
}
