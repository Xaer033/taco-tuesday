using UnityEngine;
using System.Collections;
using GhostGen;
using DG.Tweening;

public class PlayerHandView : UIView
{

    public Transform[] cardSlotList;

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
        if(_cardDataList[index] == null || card.id != _cardDataList[index].id)
        {
            _cardDataList[index] = card;
            invalidateFlag = INVALIDATE_STATIC_DATA;
        }
    }

    protected override void OnViewUpdate()
    {
        if(IsInvalid(INVALIDATE_STATIC_DATA))
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
                        GameManager.Get().viewFactory.RemoveView(cardView);
                        _cardViewList[i] = null;
                    }
                    continue;
                }
                
                if(cardView == null)
                {
                    _cardViewList[i] = (IngredientCardView)GameManager.cardResourceBank.CreateCardView(cardData, cardParent);
                    _cardViewList[i].transform.localPosition = Vector3.zero;
                }
                else if(cardData.id != cardView.cardData.id)
                {
                    _cardViewList[i].cardData = cardData;
                }
                
            }
        }
    }
}

