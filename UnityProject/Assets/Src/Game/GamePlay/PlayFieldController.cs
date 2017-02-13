using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

using GhostGen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public sealed class PlayFieldController : BaseController
{
    const int kLocalPlayerIndex = 0; // TODO: Temporary until we have real multiplayer

    private PlayerHandView _playerHandView;
    private ActiveCustomersView _activeCustomersView;

    private PlayFieldView _playfieldView;
    private GameLogic _gameLogic;
    private ParticleSystem _hoverEffect;

    private IngredientCardView _draggedIngredient = null;

    public void Start(GameLogic gameLogic)
    {
         _gameLogic = gameLogic;

        _setupHoverFX();

        viewFactory.CreateAsync<PlayFieldView>("PlayFieldView", (view) =>
        {
            _playfieldView = view as PlayFieldView;
            _playfieldView.onIntroFinishedEvent += _playfieldView_OnIntroTransitionEvent;
            _playfieldView.confirmButton.onClick.AddListener(_onConfirmTurnButton);
            _playfieldView.undoButton.onClick.AddListener(_onUndoButton);

            _setupActiveCustomers(_playfieldView.staticCardLayer);
            _setupLocalPlayerHandView(kLocalPlayerIndex, _playfieldView.staticCardLayer);
        });
    }

    private void _playfieldView_OnIntroTransitionEvent(UIView p_view)
    {
        Debug.Log(string.Format("View {0} has fininished intro", p_view.name));
    }

    private void _setupHoverFX()
    {
        GameObject hoverObj = Singleton.instance.vfxBank.Create(
            VFXType.CARD_HOVER,
            Vector3.zero,
            Quaternion.identity);
        _hoverEffect = hoverObj.GetComponent<ParticleSystem>();
        _hoverEffect.gameObject.SetActive(false);
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
                    CustomerCardState state = _gameLogic.activeCustomerSet.GetCustomeByIndex(i);
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
            return;
        }
        IngredientCardView view = Singleton.instance.cardResourceBank.CreateCardView(
            cardData, 
            _playerHandView.cardSlotList[handSlot]) as IngredientCardView;

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
            kLocalPlayerIndex,
            _draggedIngredient.handIndex, 
            customerIndex);

        customerView.invalidateFlag |= UIView.InvalidationFlag.DYNAMIC_DATA;

        bool customerFinished = _gameLogic.ResolveCustomerCard(customerIndex, kLocalPlayerIndex);
        if (customerFinished)
        {
            CustomerCardState newState = _gameLogic.activeCustomerSet.GetCustomeByIndex(customerIndex);
            if (newState == null)
            {
                CustomerCardView view = _activeCustomersView.GetCardByIndex(customerIndex);
                Singleton.instance.viewFactory.RemoveView(view, true);
            }
            else
            { 
                _setupCustomerView(customerIndex, newState);
            }
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
            int handIndex = _draggedIngredient.handIndex;
            Singleton.instance.viewFactory.RemoveView(_draggedIngredient, true);
            _setupIngredientView(handIndex, localPlayer.hand.GetCard(handIndex));
        }

        _draggedIngredient = null;
    }

    private PlayerState localPlayer
    {
        get { return _gameLogic.playerGroup.GetPlayer(kLocalPlayerIndex); }
    }

    private void _activeHoverFX(Vector3 position)
    {
        _hoverEffect.gameObject.SetActive(true);
        _hoverEffect.transform.position = position;
    }
    private void _deactiveHoverFX()
    {
        _hoverEffect.gameObject.SetActive(false);
    }

    private void _setupLocalPlayerHandView(
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
            PlayerState player = _gameLogic.playerGroup.GetPlayer(localPlayerIndex);

            for (int i = 0; i < PlayerState.kHandSize; ++i)
            {
                IngredientCardData ingredientCard = player.hand.GetCard(i);
                _setupIngredientView(i, ingredientCard);
            }
        }, handParent);
    }

    private void _onConfirmTurnButton()
    {
        _gameLogic.EndPlayerTurn();
    }

    private void _onUndoButton()
    {
        bool didUndo = _gameLogic.UndoLastAction();
        if (!didUndo) { return; }

        _playerHandView.invalidateFlag = UIView.InvalidationFlag.ALL;
        _activeCustomersView.invalidateFlag = UIView.InvalidationFlag.ALL;

        for(int i = 0; i < PlayerState.kHandSize; ++i)
        {
            IngredientCardView view = _playerHandView.GetCardAtIndex(i);
            if(view != null)
            {
                Singleton.instance.viewFactory.RemoveView(view, true);
            }
            _setupIngredientView(i, localPlayer.hand.GetCard(i));
        }

        for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
        {
            CustomerCardView view = _activeCustomersView.GetCardByIndex(i);
            //if (view != null)
            //{
            //    Singleton.instance.viewFactory.RemoveView(view, true);
            //}
            _setupCustomerView(i, _gameLogic.activeCustomerSet.GetCustomeByIndex(i));
        }
    }
}
