using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GhostGen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class PlayFieldController : BaseController
{
    private CardDeck _customerDeck;
    private CardDeck _ingredientDeck;

    private List<PlayerState> _playerList;
    private PlayerHandView _playerHandView;
    
    private PlayFieldView _playfieldView;
    
    public void Start(List<PlayerState> playerList)
    {
        _customerDeck = CardDeck.FromFile("Decks/CustomerDeck");
        _customerDeck.Shuffle();

        _ingredientDeck = CardDeck.FromFile("Decks/IngredientDeck");
        _ingredientDeck.Shuffle();

        _setupPlayers(playerList);

        viewFactory.CreateAsync<PlayFieldView>("PlayFieldView", (view) =>
        {
            _playfieldView = view as PlayFieldView;
            _playfieldView.onIntroFinishedEvent += _playfieldView_OnIntroTransitionEvent;

            _setupCustomers(_playfieldView.staticCardLayer);
            _setupLocalPlayerHandView(0, _playfieldView.staticCardLayer);
        });
    }

    private void _playfieldView_OnIntroTransitionEvent(UIView p_view)
    {
        Debug.Log(string.Format("View {0} has fininished intro", p_view.name));
    }

    private void _setupCustomers(Transform parent)
    {
        viewFactory.CreateAsync<ActiveCustomersView>(
            "ActiveCustomersView",
            (view) =>
            {
                ActiveCustomersView customersView = (ActiveCustomersView)view;
                for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
                {
                    CustomerCardData card = _customerDeck.Pop() as CustomerCardData;
                    CustomerCardState state = CustomerCardState.Create(card);

                    customersView.SetCardByIndex(i, state);
                }
            }, parent
        );
        
    }

    private void _setupPlayers(List<PlayerState> playerList)
    {
        _playerList = playerList;
        foreach (PlayerState player in _playerList)
        {
            _fillPlayerHand(player);
        }
    }

    private void _setupLocalPlayerHandView(int localPlayerIndex, Transform handParent)
    {
        if (_playerHandView)
        {
            viewFactory.RemoveView(_playerHandView, true);
        }

        viewFactory.CreateAsync<PlayerHandView>("PlayerHandView", (view)=>
        {
            _playerHandView = view as PlayerHandView;
            for (int i = 0; i < PlayerState.kHandSize; ++i)
            {
                _playerHandView.SetCardAtIndex(i, _playerList[localPlayerIndex].hand.GetCard(i));
            }
        }, handParent);
    }

    private void _fillPlayerHand(PlayerState player)
    {
        Debug.Assert(player != null);
        Debug.Assert(_ingredientDeck != null);

        while(player.hand.emptyCards > 0)
        {
            IngredientCardData card = (IngredientCardData)_ingredientDeck.Pop();
            if(card == null)
            {
                Debug.Log("Ingredient Deck empty");
                break;
            }
            player.hand.ReplaceEmptyCard(card);
        }
    }
}
