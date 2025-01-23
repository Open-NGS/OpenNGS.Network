using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNGS.Platform
{
    public interface IAchievementProvider : IModuleProvider
    {
        void Unlock(int id);
        void UnlockProgress(int id, long value);

        void Unlock(string key);
        void UnlockProgress(string key, long value);
        void ResetAllAchievements();
    }
}
