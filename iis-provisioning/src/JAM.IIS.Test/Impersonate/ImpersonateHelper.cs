using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;

namespace JAM.IIS.Test
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ProfileInfo
    {
        /// 
        /// Specifies the size of the structure, in bytes.
        /// 
        public int dwSize;

        /// 
        /// This member can be one of the following flags: PI_NOUI or PI_APPLYPOLICY
        /// 
        public int dwFlags;

        /// 
        /// Pointer to the name of the user. 
        /// This member is used as the base name of the directory in which to store a new profile. 
        /// 
        public string lpUserName;

        /// 
        /// Pointer to the roaming user profile path. 
        /// If the user does not have a roaming profile, this member can be NULL.
        /// 
        public string lpProfilePath;

        /// 
        /// Pointer to the default user profile path. This member can be NULL. 
        /// 
        public string lpDefaultPath;

        /// 
        /// Pointer to the name of the validating domain controller, in NetBIOS format. 
        /// If this member is NULL, the Windows NT 4.0-style policy will not be applied. 
        /// 
        public string lpServerName;

        /// 
        /// Pointer to the path of the Windows NT 4.0-style policy file. This member can be NULL. 
        /// 
        public string lpPolicyPath;

        /// 
        /// Handle to the HKEY_CURRENT_USER registry key. 
        /// 
        public IntPtr hProfile;
    }

    /// <summary>
    /// Provides the functionality of impersonating a domain or local PC user.
    /// Microsoft KB link for impersonation: http://support.microsoft.com/kb/306158
    /// </summary>
    public class ImpersonateHelper: IDisposable
    {
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        #region PInvoke
        [DllImport("advapi32.dll")]
        public static extern int LogonUser(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        /// <summary>
        /// A process should call the RevertToSelf function after finishing any impersonation begun by using the DdeImpersonateClient, ImpersonateDdeClientWindow, ImpersonateLoggedOnUser, ImpersonateNamedPipeClient, ImpersonateSelf, ImpersonateAnonymousToken or SetThreadToken function.
        /// If RevertToSelf fails, your application continues to run in the context of the client, which is not appropriate. You should shut down the process if RevertToSelf fails.
        /// RevertToSelf Function: http://msdn.microsoft.com/en-us/library/aa379317(VS.85).aspx
        /// </summary>
        /// <returns>A boolean value indicates the function succeeded or not.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LoadUserProfile(IntPtr hToken, ref ProfileInfo lpProfileInfo);

        [DllImport("Userenv.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UnloadUserProfile(IntPtr hToken, IntPtr lpProfileInfo);

        #endregion

        private static WindowsImpersonationContext m_ImpersonationContext = null;

        public ImpersonateHelper(string userName, string password, string domain="")
        {
            WindowsIdentity m_ImpersonatedUser;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            const int SecurityImpersonation = 2;
           
            try
            {
                if (domain == "")
                    domain = Environment.MachineName;

                if (RevertToSelf())
                {
                    Console.WriteLine("Before impersonation: " +
                                      WindowsIdentity.GetCurrent().Name);

                    //String userName = "TempUser";
                    //IntPtr pass = GetPassword(password);
                    if (LogonUser(userName, domain, password, LOGON32_LOGON_INTERACTIVE,
                                  LOGON32_PROVIDER_DEFAULT, ref token) != 0)
                    {
                        if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                        {
                            m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                            var m_ImpersonationContext = m_ImpersonatedUser.Impersonate();

    
                                if (m_ImpersonationContext != null)
                                {
                                    Console.WriteLine("After Impersonation succeeded: " + Environment.NewLine +
                                                      "User Name: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                                      Environment.NewLine +
                                                      "SID: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                                          Value);

                                }
                            
                        }
                        else
                        {
                            Console.WriteLine("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                            throw new Win32Exception(Marshal.GetLastWin32Error());
                        }
                    }
                }
            }
            catch (Win32Exception we)
            {
                throw we;
            }
            catch
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }
        public void Undo()
        {
            if (m_ImpersonationContext != null)
            {
                m_ImpersonationContext.Undo();
                m_ImpersonationContext.Dispose();
                Console.WriteLine("After finished impersonation: " + WindowsIdentity.GetCurrent().Name);
            }
        }
        private static IntPtr GetPassword(string pass)
        {
            IntPtr password = IntPtr.Zero;

            using (SecureString secureString = new SecureString())
            {
                foreach (char c in pass)
                    secureString.AppendChar(c);

                // Lock the password down
                secureString.MakeReadOnly();

                password = Marshal.SecureStringToBSTR(secureString);
            }

            return password;
        }

        public void Dispose()
        {
            Undo();
        }
    }
}
