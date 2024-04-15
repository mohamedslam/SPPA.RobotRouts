using System;
using System.Security.Permissions;

namespace Stimulsoft.System.Security.Permissions
{
    sealed public class SecurityPermission
    {
        private SecurityPermissionFlag m_flags;

        public SecurityPermission(PermissionState state)
        {
            if (state == PermissionState.Unrestricted)
            {
                SetUnrestricted(true);
            }
            else if (state == PermissionState.None)
            {
                SetUnrestricted(false);
                Reset();
            }
            else
            {
                throw new ArgumentException("Argument_InvalidPermissionState");
            }
        }

        public SecurityPermission(SecurityPermissionFlag flag)
        {

            SetUnrestricted(false);
            m_flags = flag;
        }

        private void SetUnrestricted(bool unrestricted)
        {
            if (unrestricted)
            {
                m_flags = SecurityPermissionFlag.AllFlags;
            }
        }

        private void Reset()
        {
            m_flags = SecurityPermissionFlag.NoFlags;
        }


        public SecurityPermissionFlag Flags
        {
            set
            {

                m_flags = value;
            }

            get
            {
                return m_flags;
            }
        }

        public void Demand()
        {
        }
    }
}