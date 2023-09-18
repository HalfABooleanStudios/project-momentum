[System.Serializable]
public class ResetableVar<T> {
    public T trueValue;
    public T val;

    public ResetableVar(T initial)
    {
        trueValue = initial;
        val = initial;
    }

    public void Reset()
    {
        val = trueValue;
    }
}