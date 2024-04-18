namespace Ston
{
    public interface IStonValue
    {
        void Populate(StonFieldOrProperty fieldOrProperty, object obj, StonSettings settings);
    }
}
