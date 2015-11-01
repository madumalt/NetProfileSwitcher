using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using Microsoft.Win32;
using System.Runtime.InteropServices;

namespace NetProfileSwitcher
{
    class ProxySetting_Help
    {

        [DllImport("wininet.dll")]
        //InternetSetOption API in WinINET.
        public static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int dwBufferLength);
        public const int INTERNET_OPTION_SETTINGS_CHANGED = 39;
        public const int INTERNET_OPTION_REFRESH = 37;
        static bool settingsReturn, refreshReturn;


        ///<summary>
        /// retreives the current proxy settings in the registry
        /// </summary>
        ///
        public ArrayList getCurrentProxy(){

            ArrayList Csettings = new ArrayList();

            //accessing registry key
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            return Csettings;
        }


        ///<summary>
        ///set the manually entered proxy setting in the registry
        ///</summary>
        ///<param name="http">httpProxyServer:port</param>
        ///<param name="secure">secure(https)ProxrServer:port</param>
        ///<param name="ftp">ftpProxyServer:Port</param>
        ///<param name="socks">sockProxyServer:port</param>
        ///  ///<param name="byPassLocal">enable/disable by passing proxy for local addresses</param>
        ///<param name="proxyExceptions">proxy exceptions</param>
        ///<param name="autoDetect">enable/disable automatic proxy detection</param>
        ///
        public bool setManualProxy(bool sameproxy, string proxy, bool byPassLocal, string proxyExceptions, bool autoDetect)
        {

            //accessing registry key
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);
            //enabling proxy
            registry.SetValue("ProxyEnable", 1);
            //assignment of proxy server
            registry.SetValue("ProxyServer", proxy);

            #region proxy exception
            if (byPassLocal)
            {
                if (string.IsNullOrEmpty(proxyExceptions)) registry.SetValue("ProxyOverride", "<local>");
                else
                {
                    string exceptions = string.Concat("<local>;", proxyExceptions);
                    registry.SetValue("ProxyOverride", exceptions);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(proxyExceptions)) registry.SetValue("ProxyOverride", proxyExceptions);
                else { registry.SetValue("ProxyOverride", string.Empty); }
            }
            #endregion

            #region automatic proxy detection enable/disable
            if (autoDetect) registry.SetValue("AutoDetect", 1);
            else registry.SetValue("AutoDetect", 0);
            #endregion

            if (InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0))
            {
                // Success. Announce to the world that we've changed the proxy
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                return true;
            }

            return false;
        }


        ///<summary>
        ///set automatic proxy configuration using the given(by the URL) proxy configuration script
        /// </summary>
        /// <param name="ScriptURL">URL of automatic proxy configuration script</param>
        ///  ///<param name="byPassLocal">enable/disable by passing proxy for local addresses</param>
        ///<param name="proxyExceptions">proxy exceptions</param>
        ///<param name="autoDetect">enable/disable automatic proxy detection</param>
        /// 
        public bool setProxyScript(string ScriptURL, bool byPassLocal, string proxyExceptions, bool autoDetect) {
            //accessing registry key
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            //assignment of automatic configuration script
            registry.SetValue("AutoConfigURL", ScriptURL);

            #region proxy exception
            if (byPassLocal)
            {
                if (string.IsNullOrEmpty(proxyExceptions)) registry.SetValue("ProxyOverride", "<local>");
                else
                {
                    string exceptions = string.Concat("<local>;", proxyExceptions);
                    registry.SetValue("ProxyOverride", exceptions);
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(proxyExceptions)) registry.SetValue("ProxyOverride", proxyExceptions);
            }
            #endregion

            #region automatic proxy detection enable/disable
            if (autoDetect) registry.SetValue("AutoDetect", 1);
            else registry.SetValue("AutoDetect", 0);
            #endregion

            if (InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0))
            {
                // Success. Announce to the world that we've changed the proxy
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                return true;
            }

            return false;
        }


        ///<summary>
        ///disabling enabling automatic proxy detection
        /// </summary>
        ///<param name="autoDetect">enable/disable automatic proxy detection</param>
        /// 
        public bool automaticProxyDetection(bool autoDetect) {

            //accessing registry key
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            #region automatic proxy detection enable/disable
            if (autoDetect) registry.SetValue("AutoDetect", 1);
            else registry.SetValue("AutoDetect", 0);
            #endregion

            if (InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0))
            {
                // Success. Announce to the world that we've changed the proxy
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                return true;
            }
            return false;
        }


        ///<summary>
        ///disable proxy settings
        /// </summary>
        /// 
        public bool disableProxy() {

            //accessing registry key
            RegistryKey registry = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            //disabling proxy
            registry.SetValue("ProxyEnable", 0);

            if (InternetSetOption(IntPtr.Zero, INTERNET_OPTION_SETTINGS_CHANGED, IntPtr.Zero, 0))
            {
                // Success. Announce to the world that we've changed the proxy
                InternetSetOption(IntPtr.Zero, INTERNET_OPTION_REFRESH, IntPtr.Zero, 0);
                return true;
            }
            return false;
        }

    }
}
