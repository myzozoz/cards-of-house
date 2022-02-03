using UnityEngine;

public interface ICard: IClickable
{
    public System.Guid GetId();
    public void FlyToHand(int index);
    public Card.State CardState { get; set; }
    public void Disappear();
    public void Delete();
    public GameObject GetSpawnableUnit();
    public int TIndex { get; set; }
}
