using System.Collections.Generic;

namespace Storm
{
    public interface IStormValue
    {
        bool TryGetEntry(string name, out IStormValue entry, bool ignoreCase);
        void GetEntries(List<IStormValue> entries);
        void Populate(IStormVariableW variable, StormContext ctx);
    }
}
