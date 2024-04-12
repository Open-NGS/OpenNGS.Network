using System.Collections.Generic;


namespace OpenNGS.Systems
{
    public interface IStatisticSystem
    {
    }

    public interface IStatisticEvent
    {
        int StatID { get; }
        void OnStatValueChange(uint statId, double value);
    }

}