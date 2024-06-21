public class FastUnit : Unit
{

    public FastUnit() 
    {
        _costToSend = 50f;
        _incomeIncrease = 10f;
    }
    public override void Start()
    {
        base.Start();
        SetHealth(80);
        SetSpeed(_moveSpeed * 1.5f);
        _isFlying = false;
    }
    public override float GetCostToSend()
    {
        return _costToSend;
    }
}