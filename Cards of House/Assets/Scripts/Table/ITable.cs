public interface ITable: IStage
{
    public void TryToggleSelect(System.Guid id);
    public bool ReadyToSubmit();
    public void SubmitCards();
    public void SendInputToCard(int index);
}
