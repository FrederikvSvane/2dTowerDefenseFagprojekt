public class FastUnit : Unit
{
    private float _costToSend = 50f;
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
    public override float GetCostToSend()
    {
        return _costToSend;
    }
}