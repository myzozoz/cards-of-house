public interface ICard
{
    public System.Guid GetId();
    public bool IsSelected();
    public void SetSelected(bool sel);
}
