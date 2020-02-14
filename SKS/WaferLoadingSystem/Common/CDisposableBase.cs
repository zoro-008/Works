using System;

namespace COMMON
{
    public class CDisposable:IDisposable
    {
        private bool m_bDisposed = false;

        ~CDisposable()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if(m_bDisposed)
                return;

            if(isDisposing)
            {
                // TODO : free managed resources here. 

            }
            // TODO : free unmanged resources here. 
            // Call the appropriate methods to clean up
            // unmanaged resources here.
            // If disposing is false,
            // only the following code is executed.
            //CloseHandle(handle);
            //handle = IntPtr.Zero;

            m_bDisposed = true;
        }
    }

    // 위의class를base class 로하는derived class를정의한다면 
/*
    public class DerivedResourceHog:CDisposable
    {
        private bool m_bDisposed = false;

        protected override void Dispose(bool isDisposing)
        {
            if(m_bDisposed)
                return;

            if(isDisposing)
            {
                // TODO : Free managed resources here 
            }
            // TODO : Free unmanaged resources here 

            base.Dispose(isDisposing);
            m_bDisposed = true;
        }
    } 
 */

}
