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
    //private IngredientCardData[] _cardDataList;

    void Awake()
    {
        //_cardDataList = new IngredientCardData[PlayerState.kHandSize];
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


    public void SetCardAtIndex(int index, IngredientCardView card)
    {
        if(card != _cardViewList[index])
        {
            _cardViewList[index] = _processCardView(index, card);
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
            for(int i = 0; i < _cardViewList.Length; ++i)
            {
                IngredientCardView cardView = _cardViewList[i];
                cardView.invalidateFlag = InvalidationFlag.ALL;
            }
        }
    }

    private IngredientCardView _processCardView(
        int handIndex,
        IngredientCardView cardView)
    {
        if (cardView == null) { return null; }

        cardView.transform.SetParent(cardSlotList[handIndex]);
        cardView.transform.localPosition = Vector3.zero;
        cardView.handView = this;
        cardView.handSlot = cardSlotList[handIndex];
        cardView.dragLayer = dragCardLayer;
        cardView.handIndex = handIndex;
        return cardView;
    }
}

