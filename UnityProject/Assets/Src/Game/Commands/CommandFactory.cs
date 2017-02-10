using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandFactory
{
    private Stack<ICommand> _undoStack = new Stack<ICommand>();

    public void Execute(ICommand command)
    {
        command.Execute();
        _undoStack.Push(command);
    }
   
    public bool Undo()
    {
        if(_undoStack.Count == 0)
        {
            Debug.Log("Command Stack is empty!");
            return false;
        }

        ICommand command;
        do
        {
            command = _undoStack.Pop();
            command.Undo();
        }
        while (command.isLinked);
        //Maybe save this command into another stack for re-do's
        return true;
    }
}
