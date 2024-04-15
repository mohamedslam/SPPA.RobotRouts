using System;

namespace Stimulsoft.System.Security.Permissions
{
    public enum ReflectionPermissionFlag
    {
        NoFlags = 0x00,
        TypeInformation = 0x01,
        MemberAccess = 0x02,
        ReflectionEmit = 0x04,
        RestrictedMemberAccess = 0x08,
        AllFlags = 0x07
    }

    sealed public class ReflectionPermission
    {
        internal const ReflectionPermissionFlag AllFlagsAndMore = ReflectionPermissionFlag.AllFlags | ReflectionPermissionFlag.RestrictedMemberAccess;

        private ReflectionPermissionFlag m_flags;

        public ReflectionPermission(PermissionState state)
        {
            if (state == PermissionState.Unrestricted)
            {
                SetUnrestricted(true);
            }
            else if (state == PermissionState.None)
            {
                SetUnrestricted(false);
            }
            else
            {
                throw new ArgumentException("Argument_InvalidPermissionState");
            }
        }

        public ReflectionPermission(ReflectionPermissionFlag flag)
        {

            SetUnrestricted(false);
            m_flags = flag;
        }

        private void SetUnrestricted(bool unrestricted)
        {
            if (unrestricted)
            {
                m_flags = ReflectionPermission.AllFlagsAndMore;
            }
            else
            {
                Reset();
            }
        }


        private void Reset()
        {
            m_flags = ReflectionPermissionFlag.NoFlags;
        }


        public ReflectionPermissionFlag Flags
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