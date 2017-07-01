using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using GhostGen;
using DG.Tweening;

public class PlayerHandView : UIView
{
    public Transform[] cardSlotList;

    public CanvasGroup canvasGroup;
    public Transform dragCardLayer;
    public Image dragBlocker;

    private IngredientCardView[] _cardViewList;

    void Awake()
    {
        _cardViewList = new IngredientCardView[PlayerState.kHandSize];
        blockCardDrag = false;
    }
    
    void Start()
    {
        OnIntroTransitionFinished();
    }

    public bool blockCardDrag
    {
        set
        {
            dragBlocker.gameObject.SetActive(value);
        }
    }
    
    public void SetCardAtIndex(int index, IngredientCardView card)
    {
        _boundsCheck(index);
        if(card != _cardViewList[index])
        {
            _cardViewList[index] = _processCardView(index, card);
            invalidateFlag |= InvalidationFlag.STATIC_DATA;
        }
    }

    public IngredientCardView GetCardAtIndex(int index)
    {
        _boundsCheck(index);
        return _cardViewList[index];
    }

    public void RemoveCardByIndex(int index)
    {
        _boundsCheck(index);
        if(_cardViewList[index])
        {
            //_cardViewList[index].gameObject.SetActive(false);
            Singleton.instance.viewFactory.RemoveView(_cardViewList[index]);
            _cardViewList[index] = null;
            invalidateFlag = InvalidationFlag.ALL;
        }
    }


    protected override void OnViewUpdate()
    {
        if(IsInvalid(InvalidationFlag.STATIC_DATA))
        {
            for(int i = 0; i < _cardViewList.Length; ++i)
            {
                IngredientCardView cardView = _cardViewList[i];
                if (cardView)
                {
                    cardView.invalidateFlag = InvalidationFlag.STATIC_DATA;
                }
            }
        }
    }

    private IngredientCardView _processCardView(
        int handIndex,
        IngredientCardView cardView)
    {
        if (cardView == null) { return null; }
        _boundsCheck(handIndex);

        cardView.transform.SetParent(cardSlotList[handIndex]);
        cardView.transform.localPosition = Vector3.zero;
        cardView.handView = this;
        cardView.handSlot = cardSlotList[handIndex];
        cardView.dragLayer = dragCardLayer;
        cardView.handIndex = handIndex;
        return cardView;
    }

    private void _boundsCheck(int index)
    {
        Debug.Assert(index >= 0, "Index is less than 0!");
        Debug.Assert(index < PlayerHand.kDefaultHandSize, "Index is greater than slot container size");
    }
}

