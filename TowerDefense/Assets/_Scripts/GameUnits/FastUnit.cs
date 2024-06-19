public class FastUnit : Unit
{
    public FastUnit() 
    {
    }
    public override void Start()
    {
        base.Start();
        SetHealth(80);
        SetSpeed(_moveSpeed * 1.5f);
        _isFlying = true;
    }
}