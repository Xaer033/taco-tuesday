using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

using GhostGen;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using DG.Tweening;

public sealed class NormalPlayFieldController : BaseController
{
    private GameMatchState _matchState;

    private PlayerHandView      _playerHandView;
    private ActiveCustomersView _activeCustomersView;

    private PlayFieldView   _playfieldView;

    private ParticleSystem  _hoverFX;
    private ParticleSystem  _slamFX;

    private IngredientCardView      _draggedIngredient  = null;
    private CustomerCardView        _droppedCustomer    = null;

    private NetworkManager  _networkManager = Singleton.instance.networkManager;
    private MoveRequest[]    _moveRequests = new MoveRequest[PlayerState.kMaxCardsPerTurn];
    
// Broadcast events
    public Func<MoveRequest, bool>          onPlayOnCustomer    { set; get; }
    public Func<int, bool>                  onResolveScore      { set; get; }
    public Action<MoveRequest[]>            onEndTurn           { set; get; }
    public Func<bool>                       onUndoTurn          { set; get; }
    public Action<bool>                     onGameOver          { set; get; }
    

    public void Start(GameMatchState matchState)
    {
        _matchState  = matchState;

        _setupFX();
        
        viewFactory.CreateAsync<PlayFieldView>("PlayFieldView", (_view) =>
        {
            _playfieldView = _view as PlayFieldView;
            view = _view;

            _playfieldView.onIntroFinishedEvent += _playfieldView_OnIntroTransitionEvent;
            _playfieldView.confirmButton.onClick.AddListener(_onConfirmTurnButton);
            _playfieldView.undoButton.onClick.AddListener(_onUndoButton);
            _playfieldView.exitButton.onClick.AddListener(_onExitButton);

            _playfieldView.SetActivePlayer(activePlayer.index);

            if(_networkManager.isConnected)
            {
                _playfieldView.SetThisPlayer(PhotonNetwork.player.NickName);
            }

            for(int i = 0; i < _matchState.playerGroup.playerCount; ++i)
            {
                PlayerState player = _matchState.playerGroup.GetPlayerByIndex(i);
                _playfieldView.SetPlayerName(i, player.name);
                _playfieldView.SetPlayerScore(i, player.score);
            }

            _setupActiveCustomersView(_playfieldView.staticCardLayer);
            _createPlayerHandView(localPlayer.index, _playfieldView.staticCardLayer);
        });
    }

    override public void RemoveView(bool immediately = false)
    {
        if (_hoverFX)
            GameObject.Destroy(_hoverFX.gameObject);

        if (_slamFX)
            GameObject.Destroy(_slamFX.gameObject);
        
        base.RemoveView(immediately);
    }


    public void RefreshCustomersView()
    {
        _refreshCustomersView();
    }
    public void RefreshHandView()
    {
        _refreshHandView(localPlayer);
    }

    private void _playfieldView_OnIntroTransitionEvent(UIView p_view)
    {
        Debug.Log(string.Format("View {0} has fininished intro", p_view.name));
    }

    private void _setupFX()
    {
        Transform canvasTransform = Singleton.instance.gui.mainCanvas.transform;
        GameObject hoverObj = Singleton.instance.vfxBank.Create(VFXType.CARD_HOVER, canvasTransform);
        _hoverFX = hoverObj.GetComponent<ParticleSystem>();
        _hoverFX.gameObject.SetActive(false);


        GameObject slamObj = Singleton.instance.vfxBank.Create(VFXType.CARD_SLAM, canvasTransform);
        _slamFX = slamObj.GetComponent<ParticleSystem>();
    }

    private void _setupActiveCustomersView(Transform parent)
    {
        viewFactory.CreateAsync<ActiveCustomersView>("ActiveCustomersView", (view) =>
            {
                _activeCustomersView = (ActiveCustomersView)view;
                for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
                {
                    CustomerCardState state = _matchState.activeCustomerSet.GetCustomerByIndex(i);
                    _setupCustomerView(i, state);
                }
            }, 
            parent
        );
    }

    private void _setupCustomerView(int customerSlot, CustomerCardState cardState)
    {
        PlayFieldViewUtils.SetupCustomerView(
            _activeCustomersView,
            customerSlot,
            cardState,
            _handleIngredientCardDrop,
            _handleIngredientCardHover,
            _deactiveHoverFX);
    }

    private void _setupIngredientView(int handSlot, IngredientCardData cardData)
    {
        PlayFieldViewUtils.SetupIngredientView(
            _playerHandView,
            handSlot, 
            cardData,
            _handleIngredientCardBeginDrag,
            _handleEndDrag);
    }

    private void _handleIngredientCardDrop(CustomerCardView customerView)
    {
        Debug.Log("LocalPlayer: " + localPlayer.index + ", Active Player: " + activePlayer.index);

        if (localPlayer != activePlayer) { return; }
        if (_draggedIngredient == null) { return; }

        CustomerCardState customerState = customerView.cardState;
        IngredientCardData ingredientData = _draggedIngredient.cardData as IngredientCardData;

        if (!customerState.CanAcceptCard(ingredientData)) { return; }
        
        MoveRequest move = MoveRequest.Create(
            localPlayer.index,
            _draggedIngredient.handIndex,
            customerState.slotIndex);

        Assert.IsNotNull(onPlayOnCustomer);
        _draggedIngredient.isDropSuccessfull = onPlayOnCustomer(move);
        
        if(_draggedIngredient.isDropSuccessfull)
        {       
            _droppedCustomer = customerView;
            _moveRequests[localPlayer.cardsPlayed - 1] = move;
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

    private void _handleIngredientCardBeginDrag(IngredientCardView view)
    {
        _draggedIngredient = view;
    }

    private void _handleEndDrag()
    {
        _deactiveHoverFX();

        if(_draggedIngredient.isDropSuccessfull)
        {
            _playerHandView.blockCardDrag = true;

            PlayFieldViewUtils.ZoomSlamTween(
            _draggedIngredient,
            _droppedCustomer,
            _onCardSlam,
            _onCardDropFinished);
        }
    }
    
    private void _onCardSlam()
    {
        _draggedIngredient.gameObject.SetActive(false);
        _droppedCustomer.invalidateFlag = UIView.InvalidationFlag.DYNAMIC_DATA;
        _slamFX.transform.position = _draggedIngredient.transform.position;
        _slamFX.Play();
    }

    private void _onCardDropFinished()
    {
        int handIndex       = _draggedIngredient.handIndex;
        int customerIndex   = _droppedCustomer.cardState.slotIndex;

        _draggedIngredient = null;

        _playerHandView.RemoveCardByIndex(handIndex);

        Assert.IsNotNull(onResolveScore);
        bool customerFinished = onResolveScore(customerIndex);
        if (customerFinished)
        {
            _playfieldView.SetPlayerScore(activePlayer.index, activePlayer.score);
            CustomerCardState newState = _matchState.activeCustomerSet.GetCustomerByIndex(customerIndex);
            _setupCustomerView(customerIndex, newState);
        }

        _playerHandView.blockCardDrag = false;

        //if (_matchState.isGameOver)
        //{
        //    Assert.IsNotNull(onGameOver);
        //    onGameOver(true);
        //}
    }

    public void SetPlayerScoreView(int playerIndex, int score)
    {
        _playfieldView.SetPlayerScore(playerIndex, score);
    }
    private PlayerState localPlayer
    {
        get
        {
            PlayerGroup playerGroup = _matchState.playerGroup;
            if (_networkManager.isConnected)
            {

                return playerGroup.GetPlayerById(PhotonNetwork.player.ID);
            }
            else
            {
                return playerGroup.GetPlayerByIndex(0);
            }
        }
    }

    private PlayerState activePlayer
    {
        get { return _matchState.playerGroup.activePlayer; }
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
        PlayerState player = _matchState.playerGroup.GetPlayerByIndex(playerIndex);
        for (int i = 0; i < PlayerState.kHandSize; ++i)
        {
            IngredientCardData ingredientCard = player.hand.GetCard(i);
            _setupIngredientView(i, ingredientCard);
        }
    }

    private void _onConfirmTurnButton()
    {
        if(localPlayer.id != activePlayer.id)
        {
            return;
        }

        Assert.IsNotNull(onEndTurn);
        onEndTurn(_moveRequests);

        for(int i = 0; i < _moveRequests.Length; ++i)
        {
            _moveRequests[i] = MoveRequest.Invalid();
        }

        _refreshHandView(localPlayer);
        _playfieldView.SetActivePlayer(activePlayer.index);
    }

    private void _onUndoButton()
    {
        Assert.IsNotNull(onUndoTurn);
        
        bool didUndo = onUndoTurn();
        if (!didUndo) { return; }
        
        _refreshHandView(localPlayer);
        _refreshCustomersView();
    }

    private void _onExitButton()
    {
        _playfieldView.exitButton.onClick.RemoveListener(_onExitButton);
        Assert.IsNotNull(onGameOver);
        onGameOver(false);
    }

    private void _refreshCustomersView()
    {
        _activeCustomersView.invalidateFlag = UIView.InvalidationFlag.ALL;
        for (int i = 0; i < ActiveCustomerSet.kMaxActiveCustomers; ++i)
        {
            _setupCustomerView(i, _matchState.activeCustomerSet.GetCustomerByIndex(i));
        }
    }

    private void _refreshHandView(PlayerState player)
    {
        _playfieldView.SetPlayerScore(player.index, player.score);

        _playerHandView.invalidateFlag = UIView.InvalidationFlag.ALL;
        for (int i = 0; i < PlayerState.kHandSize; ++i)
        {
            _setupIngredientView(i, player.hand.GetCard(i));
        }
        
        _playfieldView.SetActivePlayer(activePlayer.index);
    }
}
