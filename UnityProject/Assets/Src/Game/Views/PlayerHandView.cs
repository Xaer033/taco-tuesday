using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using GhostGen;
using DG.Tweening;

public class PlayerHandView : UIView
{

    public Transform[] cardSlotList;

    public CanvasGroup canvasGroup;
    public Transform dragCardLayer;


    private IngredientCardView[] _cardViewList;
    private IngredientCardData[] _cardDataList;

    void Awake()
    {
        _cardDataList = new IngredientCardData[PlayerState.kHandSize];
        _cardViewList = new IngredientCardView[PlayerState.kHandSize];
    }
    
    void Start()
    {
        OnIntroTransitionFinished();
    }

    public override void OnViewOutro(bool immediately, OnViewRemoved removedCallback)
    {
        base.OnViewOutro(immediately, removedCallback);
    }


    public void SetCardAtIndex(int index, IngredientCardData card)
    {
        Assert.IsNotNull(card);
        if(_cardDataList[index] == null || card.id != _cardDataList[index].id)
        {
            _cardDataList[index] = card;
            invalidateFlag |= InvalidationFlag.STATIC_DATA;
        }
    }

    public IngredientCardView GetCardAtIndex(int index)
    {
        return _cardViewList[index];
    }

    protected override void OnViewUpdate()
    {
        if(IsInvalid(InvalidationFlag.STATIC_DATA))
        {
            for(int i = 0; i < _cardDataList.Length; ++i)
            {
                IngredientCardData cardData = _cardDataList[i];
                IngredientCardView cardView = _cardViewList[i];
                Transform cardParent = cardSlotList[i];

                if (cardData == null)
                {
                    if(cardView != null)
                    {
                        Singleton.instance.viewFactory.RemoveView(cardView);
                        _cardViewList[i] = null;
                    }
                    continue;
                }
                
                if(cardView == null)
                {
                    _cardViewList[i] = _createCardView(cardData, cardParent);
                }
                else if(cardData.id != cardView.cardData.id)
                {
                    _cardViewList[i].cardData = cardData;
                }
            }
        }
    }

    private IngredientCardView _createCardView(IngredientCardData cardData, Transform cardParent)
    {
        IngredientCardView cardView = (IngredientCardView)Singleton.instance.cardResourceBank.CreateCardView(cardData, cardParent);
        cardView.transform.localPosition = Vector3.zero;
        cardView.handView = this;
        cardView.handSlot = cardParent;
        cardView.dragLayer = dragCardLayer;
        return cardView;
    }
}

