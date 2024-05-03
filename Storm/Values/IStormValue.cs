using System.Collections.Generic;

namespace Storm
{
    public interface IStormValue
    {
        void GetEntries(List<IStormValue> entries);
        void Populate(IStormVariable variable, StormContext ctx);
    }
}
