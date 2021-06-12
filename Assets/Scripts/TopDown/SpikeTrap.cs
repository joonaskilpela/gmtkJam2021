public class SpikeTrap : GridObject
{
    public override void Destroy()
    {
        OnObjectDestroyed.Invoke();
    }
}
