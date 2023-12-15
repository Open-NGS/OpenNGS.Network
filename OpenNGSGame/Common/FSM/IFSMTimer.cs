using System;

namespace MissQ
{
    public interface IFSMTimer
    {
        int FsmAddTimer(FP interval, bool repeat, Action callback);

        void FsmRemoveTimer(int timerId);
    }
}
