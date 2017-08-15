using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;


public class PlayFieldViewUtils 
{
	public static Sequence ZoomSlamTween(
        BaseCardView ingredient,
        BaseCardView customer,
        bool shouldMoveTween,
        TweenCallback slamCallback,
        TweenCallback finishCallback)
    {
        Vector3 originalScale = ingredient.transform.localScale;

        if(shouldMoveTween)
        {

        }
        Vector3 cardEyeVec = (Camera.main.transform.position - customer.transform.position).normalized;
        Sequence sequence = DOTween.Sequence();
        Tween moveToTween = null;
        if (shouldMoveTween)
        {
            moveToTween = ingredient.transform.DOMove(customer.transform.position + cardEyeVec, 0.27f);
            moveToTween.SetEase(Ease.OutCubic);
        }

        Tween growTween = ingredient.transform.DOScale(originalScale * 1.3f, 0.31f);
        growTween.SetEase(Ease.OutCubic);

        Tween slamTween = ingredient.transform.DOScale(originalScale * 0.1f, 0.2f);
        slamTween.SetEase(Ease.InCubic);

        Sequence shakeSeq = DOTween.Sequence();
        Tween shakePosTween = customer.transform.DOShakePosition(0.4f, 10.0f, 22);
        shakePosTween.SetEase(Ease.OutCubic);
        Tween shakeRotTween = customer.transform.DOShakeRotation(0.4f, 6.0f, 16);
        shakeRotTween.SetEase(Ease.OutCubic);
        shakeSeq.Insert(0.0f, shakePosTween);
        shakeSeq.Insert(0.0f, shakeRotTween);
        if(shouldMoveTween)
        {
            sequence.Insert(0.0f, moveToTween);
        }
        sequence.Insert(0.0f, growTween);
        sequence.Append(slamTween);

        if(slamCallback != null)
        {
            sequence.AppendCallback(slamCallback);
        }

        sequence.Append(shakeSeq);

        if(finishCallback != null)
        {
            sequence.OnComplete(finishCallback);
        }
        return sequence;
    }

    public static void SetupIngredientView(
        PlayerHandView handView, 
        int handSlot, 
        IngredientCardData cardData, 
        Action<IngredientCardView> onBeginDrag, 
        Action onEndDrag)
    {
        if (cardData == null)
        {
            Debug.LogWarning("Card Data is null!");
            handView.RemoveCardByIndex(handSlot); // TODO: Do this On card slam instead of after the fact    
            return;
        }

        IngredientCardView view = handView.GetCardAtIndex(handSlot);
        if (view == null)
        {
            view = Singleton.instance.cardResourceBank.CreateCardView(
                cardData,
                handView.cardSlotList[handSlot]) as IngredientCardView;
        }

        view.cardData = cardData;
        
        view.eventTrigger.triggers.Clear();
        EventTrigger.Entry OnBeginDrag = new EventTrigger.Entry();
        OnBeginDrag.eventID = EventTriggerType.BeginDrag;
        OnBeginDrag.callback.AddListener((e) => onBeginDrag(view));
        view.eventTrigger.triggers.Add(OnBeginDrag);

        EventTrigger.Entry OnEndDrag = new EventTrigger.Entry();
        OnEndDrag.eventID = EventTriggerType.EndDrag;
        OnEndDrag.callback.AddListener((e)=> onEndDrag());
        view.eventTrigger.triggers.Add(OnEndDrag);

        handView.SetCardAtIndex(handSlot, view);
    }

    static public void SetupCustomerView(
        ActiveCustomersView customersView, 
        int customerIndex, 
        CustomerCardState cardState,
        Action<CustomerCardView> onCardDrop,
        Action<CustomerCardView> onPointerEnter,
        Action onPointerExit)
    {
        if (cardState == null)
        {
            Debug.LogWarning("Card State is null!");
            customersView.RemoveCardByIndex(customerIndex); // TODO: Do this On card slam instead of after the fact
            return;
        }

        CustomerCardView view = customersView.GetCardByIndex(customerIndex);
        if (view == null)
        {
            view = (CustomerCardView)Singleton.instance.cardResourceBank.CreateCardView(
            cardState.cardData,
            customersView._activeSlotList[customerIndex]);
        }

        view.cardState = cardState;

        view.eventTrigger.triggers.Clear();
        EventTrigger.Entry OnDrop = new EventTrigger.Entry();
        OnDrop.eventID = EventTriggerType.Drop;
        OnDrop.callback.AddListener((e) => onCardDrop(view));
        view.eventTrigger.triggers.Add(OnDrop);

        EventTrigger.Entry OnHoverBegin = new EventTrigger.Entry();
        OnHoverBegin.eventID = EventTriggerType.PointerEnter;
        OnHoverBegin.callback.AddListener((e) => onPointerEnter(view));
        view.eventTrigger.triggers.Add(OnHoverBegin);

        EventTrigger.Entry OnHoverEnd = new EventTrigger.Entry();
        OnHoverEnd.eventID = EventTriggerType.PointerExit;
        OnHoverEnd.callback.AddListener((e) => onPointerExit());
        view.eventTrigger.triggers.Add(OnHoverEnd);
        
        customersView.SetCardByIndex(customerIndex, view);
    }

    public static void AnimateOtherPlayerMoves(List<MoveResult> moveList, GameMatchState state, ActiveCustomersView customersView, Transform parent, Action<Vector3> slamCallback, TweenCallback callback)
    {
        Assert.IsNotNull(moveList);
        //Assert.IsTrue(moveList.Count > 0);

        Sequence allPlayedCards = DOTween.Sequence();
        //MoveResult[] moveResults = new MoveResult[PlayerGroup.kMaxPlayerCount];
        int count = moveList.Count;
        for(int i = 0; i < count; ++i)
        {
            MoveRequest request = moveList[i].request;
            MoveResult result = moveList[i];

            CustomerCardView customerView = customersView.GetCardByIndex(request.customerSlot);
            
            IngredientCardData cardData = state.GetCardById(result.usedIngredient) as IngredientCardData;

            if(cardData == null)
            {
                Debug.LogError("Card data is null! cant animate other player card");
                continue;
            }

            IngredientCardView ingredientView = Singleton.instance.cardResourceBank.CreateCardView(
                cardData,
                parent) as IngredientCardView;

            ingredientView.transform.position = parent.transform.position;

            Vector3 delta = (Camera.main.transform.position - customerView.transform.position).normalized;
            Vector3 targetPos = customerView.transform.position + delta;
            Tween moveTo = ingredientView.transform.DOMove(targetPos, 0.75f);
            moveTo.SetEase(Ease.OutQuad);

            ViewFactory vf = Singleton.instance.gui.viewFactory;

            Tween zoomSlam = ZoomSlamTween(ingredientView, customerView, false, ()=>
            {
                vf.RemoveView(ingredientView);
                if(slamCallback != null)
                {
                    slamCallback(ingredientView.transform.position);
                }
            }, null);

            Sequence cardTween = DOTween.Sequence();
            cardTween.Append(moveTo);
            cardTween.Insert(moveTo.Duration(false) * 0.75f, zoomSlam);

            allPlayedCards.Insert(i * (cardTween.Duration() * 0.95f), cardTween);
        }
        allPlayedCards.OnComplete(callback);
        allPlayedCards.Play();
    }
    
}
