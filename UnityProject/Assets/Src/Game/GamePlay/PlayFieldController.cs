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
    private PlayerHandView _playerHandView;
    private ActiveCustomersView _activeCustomersView;

    private PlayFieldView _playfieldView;
    private GameLogic _gameLogic;
    private ParticleSystem _hoverEffect;

    private IngredientCardView _draggedIngredient = null;

    public void Start(GameLogic gameLogic)
    {
        const int kLocalPlayerIndex = 0; // TODO: Temporary until we have real multiplayer
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
                    CustomerCardState state = _gameLogic.activeCustomerSet.GetCustomerAtSlot(i);
                    _setupCustomerView(i, state);
                }
            }, parent
        );
        
    }

    private void _setupCustomerView(int customerSlot, CustomerCardState cardState)
    {
        _activeCustomersView.SetCardByIndex(customerSlot, cardState);
        CustomerCardView view = _activeCustomersView.GetCardByIndex(customerSlot);

        view.eventTrigger.triggers.Clear();
        EventTrigger.Entry OnDop = new EventTrigger.Entry();
        OnDop.eventID = EventTriggerType.Drop;
        OnDop.callback.AddListener((e)=>_handleIngredientCardDrop((PointerEventData)e, view));
        view.eventTrigger.triggers.Add(OnDop);

        EventTrigger.Entry OnHover = new EventTrigger.Entry();
        OnHover.eventID = EventTriggerType.PointerEnter;
        OnHover.callback.AddListener((e) => _handleIngredientCardHover(view));
        view.eventTrigger.triggers.Add(OnHover);

        EventTrigger.Entry OnHoverEnd = new EventTrigger.Entry();
        OnHoverEnd.eventID = EventTriggerType.PointerExit;
        OnHoverEnd.callback.AddListener((e) => _deactiveHoverFX());
        view.eventTrigger.triggers.Add(OnHoverEnd);

    }

    private void _setupIngredientView(int handSlot, IngredientCardData cardData)
    {
        _playerHandView.SetCardAtIndex(handSlot, cardData);
        _playerHandView.Validate();

        IngredientCardView view = _playerHandView.GetCardAtIndex(handSlot);
        EventTrigger.Entry OnBeginDrag = new EventTrigger.Entry();
        OnBeginDrag.eventID = EventTriggerType.BeginDrag;
        OnBeginDrag.callback.AddListener((e) => _handleIngredientCardBeginDrag((PointerEventData)e, view));
        view.eventTrigger.triggers.Add(OnBeginDrag);

        EventTrigger.Entry OnEndDrag = new EventTrigger.Entry();
        OnEndDrag.eventID = EventTriggerType.EndDrag;
        OnEndDrag.callback.AddListener((e) => _handleEndDrag());
        view.eventTrigger.triggers.Add(OnEndDrag);
    }

    private void _handleIngredientCardDrop(PointerEventData e, CustomerCardView customerView)
    {
        Debug.Log("PlayField Received drop of: " + customerView.cardData.titleKey);
        Assert.IsNotNull(_draggedIngredient);
        CustomerCardState customerState = customerView.cardState;
        IngredientCardData ingredientData = _draggedIngredient.cardData as IngredientCardData;

        if (customerState.CanAcceptCard(ingredientData))
        {
            int currentPlayer = _gameLogic.playerGroup.activePlayer.index; // TODO: this line feels unnessisary...
            _draggedIngredient.isDropSuccessfull = _gameLogic.PlayCardOnCustomer(
                ingredientData, 
                currentPlayer, 
                customerState.slotIndex);           
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
            Singleton.instance.viewFactory.RemoveView(_draggedIngredient, true);
        }
        _draggedIngredient = null;
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
        _gameLogic.UndoLastAction();
    }
}
