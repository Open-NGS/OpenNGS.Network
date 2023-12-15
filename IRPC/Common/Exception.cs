using System;

namespace OpenNGS.IRPC
{
    public class IRPCException : System.Exception
    {
        private int m_errno;

        public IRPCException(int errno, string msg) : base(msg)
        {
            m_errno = errno;
        }

        public int Errno { get { return m_errno; } }
    }
}
