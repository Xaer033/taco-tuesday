using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    bool isLinked { get; }

    void Execute();
    void Undo();
}
