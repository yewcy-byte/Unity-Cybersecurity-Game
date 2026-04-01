using UnityEngine;

public interface IChestable
{
    int ChestID { get; }
    bool IsOpened { get; }
    void SetOpened(bool opened);
}