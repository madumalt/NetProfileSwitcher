using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Networking.Connectivity;


namespace NetProfileSwitcher
{
    public class Windows_Networking_Connectivity_Helper
    {
        


        /// <summary>
        /// retrieves the connection profiles has been connected
        /// </summary>
        /// <returns></returns>
        public static string[] GetConnectionProfiles()
        {
            // Get all connection profiles
            string[] connectionProfileList;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                connectionProfileList = new string[(int)ConnectionProfiles.Count];

                int i = 0;
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    connectionProfileList[i] = connectionProfile.ProfileName;
                    i++;
                }

                return connectionProfileList;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected exception occurred: " + ex.ToString());

                return null;
            }
        }


        /// <summary>
        /// retrieve given connectin profile info
        /// </summary>
        /// <param name="connectionProfile"></param>
        /// <returns></returns>
        public static string GetConnectionProfile(ConnectionProfile connectionProfile)
        {
            string connectionProfileInfo = string.Empty;
            if (connectionProfile != null)
            {
                connectionProfileInfo = "Profile Name : " + connectionProfile.ProfileName + "\n";

                switch (connectionProfile.GetNetworkConnectivityLevel())
                {
                    case NetworkConnectivityLevel.None:
                        connectionProfileInfo += "Connectivity Level : None\n";
                        break;
                    case NetworkConnectivityLevel.LocalAccess:
                        connectionProfileInfo += "Connectivity Level : Local Access\n";
                        break;
                    case NetworkConnectivityLevel.ConstrainedInternetAccess:
                        connectionProfileInfo += "Connectivity Level : Constrained Internet Access\n";
                        break;
                    case NetworkConnectivityLevel.InternetAccess:
                        connectionProfileInfo += "Connectivity Level : Internet Access\n";
                        break;
                }

                

            }
            return connectionProfileInfo;
        }


    }
}
