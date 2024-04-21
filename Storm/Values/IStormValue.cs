namespace Storm
{
    public interface IStormValue
    {
        void Populate(IStormVariable variable, StormSettings settings);
    }
}
