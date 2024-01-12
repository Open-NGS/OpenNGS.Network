using System;

namespace OpenNGS.ERPC
{
    public class ERPCException : System.Exception
    {
        private int m_errno;

        public ERPCException(int errno, string msg) : base(msg)
        {
            m_errno = errno;
        }

        public int Errno { get { return m_errno; } }
    }
}
