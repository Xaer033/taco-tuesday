﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

using GhostGen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using DG.Tweening;

public sealed class PlayFieldController : BaseController
{
    const int kLocalPlayerIndex = 0; // TODO: Temporary until we have real multiplayer

    private PlayerHandView _playerHandView;
    private ActiveCustomersView _activeCustomersView;

    private PlayFieldView _playfieldView;
    private GameLogic _gameLogic;

    private ParticleSystem _hoverFX;
    private ParticleSystem _slamFX;

    private IngredientCardView _draggedIngredient = null;
    private CustomerCardView _droppedCustomer = null;
    private PassInterludeController _passController;

    private Action _onGameOver;

    public void Start(GameLogic gameLogic, Action onGameOver)
    {
         _gameLogic = gameLogic;
        _onGameOver = onGameOver;

        _setupFX();

        _passController = new PassInterludeController();

        viewFactory.CreateAsync<PlayFieldView>("PlayFieldView", (_view) =>
        {
            _playfieldView = _view as PlayFieldView;
            view = _view;

            _playfieldView.onIntroFinishedEvent += _playfieldView_OnIntroTransitionEvent;
            _playfieldView.confirmButton.onClick.AddListener(_onConfirmTurnButton);
            _playfieldView.undoButton.onClick.AddListener(_onUndoButton);

            _playfieldView.SetActivePlayer(activePlayer.index);

            for(int i = 0; i < _gameLogic.playerGroup.playerCount; ++i)
            {
                PlayerState player = _gameLogic.playerGroup.GetPlayer(i);
                _playfieldView.SetPlayerName(i, player.name);
                _playfieldView.SetPlayerScore(i, player.score);
            }

            _setupActiveCustomers(_playfieldView.staticCardLayer);
            _createPlayerHandView(kLocalPlayerIndex, _playfieldView.staticCardLayer);
        });
    }

    private void _playfieldView_OnIntroTransitionEvent(UIView p_view)
    {
        Debug.Log(string.Format("View {0} has fininished intro", p_view.name));
    }

    private void _setupFX()
    {
        Transform canvasTransform = Singleton.instance.gui.viewFactory.canvas.transform;
        GameObject hoverObj = Singleton.instance.vfxBank.Create(VFXType.CARD_HOVER, canvasTransform);
        _hoverFX = hoverObj.GetComponent<ParticleSystem>();
        _hoverFX.gameObject.SetActive(false);


        GameObject slamObj = Singleton.instance.vfxBank.Create(VFXType.CARD_SLAM, canvasTransform);
        _slamFX = slamObj.GetComponent<ParticleSystem>();
    }

    private void _setupActiveCustomers(Transform parent)
    {
        viewFactory.CreateAsync<ActiveCustomersView>(
            "ActiveCustomersView",
            (view) =>
            {
                _activeCustomersView = (ActiveCustomersView)view;
                for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
                {
                    CustomerCardState state = _gameLogic.activeCustomerSet.GetCustomerByIndex(i);
                    _setupCustomerView(i, state);
                }
            }, parent
        );
        
    }

    private void _setupCustomerView(int customerSlot, CustomerCardState cardState)
    {
        if (cardState == null)
        {
            Debug.LogWarning("Card State is null!");
            _activeCustomersView.RemoveCardByIndex(customerSlot); // TODO: Do this On card slam instead of after the fact
            return;
        }

        CustomerCardView view = _activeCustomersView.GetCardByIndex(customerSlot);
        if (view == null)
        {
            view = (CustomerCardView)Singleton.instance.cardResourceBank.CreateCardView(
             cardState.cardData,
             _activeCustomersView._activeSlotList[customerSlot]);
        }

        view.cardState = cardState;

        view.eventTrigger.triggers.Clear();
        EventTrigger.Entry OnDrop = new EventTrigger.Entry();
        OnDrop.eventID = EventTriggerType.Drop;
        OnDrop.callback.AddListener((e)=>_handleIngredientCardDrop((PointerEventData)e, view));
        view.eventTrigger.triggers.Add(OnDrop);

        EventTrigger.Entry OnHoverBegin = new EventTrigger.Entry();
        OnHoverBegin.eventID = EventTriggerType.PointerEnter;
        OnHoverBegin.callback.AddListener((e) => _handleIngredientCardHover(view));
        view.eventTrigger.triggers.Add(OnHoverBegin);

        EventTrigger.Entry OnHoverEnd = new EventTrigger.Entry();
        OnHoverEnd.eventID = EventTriggerType.PointerExit;
        OnHoverEnd.callback.AddListener((e) => _deactiveHoverFX());
        view.eventTrigger.triggers.Add(OnHoverEnd);

        _activeCustomersView.SetCardByIndex(customerSlot, view);
    }

    private void _setupIngredientView(int handSlot, IngredientCardData cardData)
    {
        if(cardData == null)
        {
            Debug.LogWarning("Card Data is null!");
            _playerHandView.RemoveCardByIndex(handSlot); // TODO: Do this On card slam instead of after the fact    
            return;
        }

        IngredientCardView view = _playerHandView.GetCardAtIndex(handSlot);
        if (view == null)
        {
            view = Singleton.instance.cardResourceBank.CreateCardView(
            cardData,
            _playerHandView.cardSlotList[handSlot]) as IngredientCardView;
        }

        view.cardData = cardData;

        view.eventTrigger.triggers.Clear();
        EventTrigger.Entry OnBeginDrag = new EventTrigger.Entry();
        OnBeginDrag.eventID = EventTriggerType.BeginDrag;
        OnBeginDrag.callback.AddListener((e) => _handleIngredientCardBeginDrag((PointerEventData)e, view));
        view.eventTrigger.triggers.Add(OnBeginDrag);

        EventTrigger.Entry OnEndDrag = new EventTrigger.Entry();
        OnEndDrag.eventID = EventTriggerType.EndDrag;
        OnEndDrag.callback.AddListener((e) => _handleEndDrag());
        view.eventTrigger.triggers.Add(OnEndDrag);

        _playerHandView.SetCardAtIndex(handSlot, view);
    }

    private void _handleIngredientCardDrop(PointerEventData e, CustomerCardView customerView)
    {
        Debug.Log("PlayField Received drop of: " + customerView.cardData.titleKey);
        if (_draggedIngredient == null) { return; }

        CustomerCardState customerState = customerView.cardState;
        IngredientCardData ingredientData = _draggedIngredient.cardData as IngredientCardData;

        if (!customerState.CanAcceptCard(ingredientData)) { return; }
        
        int customerIndex = customerState.slotIndex;

        _draggedIngredient.isDropSuccessfull = _gameLogic.PlayCardOnCustomer(
            _gameLogic.playerGroup.activePlayer.index,
            _draggedIngredient.handIndex, 
            customerIndex);

        if(_draggedIngredient.isDropSuccessfull)
        {       
            _droppedCustomer = customerView; 
        }
    }

    private void _handleIngredientCardHover(CustomerCardView customerView)
    {
        if(_draggedIngredient == null) { return; }
        if (!_draggedIngredient.isDragging) { return; }

        CustomerCardState customerState = customerView.cardState;
        IngredientCardData ingredientData = _draggedIngredient.cardData as IngredientCardData;

        if (customerState.CanAcceptCard(ingredientData))
        {
            _activeHoverFX(customerView.transform.position);
        }
    }

    private void _handleIngredientCardHoverEnd(CustomerCardView customerView)
    {
        if (_draggedIngredient == null) { return; }
        if (!_draggedIngredient.isDragging) { return; }

        _activeHoverFX(customerView.transform.position);
    }

    private void _handleIngredientCardBeginDrag(PointerEventData e, IngredientCardView view)
    {
        _draggedIngredient = view;
    }

    private void _handleEndDrag()
    {
        _deactiveHoverFX();

        if(_draggedIngredient.isDropSuccessfull)
        {
            _playerHandView.blockCardDrag = true;
            _zoomSlamTween(_draggedIngredient, _droppedCustomer, () =>
            {
                int handIndex = _draggedIngredient.handIndex;
                //Singleton.instance.viewFactory.RemoveView(_draggedIngredient, true);
                _draggedIngredient = null;

                //_playerHandView.SetCardAtIndex(handIndex, null);
                _playerHandView.RemoveCardByIndex(handIndex);
                IngredientCardData newIngredientCard = activePlayer.hand.GetCard(handIndex);
                _setupIngredientView(handIndex, newIngredientCard);

                //_droppedCustomer.invalidateFlag |= UIView.InvalidationFlag.DYNAMIC_DATA;
                int customerIndex = _droppedCustomer.cardState.slotIndex;
                bool customerFinished = _gameLogic.ResolveCustomerCard(customerIndex, activePlayer.index);
                if (customerFinished)
                {
                    _playfieldView.SetPlayerScore(activePlayer.index, activePlayer.score);
                    CustomerCardState newState = _gameLogic.activeCustomerSet.GetCustomerByIndex(customerIndex);
                    _setupCustomerView(customerIndex, newState);                    
                }
           
                _playerHandView.blockCardDrag = false;

                if(_gameLogic.isGameOver)
                {
                    if(_onGameOver != null)
                    {
                        _onGameOver();
                    }  
                }
            });
        }
    }

    private PlayerState localPlayer
    {
        get { return _gameLogic.playerGroup.GetPlayer(kLocalPlayerIndex); }
    }

    private PlayerState activePlayer
    {
        get { return _gameLogic.playerGroup.activePlayer; }
    }

    private void _activeHoverFX(Vector3 position)
    {
        _hoverFX.gameObject.SetActive(true);
        _hoverFX.transform.position = position;
    }
    private void _deactiveHoverFX()
    {
        _hoverFX.gameObject.SetActive(false);
    }

    private void _createPlayerHandView(
        int localPlayerIndex, 
        Transform handParent)
    {
        if (_playerHandView)
        {
            viewFactory.RemoveView(_playerHandView, true);
        }

        viewFactory.CreateAsync<PlayerHandView>("PlayerHandView", (view)=>
        {
            _playerHandView = view as PlayerHandView;
            _setupHandViewFromPlayer(localPlayerIndex);
        }, handParent);
    }

    private void _setupHandViewFromPlayer(int playerIndex)
    {
        PlayerState player = _gameLogic.playerGroup.GetPlayer(playerIndex);
        for (int i = 0; i < PlayerState.kHandSize; ++i)
        {
            IngredientCardData ingredientCard = player.hand.GetCard(i);
            _setupIngredientView(i, ingredientCard);
        }
    }

    private void _onConfirmTurnButton()
    {
        string instructionTextKey = "Pass device to " + _gameLogic.playerGroup.GetNextPlayer().name;
        _passController.Start(instructionTextKey, () =>
        {
            _gameLogic.EndPlayerTurn();
            _refreshHandView(activePlayer);
            _playfieldView.SetActivePlayer(activePlayer.index);
        });
    }

    private void _onUndoButton()
    {
        bool didUndo = _gameLogic.UndoLastAction();
        if (!didUndo) { return; }
        
        _refreshHandView(activePlayer);

        _activeCustomersView.invalidateFlag = UIView.InvalidationFlag.ALL;
        for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
        {
            _setupCustomerView(i, _gameLogic.activeCustomerSet.GetCustomerByIndex(i));
        }

        _playfieldView.SetActivePlayer(activePlayer.index);
    }

    private void _refreshHandView(PlayerState player)
    {
        _playfieldView.SetPlayerScore(player.index, player.score);

        _playerHandView.invalidateFlag = UIView.InvalidationFlag.ALL;
        for (int i = 0; i < PlayerState.kHandSize; ++i)
        {
            _setupIngredientView(i, player.hand.GetCard(i));
        }
    }

    private void _zoomSlamTween(
        IngredientCardView ingredient, 
        CustomerCardView customer, 
        TweenCallback callback)
    {
        Vector3 originalScale = ingredient.transform.localScale;

        Vector3 cardEyeVec = (Camera.main.transform.position - customer.transform.position).normalized;
        Sequence sequence = DOTween.Sequence();
        Tween moveToTween = ingredient.transform.DOMove(customer.transform.position + cardEyeVec, 0.27f);
        moveToTween.SetEase(Ease.OutCubic);

        Tween growTween = ingredient.transform.DOScale(originalScale * 1.3f, 0.31f);
        moveToTween.SetEase(Ease.OutCubic);

        Tween slamTween = ingredient.transform.DOScale(originalScale * 0.1f, 0.2f);
        slamTween.SetEase(Ease.InCubic);

        Sequence shakeSeq = DOTween.Sequence();
        Tween shakePosTween = customer.transform.DOShakePosition(0.4f, 10.0f, 22);
        shakePosTween.SetEase(Ease.OutCubic);
        Tween shakeRotTween = customer.transform.DOShakeRotation(0.4f, 6.0f, 16);
        shakeRotTween.SetEase(Ease.OutCubic);
        shakeSeq.Insert(0.0f, shakePosTween);
        shakeSeq.Insert(0.0f, shakeRotTween);

        sequence.Insert(0.0f, moveToTween);
        sequence.Insert(0.0f, growTween);
        sequence.Append(slamTween);
        sequence.AppendCallback(() =>
        {
            ingredient.gameObject.SetActive(false);
            customer.invalidateFlag = UIView.InvalidationFlag.DYNAMIC_DATA;
            _slamFX.transform.position = ingredient.transform.position;
            _slamFX.Play();
        });
        sequence.Append(shakeSeq);
        sequence.OnComplete(callback);
    }
}
