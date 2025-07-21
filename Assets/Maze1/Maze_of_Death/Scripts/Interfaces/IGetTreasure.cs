
public interface IGetTreasure
{
    bool IsOpened { get; set; }
    void GetTreasure(int count);
    void BoxOpened();
}
