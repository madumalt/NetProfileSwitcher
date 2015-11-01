using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Data.SQLite;
using System.Configuration;

namespace NetProfileSwitcher
{
    /// <summary>
    /// data container, use to store, pass network page details
    /// </summary>
    public class NetworkPageDetails
    {
        public string profileName = null;
        public string nicName = null;
        public int dhcp = -1;
        public int manual = -1;
        public string ipAddress = null;
        public string subnetMask = null;
        public string gateway = null;
        public string dnsServer = null;
    }

    /// <summary>
    /// data container, use to store. pass proxy page details
    /// </summary>
    public class ProxyPageDetails 
    { 
        public string profileName = null;
        public int enableProxy = -1;
        public int enableManual = -1;
        public int sameProxy = -1;
        public string http = null;
        public string https = null;
        public string ftp = null;
        public string socks = null;
        public int bypassProxy = -1;
        public int enableScript = -1;
        public string scriptURL = null;
        public int clearAtActivation = -1;
        public int autoDetect = -1;
        public int enableException = -1;
        public string exception = null;
    }

    /// <summary>
    /// enum of message related to database connection and transactions
    /// </summary>
    public enum DatabaseMessage{
        noProfileNameError,
        databaseError,
        proxyenableUnassignedError,
        successfulInsertion,
        insertionFailed,
        entryDoesNotExists,
        entryExists,
    };

    public class Database_Helper
    {
        /// <summary>
        /// Insert the given networkpage details in to the network Table in database
        /// </summary>
        /// <param name="npd">NetworkPageDetails instance containig network page details</param>
        /// <returns>DatabaseMessage</returns>
        public static DatabaseMessage Insert_to_networkTable(NetworkPageDetails npd) { 
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                    SQLiteCommand cmd = new SQLiteCommand();

                        cmd.Connection = conn;
                        cmd.CommandText = "INSERT INTO network(profileName, nicName, dhcp, manual, ipAddress, subnetMask, gateway, dnsServer) VALUES(@profileName, @nicName, @dhcp, @manual, @ipAddress, @subnetMask, @gateway, @dnsServer)";
                        cmd.Prepare();

                        if (string.IsNullOrEmpty(npd.profileName)) return DatabaseMessage.noProfileNameError;
                        else
                        {
                            cmd.Parameters.AddWithValue("@profileName", npd.profileName);

                            if (string.IsNullOrEmpty(npd.nicName)) cmd.Parameters.AddWithValue("@nicName", null);
                            else cmd.Parameters.AddWithValue("@nicName", npd.nicName);

                            if (npd.dhcp == -1) cmd.Parameters.AddWithValue("@dhcp", null);
                            else cmd.Parameters.AddWithValue("@dhcp", npd.dhcp == 1 ? true : false);

                            if (npd.manual == -1) cmd.Parameters.AddWithValue("@manual", null);
                            else cmd.Parameters.AddWithValue("@manual", npd.manual == 1 ? true : false);

                            if (string.IsNullOrEmpty(npd.ipAddress)) cmd.Parameters.AddWithValue("@ipAddress", null);
                            else cmd.Parameters.AddWithValue("@ipAddress", npd.ipAddress);

                            if (string.IsNullOrEmpty(npd.subnetMask)) cmd.Parameters.AddWithValue("@subnetMask", null);
                            else cmd.Parameters.AddWithValue("@subnetMask", npd.subnetMask);

                            if (string.IsNullOrEmpty(npd.gateway)) cmd.Parameters.AddWithValue("@gateway", null);
                            else cmd.Parameters.AddWithValue("@gateway", npd.gateway);

                            if (string.IsNullOrEmpty(npd.dnsServer)) cmd.Parameters.AddWithValue("@dnsServer", null);
                            else cmd.Parameters.AddWithValue("@dnsServer", npd.dnsServer);
                        }

                        cmd.ExecuteNonQuery(); //ExecuteNonQuery used since no return expected

                    if (conn != null)
                    {
                        conn.Close();
                    }
                    return DatabaseMessage.successfulInsertion;
                
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
                if (conn != null)
                {
                    conn.Close();
                }
                return DatabaseMessage.databaseError;
            }
            finally {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

        /// <summary>
        /// Insert the given proxypage details in to the proxy Table in database
        /// </summary>
        /// <param name="ppd">ProxyPageDetails instance containig proxy page details</param>
        /// <returns>DatabaseMessage</returns>
        public static DatabaseMessage Insert_to_proxyTable(ProxyPageDetails ppd){
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;

            try
            {
                conn = new SQLiteConnection(ConString);
                
                    conn.Open();

                    SQLiteCommand cmdPrf = new SQLiteCommand();
                    
                        cmdPrf.Connection = conn;
                        cmdPrf.CommandText = "INSERT INTO proxy(profileName, enableProxy, enableManual, sameProxy, http, https, ftp, socks, byPassProxyForLocal, enableScript, scriptURL, clearAtActivation, autoDetect, enableException, exception) VALUES (@profileName, @enableProxy, @enableManual, @sameProxy, @http, @https, @ftp, @socks, @byPassProxyForLocal, @enableScript, @scriptURL, @clearAtActivation, @autoDetect, @enableException, @exception)";
                        cmdPrf.Prepare();

                        if (string.IsNullOrEmpty(ppd.profileName)) return DatabaseMessage.noProfileNameError;
                        else
                        {
                            cmdPrf.Parameters.AddWithValue("@profileName", ppd.profileName);

                            if (ppd.enableProxy == -1) return DatabaseMessage.proxyenableUnassignedError;
                            else cmdPrf.Parameters.AddWithValue("@enableProxy", ppd.enableProxy == 1 ? true : false);

                            if (ppd.enableManual == -1) cmdPrf.Parameters.AddWithValue("@enableManual", null);
                            else cmdPrf.Parameters.AddWithValue("@enableManual", ppd.enableManual == 1 ? true : false);

                            if (ppd.sameProxy == -1) cmdPrf.Parameters.AddWithValue("@sameProxy", null);
                            else cmdPrf.Parameters.AddWithValue("@sameProxy", ppd.sameProxy == 1 ? true : false);

                            if (string.IsNullOrEmpty(ppd.http)) cmdPrf.Parameters.AddWithValue("@http", null);
                            else cmdPrf.Parameters.AddWithValue("@http", ppd.http);

                            if (string.IsNullOrEmpty(ppd.https)) cmdPrf.Parameters.AddWithValue("@https", null);
                            else cmdPrf.Parameters.AddWithValue("@https", ppd.https);

                            if (string.IsNullOrEmpty(ppd.ftp)) cmdPrf.Parameters.AddWithValue("@ftp", null);
                            else cmdPrf.Parameters.AddWithValue("@ftp", ppd.ftp);

                            if (string.IsNullOrEmpty(ppd.socks)) cmdPrf.Parameters.AddWithValue("@socks", null);
                            else cmdPrf.Parameters.AddWithValue("@socks", ppd.socks);

                            if (ppd.bypassProxy == -1) cmdPrf.Parameters.AddWithValue("@byPassProxyForLocal", null);
                            else cmdPrf.Parameters.AddWithValue("@byPassProxyForLocal", ppd.bypassProxy == 1 ? true : false);

                            if (ppd.enableScript == -1) cmdPrf.Parameters.AddWithValue("@enableScript", null);
                            else cmdPrf.Parameters.AddWithValue("@enableScript", ppd.enableScript == 1 ? true : false);

                            if (string.IsNullOrEmpty(ppd.scriptURL)) cmdPrf.Parameters.AddWithValue("@scriptURL", null);
                            else cmdPrf.Parameters.AddWithValue("@scriptURL", ppd.scriptURL);

                            if (ppd.clearAtActivation == -1) cmdPrf.Parameters.AddWithValue("@clearAtActivation", null);
                            else cmdPrf.Parameters.AddWithValue("@clearAtActivation", ppd.clearAtActivation == 1 ? true : false);

                            if (ppd.autoDetect == -1) cmdPrf.Parameters.AddWithValue("@autoDetect", null);
                            else cmdPrf.Parameters.AddWithValue("@autoDetect", ppd.autoDetect == 1 ? true : false);

                            if (ppd.enableException == -1) cmdPrf.Parameters.AddWithValue("@enableException", null);
                            else cmdPrf.Parameters.AddWithValue("@enableException", ppd.enableException == 1 ? true : false);

                            if (string.IsNullOrEmpty(ppd.exception)) cmdPrf.Parameters.AddWithValue("@exception", null);
                            else cmdPrf.Parameters.AddWithValue("@exception", ppd.exception);
                        }

                        cmdPrf.ExecuteNonQuery();
                    

                    if (conn != null)
                    {
                        conn.Close();
                    }
                    return DatabaseMessage.successfulInsertion;
                
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
                if (conn != null)
                {
                    conn.Close();
                }
                return DatabaseMessage.databaseError;
            }
            finally {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// retreives the first row from the network Table which contains the given 'profileName'
        /// </summary>
        /// <param name="profileName">profileName of which profile network page details desired</param>
        /// <returns>NetworkPageDetails instance containing retreived network page details</returns>
        public static NetworkPageDetails Read_networkTable(string profileName)
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;
            SQLiteDataReader rdr = null;

            NetworkPageDetails npd = new NetworkPageDetails();

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM network WHERE profileName = @profileName";
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@profileName", profileName);
                rdr = cmd.ExecuteReader();

                if (!rdr.HasRows) return new NetworkPageDetails();

                while (rdr.Read())
                {
                    npd.profileName = rdr.GetString(0);

                    if (rdr[1].Equals(DBNull.Value)) npd.nicName = null;
                    else npd.nicName = rdr.GetString(1);

                    if (rdr[2].Equals(DBNull.Value)) npd.dhcp = -1;
                    else npd.dhcp = rdr.GetBoolean(2) ? 1 : 0;

                    if (rdr[3].Equals(DBNull.Value)) npd.manual = -1;
                    else npd.manual = rdr.GetBoolean(3) ? 1 : 0;

                    if (rdr[4].Equals(DBNull.Value)) npd.ipAddress = null;
                    else npd.ipAddress = rdr.GetString(4);

                    if (rdr[5].Equals(DBNull.Value)) npd.subnetMask = null;
                    else npd.subnetMask = rdr.GetString(5);

                    if (rdr[6].Equals(DBNull.Value)) npd.gateway = null;
                    else npd.gateway = rdr.GetString(6);

                    if (rdr[7].Equals(DBNull.Value)) npd.dnsServer = null;
                    else npd.dnsServer = rdr.GetString(7);

                    break;
                }

                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }

                return npd;

            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }
                return new NetworkPageDetails();
            }
            finally {
                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }
            }
            
        }

        public static ArrayList getStoredProfiles() {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;
            SQLiteDataReader rdr = null;

            ArrayList profiles = null;

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand("SELECT profileName FROM network", conn);
                rdr = cmd.ExecuteReader();

                if (!rdr.HasRows) return null;

                profiles = new ArrayList();
                while (rdr.Read())
                {
                    profiles.Add((string)rdr.GetString(0));  ///did a change whilst converting to SQLite
                }

                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }

                return profiles;

            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }
                return null;
            }
            finally {
                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// retreives the first row from the proxy Table which contains the given 'profileName'
        /// </summary>
        /// <param name="profileName">profileName of which profile proxy page details desired</param>
        /// <returns>ProxyPageDetails instance containing retreived proxy page details</returns>
        public static ProxyPageDetails Readd_proxyTable(string profileName)
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;
            SQLiteDataReader rdr = null;

            ProxyPageDetails ppd = new ProxyPageDetails();

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand cmd = new SQLiteCommand();
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM proxy WHERE profileName = @profileName";
                cmd.Prepare();

                cmd.Parameters.AddWithValue("@profileName", profileName);
                rdr = cmd.ExecuteReader();

                if (!rdr.HasRows) return new ProxyPageDetails();

                while (rdr.Read())
                {
                    ppd.profileName = rdr.GetString(0);

                    ppd.enableProxy = rdr.GetBoolean(1) ? 1 : 0;

                    if (rdr[2].Equals(DBNull.Value)) ppd.enableManual = -1;
                    else ppd.enableManual = rdr.GetBoolean(2) ? 1 : 0;

                    if (rdr[3].Equals(DBNull.Value)) ppd.sameProxy = -1;
                    else ppd.sameProxy = rdr.GetBoolean(3) ? 1 : 0;

                    if (rdr[4].Equals(DBNull.Value)) ppd.http = null;
                    else ppd.http = rdr.GetString(4);

                    if (rdr[5].Equals(DBNull.Value)) ppd.https = null;
                    else ppd.https = rdr.GetString(5);

                    if (rdr[6].Equals(DBNull.Value)) ppd.ftp = null;
                    else ppd.ftp = rdr.GetString(6);

                    if (rdr[7].Equals(DBNull.Value)) ppd.socks = null;
                    else ppd.socks = rdr.GetString(7);

                    if (rdr[8].Equals(DBNull.Value)) ppd.bypassProxy = -1;
                    else ppd.bypassProxy = rdr.GetBoolean(8) ? 1 : 0;

                    if (rdr[9].Equals(DBNull.Value)) ppd.enableScript = -1;
                    else ppd.enableScript = rdr.GetBoolean(9) ? 1 : 0;

                    if (rdr[10].Equals(DBNull.Value)) ppd.scriptURL = null;
                    else ppd.scriptURL = rdr.GetString(10);

                    if (rdr[11].Equals(DBNull.Value)) ppd.clearAtActivation = -1;
                    else ppd.clearAtActivation = rdr.GetBoolean(11) ? 1 : 0;

                    if (rdr[12].Equals(DBNull.Value)) ppd.autoDetect = -1;
                    else ppd.autoDetect = rdr.GetBoolean(12) ? 1 : 0;

                    if (rdr[13].Equals(DBNull.Value)) ppd.enableException = -1;
                    else ppd.enableException = rdr.GetBoolean(13) ? 1 : 0;

                    if (rdr[14].Equals(DBNull.Value)) ppd.exception = null;
                    else ppd.exception = rdr.GetString(14);

                    break;
                }

                return ppd;


            }
            catch (SQLiteException ex)
            {
                Console.WriteLine("Error: {0}", ex.ToString());
                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }
                return new ProxyPageDetails();
            }
            finally {
                if (rdr != null)
                {
                    rdr.Close();
                }

                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static DatabaseMessage DeleteIfExist_NetworkPage(string profileName)
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand delCmd = new SQLiteCommand();
                delCmd.Connection = conn;
                delCmd.CommandText = "DELETE FROM network WHERE profileName = @profileName";
                delCmd.Prepare();
                delCmd.Parameters.AddWithValue("@profilename", profileName);
                int affectedRows = delCmd.ExecuteNonQuery();

                if (conn != null)
                {
                    conn.Close();
                }

                if (affectedRows > 0) return DatabaseMessage.entryExists;
                return DatabaseMessage.entryDoesNotExists;
            }
            catch (SQLiteException ex)
            {

                if (conn != null)
                {
                    conn.Close();
                }

                Console.WriteLine("Error: {0}", ex.ToString());
                return DatabaseMessage.databaseError;
            }
            finally
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static DatabaseMessage DeleteIfExist_ProxyPage(string profileName)
        {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;
            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand delCmd = new SQLiteCommand();
                delCmd.Connection = conn;
                delCmd.CommandText = "DELETE FROM proxy WHERE profileName = @profileName";
                delCmd.Prepare();
                delCmd.Parameters.AddWithValue("@profilename", profileName);
                int affectedRows = delCmd.ExecuteNonQuery();

                if (conn != null)
                {
                    conn.Close();
                }

                if (affectedRows > 0) return DatabaseMessage.entryExists;
                return DatabaseMessage.entryDoesNotExists;

            }
            catch (SQLiteException ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                Console.WriteLine("Error: {0}", ex.ToString());
                return DatabaseMessage.databaseError;
            }
            finally {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static DatabaseMessage saveRemarks(string profileName, string remark) {

            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand cmdPrf = new SQLiteCommand();
                cmdPrf.Connection = conn;
                cmdPrf.CommandText = "INSERT INTO remarks (profileName, remark)  VALUES (@profileName, @remark)";
                cmdPrf.Prepare();

                cmdPrf.Parameters.AddWithValue("@profileName", profileName);
                cmdPrf.Parameters.AddWithValue("@remark", remark);

                cmdPrf.ExecuteNonQuery();
                if (conn != null)
                {
                    conn.Close();
                }
                return DatabaseMessage.successfulInsertion;
            }
            catch (SQLiteException)
            {
                if (conn != null)
                {
                    conn.Close();
                }
                return DatabaseMessage.databaseError;
            }
            finally {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileName"></param>
        /// <param name="remark"></param>
        /// <returns></returns>
        public static DatabaseMessage updateRemark(string profileName, string remark) {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand cmdPrf = new SQLiteCommand();
                cmdPrf.Connection = conn;
                cmdPrf.CommandText = "UPDATE remarks SET remark = @remark WHERE profileName = @profileName";
                cmdPrf.Parameters.AddWithValue("@profileName", profileName);
                cmdPrf.Parameters.AddWithValue("@remark", remark);

                cmdPrf.ExecuteNonQuery();

                if (conn != null)
                {
                    conn.Close();
                }
                return DatabaseMessage.successfulInsertion;
            }
            catch (SQLiteException)
            {
                if (conn != null)
                {
                    conn.Close();
                }
                return DatabaseMessage.databaseError;
            }
            finally {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static string retrieveRemarks(string profileName) {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;
            SQLiteDataReader rdr = null;

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand cmdPrf = new SQLiteCommand();
                cmdPrf.Connection = conn;
                cmdPrf.CommandText = "SELECT remark FROM remarks WHERE profileName = @profileName";
                cmdPrf.Parameters.AddWithValue("@profileName", profileName);

                rdr = cmdPrf.ExecuteReader();

                string remark = null;
                while (rdr.Read())
                {
                    remark = rdr.GetString(0);
                    if (rdr != null)
                    {
                        rdr.Close();
                    }
                    if (conn != null)
                    {
                        conn.Close();
                    }

                    return remark;
                }

                if (rdr != null)
                {
                    rdr.Close();
                }
                if (conn != null)
                {
                    conn.Close();
                }
                return null;

            }
            catch (SQLiteException)
            {
                if (conn != null)
                {
                    conn.Close();
                }
                return null;
            }
            finally 
            {
                if (conn != null)
                {
                    conn.Close();
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="profileName"></param>
        /// <returns></returns>
        public static DatabaseMessage deleteRemarkIfExist(string profileName) {
            string ConString = ConfigurationManager.ConnectionStrings["ConString"].ConnectionString;
            SQLiteConnection conn = null;

            try
            {
                conn = new SQLiteConnection(ConString);
                conn.Open();

                SQLiteCommand delCmd = new SQLiteCommand();
                delCmd.Connection = conn;
                delCmd.CommandText = "DELETE FROM remarks WHERE profileName = @profileName";
                delCmd.Prepare();
                delCmd.Parameters.AddWithValue("@profilename", profileName);
                int affectedRows = delCmd.ExecuteNonQuery();

                if (conn != null)
                {
                    conn.Close();
                }

                if (affectedRows > 0) return DatabaseMessage.entryExists;
                return DatabaseMessage.entryDoesNotExists;
            }
            catch (SQLiteException ex)
            {
                if (conn != null)
                {
                    conn.Close();
                }

                Console.WriteLine("Error: {0}", ex.ToString());
                return DatabaseMessage.databaseError;
            }
            finally {
                if (conn != null)
                {
                    conn.Close();
                }
            }
        
        }

    }
}
