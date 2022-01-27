using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHand: IStage
{
    public void TryToggleSelect(System.Guid id);
    public bool ReadyToSubmit();
    public ICard GetSelectedCard();
    public void LockSelectedCard();
}
