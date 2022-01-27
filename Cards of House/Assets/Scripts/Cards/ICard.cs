using UnityEngine;

public interface ICard
{
    public System.Guid GetId();
    public void FlyToHand(int index);
    public Card.State CardState { get; set; }
    public void Disappear();
    public void Delete();

    public GameObject GetSpawnableUnit();
}
