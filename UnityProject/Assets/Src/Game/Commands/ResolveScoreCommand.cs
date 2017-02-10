using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResolveScoreCommand : ICommand
{
    private int _savedScore = -1;
    private PlayerState _player;
    private CustomerCardState _customer;

    public static ResolveScoreCommand Create(
        PlayerState player,
        CustomerCardState customer)
    {
        ResolveScoreCommand command = new ResolveScoreCommand();
        command._player = player;
        command._customer = customer;
        return command;
    }

    private ResolveScoreCommand()
    {
    }

    public bool isLinked
    {
        get { return true; }
    }

    public void Execute()
    {
        _savedScore = _player.points;
        _player.points += _customer.totalScore;
    }

    public void Undo()
    {
        _player.points = _savedScore;
    }
}
