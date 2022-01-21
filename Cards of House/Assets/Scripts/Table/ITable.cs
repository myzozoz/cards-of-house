public interface ITable: IStage
{
    public bool TryToggleSelect(System.Guid id);
    public bool ReadyToSubmit();
    public void SubmitCards();
}
