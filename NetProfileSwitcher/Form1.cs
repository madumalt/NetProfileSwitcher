using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Windows.Networking.Connectivity;
using System.Management;
using System.Collections;

using System.Net;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace NetProfileSwitcher
{
    public sealed partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            this.MaximizeBox = false;

            #region adding connection profiles
            //getting connection profiles stored in the database
            ArrayList profiles = Database_Helper.getStoredProfiles();
            //adding profile elements to profileSelection_comboBox
            if (profiles == null) { ProfileSelection_ComboBox.Items.Clear(); }
            else
            {
                ProfileSelection_ComboBox.Items.Clear();
                for (int i = 0; i < profiles.Count; i++)
                {
                    ProfileSelection_ComboBox.Items.Add(profiles[i]);
                }
            }

            //getting connection profiles available or previously connected
            string[] prfs = Windows_Networking_Connectivity_Helper.GetConnectionProfiles();

            //appending profile elements to profileSelection_comboBox
            for (int i = 0; i < prfs.Length; i++)
            {
                if(!ProfileSelection_ComboBox.Items.Contains(prfs[i]))
                    ProfileSelection_ComboBox.Items.Add(prfs[i]);
            }
            #endregion

            #region adding available networkCards to NetworkCards_check
            ArrayList Ncards = Win32_NetAdapConfig_Help.getNetworkCards();
            NetworkCard_comboBox.Items.Clear();
            for (int i = 0; i < Ncards.Count; i++ ) NetworkCard_comboBox.Items.Add((string)Ncards[i]);
            NetworkCard_comboBox.Items.Add("");
            #endregion

            #region disabling network page features until a network card is selected: phase_1
                enableDHCP_radioButton.Enabled = false;
                enableManualIP_radioButton.Enabled = false;
            #endregion
            
            Form1.CheckForIllegalCrossThreadCalls = false; 
            NetworkChange.NetworkAvailabilityChanged += new NetworkAvailabilityChangedEventHandler(NetworkChange_NetworkAvailabilityChanged);
    
        }

        #region atNetWorkChange Event Handling
        private void atNetworkChange(string profileName, string accesssType) {
            if(string.IsNullOrEmpty(profileName)){
                MessageBox.Show("Network status changed : Not connected to a network");
                return;
            }
            else if (ProfileSelection_ComboBox.Items.Contains(profileName))
            {
                ProfileSelection_ComboBox.Text = profileName;
                this.Show();
                MessageBox.Show("Network Status Change Detected");
                return;
            }
            else
            {
                ProfileSelection_ComboBox.Items.Add(profileName);
                ProfileSelection_ComboBox.Text = profileName;
                this.Show();
                MessageBox.Show("Network Status Change Detected");
                return;
            } 
        }

        private void NetworkChange_NetworkAvailabilityChanged(object sender, NetworkAvailabilityEventArgs e)
        {
            Console.WriteLine("Network connection change detected!!!!!");
            try
            {
                // Get all connection profiles
                string connectionProfileList = string.Empty;
                try
                {
                    if (e.IsAvailable)
                    {
                        var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                        foreach (var connectionProfile in ConnectionProfiles)
                        {
                            switch (connectionProfile.GetNetworkConnectivityLevel())
                            {
                                case NetworkConnectivityLevel.None : continue;
                                case NetworkConnectivityLevel.LocalAccess:
                                    {
                                        Console.WriteLine(connectionProfile.ProfileName + " : local access");
                                        atNetworkChange(connectionProfile.ProfileName , "local");
                                        return;
                                    }
                                case NetworkConnectivityLevel.ConstrainedInternetAccess:
                                    {
                                        Console.WriteLine(connectionProfile.ProfileName + " : ConstrainedInternetAccess");
                                        atNetworkChange(connectionProfile.ProfileName, "ConstrainedInternet");
                                        return;
                                    }
                                case NetworkConnectivityLevel.InternetAccess:
                                    {
                                        Console.WriteLine(connectionProfile.ProfileName + " : InternetAccess");
                                        atNetworkChange(connectionProfile.ProfileName, "Internet");
                                        return;
                                    }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not Connected to a network");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Unexpected exception occurred: " + ex.ToString());
                }
            }
            catch (NetworkInformationException ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, ex.GetType().ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
            }
        }
        #endregion

        # region form event handling
        private void Form1_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProfileSelection_ComboBox.Text)) {
                AddProfile_Button.Enabled = false;
                RemoveProfile_Button.Enabled = false;
            }
        }

        #region Profile Selection ComboBox EventHandling
        private void ProfileSelection_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProfileSelection_ComboBox.Text))
            {
                TabMenu.Enabled = false;
                AddProfile_Button.Enabled = false;
                RemoveProfile_Button.Enabled = false;
            }
            else
            {
                TabMenu.Enabled = true;
                AddProfile_Button.Enabled = true;
                RemoveProfile_Button.Enabled = true;
            }

            //getting connection profiles stored in the database
            ArrayList profiles = Database_Helper.getStoredProfiles();
            
            //adding newly stored custom typed profiles
            if (profiles != null)
            {
                for (int i = 0; i < profiles.Count; i++)
                {
                    if (!ProfileSelection_ComboBox.Items.Contains(profiles[i]))
                        ProfileSelection_ComboBox.Items.Add(profiles[i]);
                }
            }
            
            


            if (profiles == null)
            {
                RemoveProfile_Button.Enabled = false;
                # region pages reseting
                //networkpage reset
                enableDHCP_radioButton.Checked = false;
                enableManualIP_radioButton.Checked = false;
                IPAddress_textBox.Text = null;
                subnetMask_textBox.Text = null;
                gateway_textBox.Text = null;
                DNSSever_textBox.Text = null;

                //proxypage reset
                enableProxy_checkBox.Checked = false;
                manualProxy_radioButton.Checked = false;
                HTTP_textBox.Text = null; HTTPport_textBox.Text = null;
                secure_textBox.Text = null; securePort_textBox.Text = null;
                FTP_textBox.Text = null; FTPport_textBox.Text = null;
                socks_textBox.Text = null; socksPort_textBox.Text = null;
                sameProxyForAllProtocols_checkBox.Checked = false;
                byPassProxyForLocalAddrss_checkBox.Checked = false;
                automaticProxyScript_radioButton.Checked = false;
                clearProxyAtActivation_radioButton.Checked = false;
                autoProxyDetection_checkBox.Checked = false;
                enableProxyExceptions_checkBox.Checked = false;

                remarks_textBox.Text = string.Empty;
                #endregion
            }
            else if (profiles.Contains(ProfileSelection_ComboBox.Text))
            {
                RemoveProfile_Button.Enabled = true;

                //retreiving and assigning profile details
                NetworkPageDetails npd = Database_Helper.Read_networkTable(ProfileSelection_ComboBox.Text);
                ProxyPageDetails ppd = Database_Helper.Readd_proxyTable(ProfileSelection_ComboBox.Text);

                if (npd.profileName != null && npd.profileName.Equals(ppd.profileName))
                {
                    #region loadNetworkPage
                    switch (npd.dhcp)
                    {
                        case -1:
                        case 0: enableDHCP_radioButton.Checked = false; break;
                        case 1: enableDHCP_radioButton.Checked = true; break;
                    }
                    switch (npd.manual)
                    {
                        case -1:
                        case 0: enableManualIP_radioButton.Checked = false; break;
                        case 1: enableManualIP_radioButton.Checked = true; break;
                    }
                    IPAddress_textBox.Text = npd.ipAddress;
                    subnetMask_textBox.Text = npd.subnetMask;
                    gateway_textBox.Text = npd.gateway;
                    DNSSever_textBox.Text = npd.dnsServer;
                    #endregion

                    #region loadProxyPage
                    switch (ppd.enableProxy)
                    {
                        case -1:
                        case 0: enableProxy_checkBox.Checked = false; break;
                        case 1: enableProxy_checkBox.Checked = true; break;
                    }
                    switch (ppd.enableManual)
                    {
                        case -1:
                        case 0:
                            manualProxy_radioButton.Checked = false;
                            break;
                        case 1:
                            manualProxy_radioButton.Checked = true;
                            break;
                    }

                    switch (ppd.sameProxy)
                    {
                        case -1:
                        case 0:
                            {
                                sameProxyForAllProtocols_checkBox.Checked = false;

                                if (manualProxy_radioButton.Checked)
                                {
                                    if (ppd.http != null)
                                    {
                                        string[] tempHttp = ppd.http.Split(':');
                                        if (tempHttp.Length == 2)
                                        {
                                            HTTP_textBox.Text = tempHttp[0];
                                            HTTPport_textBox.Text = tempHttp[1];
                                        }
                                        else
                                        {
                                            HTTP_textBox.Text = ppd.http;
                                            HTTPport_textBox = null;
                                        }
                                    }
                                    else { HTTP_textBox.Text = null; HTTPport_textBox.Text = null; }

                                    if (ppd.https != null)
                                    {
                                        string[] tempHttps = ppd.https.Split(':');
                                        if (tempHttps.Length == 2)
                                        {
                                            secure_textBox.Text = tempHttps[0];
                                            securePort_textBox.Text = tempHttps[1];
                                        }
                                        else
                                        {
                                            secure_textBox.Text = ppd.http;
                                            securePort_textBox.Text = null;
                                        }
                                    }
                                    else { secure_textBox.Text = null; securePort_textBox.Text = null; }

                                    if (ppd.ftp != null)
                                    {
                                        string[] tempFtp = ppd.ftp.Split(':');
                                        if (tempFtp.Length == 2)
                                        {
                                            FTP_textBox.Text = tempFtp[0];
                                            FTPport_textBox.Text = tempFtp[1];
                                        }
                                        else
                                        {
                                            FTP_textBox.Text = ppd.ftp;
                                            FTPport_textBox.Text = null;
                                        }
                                    }
                                    else { FTP_textBox.Text = null; FTPport_textBox.Text = null; }

                                    if (ppd.socks != null)
                                    {
                                        string[] tempSocks = ppd.socks.Split(':');
                                        if (tempSocks.Length == 2)
                                        {
                                            socks_textBox.Text = tempSocks[0];
                                            socksPort_textBox.Text = tempSocks[1];
                                        }
                                        else
                                        {
                                            socks_textBox.Text = ppd.socks;
                                            socksPort_textBox.Text = null;
                                        }
                                    }
                                    else { socks_textBox.Text = null; socksPort_textBox.Text = null; }
                                }
                                else
                                {
                                    HTTP_textBox.Text = HTTPport_textBox.Text = null;
                                    secure_textBox.Text = securePort_textBox.Text = null;
                                    FTP_textBox.Text = FTPport_textBox.Text = null;
                                    socks_textBox.Text = socksPort_textBox.Text = null;
                                }
                                break;
                            }
                        case 1:
                            {
                                sameProxyForAllProtocols_checkBox.Checked = true;
                                if (manualProxy_radioButton.Checked)
                                {
                                    if (ppd.http == null) { HTTP_textBox.Text = HTTPport_textBox.Text = null; }
                                    else
                                    {
                                        string[] temp = ppd.http.Split(':');
                                        if (temp.Length == 2)
                                        {
                                            HTTP_textBox.Text = temp[0];
                                            HTTPport_textBox.Text = temp[1];
                                        }
                                        else
                                        {
                                            HTTP_textBox.Text = ppd.http;
                                            HTTPport_textBox = null;
                                        }
                                    }
                                }
                                else sameProxyForAllProtocols_checkBox.Checked = false;
                                break;
                            }
                    }

                    switch (ppd.bypassProxy)
                    {
                        case -1:
                        case 0: byPassProxyForLocalAddrss_checkBox.Checked = false; break;
                        case 1: byPassProxyForLocalAddrss_checkBox.Checked = true; break;
                    }

                    switch (ppd.enableScript)
                    {
                        case -1:
                        case 0:
                            automaticProxyScript_radioButton.Checked = false;
                            break;
                        case 1:
                            automaticProxyScript_radioButton.Checked = true;
                            break;
                    }

                    scriptURL_textBox.Text = ppd.scriptURL;

                    switch (ppd.clearAtActivation)
                    {
                        case -1:
                        case 0:
                            clearProxyAtActivation_radioButton.Checked = false;
                            break;
                        case 1:
                            clearProxyAtActivation_radioButton.Checked = true;
                            break;
                    }

                    switch (ppd.autoDetect)
                    {
                        case -1:
                        case 0: autoProxyDetection_checkBox.Checked = false; break;
                        case 1: autoProxyDetection_checkBox.Checked = true; break;
                    }

                    switch (ppd.enableException)
                    {
                        case -1:
                        case 0: enableProxyExceptions_checkBox.Checked = false; break;
                        case 1: enableProxyExceptions_checkBox.Checked = true; break;
                    }
                    proxyExceptions_textBox.Text = ppd.exception;
                    #endregion

                    #region loadRemarkPage
                    string remark = Database_Helper.retrieveRemarks(npd.profileName);
                    if (!string.IsNullOrEmpty(remark))
                        remarks_textBox.Text = remark;
                    else
                        remarks_textBox.Text = string.Empty;
                    #endregion
                }



            }
            else
            {
                RemoveProfile_Button.Enabled = false;

                #region pages resetting
                //networkpage reset
                enableDHCP_radioButton.Checked = false;
                enableManualIP_radioButton.Checked = false;
                IPAddress_textBox.Text = null;
                subnetMask_textBox.Text = null;
                gateway_textBox.Text = null;
                DNSSever_textBox.Text = null;

                //proxypage reset
                enableProxy_checkBox.Checked = false;
                manualProxy_radioButton.Checked = false;
                HTTP_textBox.Text = null; HTTPport_textBox.Text = null;
                secure_textBox.Text = null; securePort_textBox.Text = null;
                FTP_textBox.Text = null; FTPport_textBox.Text = null;
                socks_textBox.Text = null; socksPort_textBox.Text = null;
                sameProxyForAllProtocols_checkBox.Checked = false;
                byPassProxyForLocalAddrss_checkBox.Checked = false;
                automaticProxyScript_radioButton.Checked = false;
                clearProxyAtActivation_radioButton.Checked = false;
                autoProxyDetection_checkBox.Checked = false;
                enableProxyExceptions_checkBox.Checked = false;

                remarks_textBox.Text = string.Empty;
                #endregion
            }
        }

        private void ProfileSelection_ComboBox_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ProfileSelection_ComboBox.Text))
            {
                TabMenu.Enabled = false;
            }
            else
            {
                TabMenu.Enabled = true;
            }
        }
        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                atSystemTray_notifyIcon.Visible = true;
                this.Hide();
                e.Cancel = true;
            }

        }

        private void atSystemTray_notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void showProToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private void hideToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("Are you sure, you want to quit Net Prfile Switcher?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (dr == DialogResult.Yes)
            {
                this.Dispose(true);
            }
        }
        #endregion

        #region networkPage_EventHandling
        private void enableDHCP_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(NetworkCard_comboBox.Text)){
                manualIPConfiguration_groupBox.Enabled = false;
            }
            else{
                if (enableDHCP_radioButton.Checked)
                {
                    manualIPConfiguration_groupBox.Enabled = false;
                }
                else
                {
                    manualIPConfiguration_groupBox.Enabled = true;
                }
            }
        }

        private void enableManualIP_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NetworkCard_comboBox.Text))
            {
                manualIPConfiguration_groupBox.Enabled = false;
            }
            else
            {
                if (enableManualIP_radioButton.Checked)
                {
                    manualIPConfiguration_groupBox.Enabled = true;
                }
                else
                {
                    manualIPConfiguration_groupBox.Enabled = false;
                }
            }
        }

        private void NetworkCard_comboBox_DropDown(object sender, EventArgs e)
        {
            // adding available networkCards to NetworkCards_checkBox
            ArrayList Ncards = Win32_NetAdapConfig_Help.getNetworkCards();

            NetworkCard_comboBox.Items.Clear();
            for (int i = 0; i < Ncards.Count; i++) NetworkCard_comboBox.Items.Add((string)Ncards[i]);
            NetworkCard_comboBox.Items.Add("");
        }

        /// <summary>
        /// disable networkPage feature if a NIC is not selected
        /// with the selection of a NIC shows Current IPAddressing
        /// </summary>
        private void NetworkCard_comboBox_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(NetworkCard_comboBox.Text))
            {
                enableDHCP_radioButton.Enabled = false;
                enableManualIP_radioButton.Enabled = false;

                //clearing out               
                CurrentIP_label.Text = string.Empty;
                CurrentSubnet_label.Text = string.Empty;
                CurrentGateway_label.Text = string.Empty;
                CurrentDNS_label.Text = string.Empty;
            }
            else{
                enableDHCP_radioButton.Enabled = true;
                enableManualIP_radioButton.Enabled = true;

                ///<summary>
                ///retrieves Current IP addressng and display them
                ///</summary>
                /// <param name="ipAddress">Array of IP</param>
                /// <param name="subnets">Array of subnet masks</param>
                /// <param name="gateways">Array of gateways</param>
                /// <param name="dnses">Array of DNS IP</param>
                string[] ipAddress = null;
                string[] subnets = null;
                string[] gateways = null;
                string[] dnses = null;

                string nicName = (string)NetworkCard_comboBox.Text;
                Win32_NetAdapConfig_Help.GetIP(nicName, out ipAddress, out subnets, out gateways, out dnses);

                if (ipAddress != null)
                {
                    CurrentIP_label.Text = string.Join(" ; ", ipAddress);
                }
                else { CurrentIP_label.Text = "IP Not Enabled"; }

                if (subnets != null)
                {
                    CurrentSubnet_label.Text = string.Join(" ; ", subnets);
                }
                else { CurrentSubnet_label.Text= "";}

                if (gateways != null)
                {
                    CurrentGateway_label.Text = string.Join(" ; ", gateways);
                }
                else { CurrentGateway_label.Text = "";}

                if (dnses != null)
                {
                    CurrentDNS_label.Text = string.Join(" ; ", dnses);
                }
                else { CurrentDNS_label.Text = "";}
            }
        }
        #endregion

        #region ProxyPage Event Handling

        #region Proxy_settings_selection
        private void manualProxy_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (manualProxy_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = true;
                automaticProxyScript_panel.Enabled = false;
            }
            else if (automaticProxyScript_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = false;
                automaticProxyScript_panel.Enabled = true;
            }
            else if (clearProxyAtActivation_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = false;
                automaticProxyScript_panel.Enabled = false;
            }
        }

        private void automaticProxyScript_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (manualProxy_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = true;
                automaticProxyScript_panel.Enabled = false;
            }
            else if (automaticProxyScript_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = false;
                automaticProxyScript_panel.Enabled = true;
            }
            else if (clearProxyAtActivation_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = false;
                automaticProxyScript_panel.Enabled = false;
            }
        }

        private void clearProxyAtActivation_radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (manualProxy_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = true;
                automaticProxyScript_panel.Enabled = false;
            }
            else if (automaticProxyScript_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = false;
                automaticProxyScript_panel.Enabled = true;
            }
            else if (clearProxyAtActivation_radioButton.Checked)
            {
                manualProxySettings_panel.Enabled = false;
                automaticProxyScript_panel.Enabled = false;
            }
        }
        #endregion

        private void enableProxy_checkBox_CheckStateChanged(object sender, EventArgs e)
        {
            if (enableProxy_checkBox.Checked)
            {
                proxySettings_groupBox.Enabled = true;
                enableProxyExceptions_checkBox.Enabled = true;
            }
            else {
                proxySettings_groupBox.Enabled = false;
                enableProxyExceptions_checkBox.Enabled = false;
            }
        }

        private void enableProxyExceptions_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (enableProxyExceptions_checkBox.Checked)
            {
                proxyExceptions_groupBox.Enabled = true;
            }
            else {
                proxyExceptions_groupBox.Enabled = false;
            }
        }

        private void sameProxyForAllProtocols_checkBox_CheckedChanged(object sender, EventArgs e)
        {
            if (sameProxyForAllProtocols_checkBox.Checked)
            {
                sameProxyForAllProtocols_disablepanel.Enabled = false;
            }
            else {
                sameProxyForAllProtocols_disablepanel.Enabled = true;
            }
        }
        #endregion

        #region Activation Button Click

        private void NetworkPage_ActivateButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NetworkCard_comboBox.Text)) return;

            if (enableDHCP_radioButton.Checked)
            {
                //enable automatic IP obtaining using DHCP
                int rtn = Win32_NetAdapConfig_Help.SetDHCP((string)NetworkCard_comboBox.Text);
                Win32_NetAdapConfig_Help.showMessage(rtn);
                return;
            }

            if (enableManualIP_radioButton.Checked)
            {
                //set manual IPAddressing
                string NIC = NetworkCard_comboBox.Text;
                string IP = IPAddress_textBox.Text;
                string subnet = subnetMask_textBox.Text;
                string gateway = gateway_textBox.Text;
                string DNS = DNSSever_textBox.Text;

                int[] rtn = Win32_NetAdapConfig_Help.SetIP(NIC, IP, subnet, gateway, DNS);
                Win32_NetAdapConfig_Help.showMessage(rtn[0]);
                return;
            }

        }

        private void ProxyPage_ActivateButton_Click(object sender, EventArgs e) {

            //local variable
            bool autoDetect = false;
            bool bypassLocal = false;
            bool sameProxy = false;
            string proxy = string.Empty;
            string http = string.Empty;
            string secure = string.Empty;
            string ftp = string.Empty;
            string socks = string.Empty;
            string exception = string.Empty;
            ProxySetting_Help ph = new ProxySetting_Help();


            if (!enableProxy_checkBox.Checked) {
                bool done = ph.disableProxy(); 
                if (done) MessageBox.Show("Succesfully applied the settings.!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else MessageBox.Show("Proxy disabling failed..!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (!autoProxyDetection_checkBox.Checked) autoDetect = false;
                else autoDetect = true;

                if (sameProxyForAllProtocols_checkBox.Checked)
                {
                    sameProxy = true;
                    if (string.IsNullOrEmpty(HTTP_textBox.Text)) { 
                        proxy = string.Empty;
                        MessageBox.Show("proxy server left blank!!", "Warning!!");
                        return;
                    }
                    http = string.Concat(HTTP_textBox.Text, ":", HTTPport_textBox.Text);
                    proxy = http;
                }
                else
                {
                    sameProxy = false;

                    //getting manually entered proxy server values
                    if (string.IsNullOrEmpty(HTTP_textBox.Text)) http = string.Empty;
                    else http = string.Concat("http=",HTTP_textBox.Text, ":", HTTPport_textBox.Text);

                    if (string.IsNullOrEmpty(secure_textBox.Text)) secure = string.Empty;
                    else secure = string.Concat("https=", secure_textBox.Text, ":", securePort_textBox.Text);

                    if (string.IsNullOrEmpty(FTP_textBox.Text)) ftp = string.Empty;
                    else ftp = string.Concat("ftp=", FTP_textBox.Text, ":", FTPport_textBox.Text);

                    if (string.IsNullOrEmpty(socks_textBox.Text)) socks = string.Empty;
                    else socks = string.Concat("socks=", socks_textBox.Text, ":", socksPort_textBox.Text);

                    #region assigning the correct Proxy
                    if (string.IsNullOrEmpty(http))
                    {
                        if (string.IsNullOrEmpty(secure))
                        {
                            if (string.IsNullOrEmpty(ftp))
                            {
                                if (string.IsNullOrWhiteSpace(socks))
                                {
                                    proxy = string.Empty;
                                    MessageBox.Show("proxy server left blank!!", "Warning!!");
                                    return;
                                }
                                else
                                {
                                    proxy = socks;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(socks))
                                {
                                    proxy = ftp;
                                }
                                else
                                {
                                    proxy = ftp + ";" + socks;
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(ftp))
                            {
                                if (string.IsNullOrWhiteSpace(socks))
                                {
                                    proxy = secure;
                                }
                                else
                                {
                                    proxy = secure + ";" + socks;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(socks))
                                {
                                    proxy = secure + ";" + ftp;
                                }
                                else
                                {
                                    proxy = secure + ";" + ftp + ";" + socks;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(secure))
                        {
                            if (string.IsNullOrEmpty(ftp))
                            {
                                if (string.IsNullOrWhiteSpace(socks))
                                {
                                    proxy = http;
                                }
                                else
                                {
                                    proxy = http + ";" + socks;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(socks))
                                {
                                    proxy = http + ";" + ftp;
                                }
                                else
                                {
                                    proxy = http + ";" + ftp + ";" + socks;
                                }
                            }
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(ftp))
                            {
                                if (string.IsNullOrWhiteSpace(socks))
                                {
                                    proxy = http + ";" + secure;
                                }
                                else
                                {
                                    proxy = http + ";" + secure + ";" + socks;
                                }
                            }
                            else
                            {
                                if (string.IsNullOrWhiteSpace(socks))
                                {
                                    proxy = http + ";" + secure + ";" + ftp;
                                }
                                else
                                {
                                    proxy = http + ";" + secure + ";" + ftp + ";" + socks;
                                }
                            }
                        }
                    }
                    #endregion

                }
  

                //asignment of bypass proxy requirement
                if (byPassProxyForLocalAddrss_checkBox.Checked) bypassLocal = true;
                else bypassLocal = false;

                //assignment of proxy exception requirement
                if (enableProxyExceptions_checkBox.Checked)
                {
                    exception = proxyExceptions_textBox.Text;
                }
                else
                {
                    exception = string.Empty;
                }

                #region manual proxy setting
                if (manualProxy_radioButton.Checked)
                {
                    bool done = ph.setManualProxy(sameProxy, proxy, bypassLocal, exception, autoDetect);
                    if (done) MessageBox.Show("Succesfully applied the settings.!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("Invalid  manual proxy settings..!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

                #region proxy configuration script
                else if (automaticProxyScript_radioButton.Checked)
                {
                    bool done = ph.setProxyScript(scriptURL_textBox.Text, bypassLocal, exception, autoDetect);
                    if (done) MessageBox.Show("Succesfully applied the settings.!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("Poxy script url changing failed..!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

                #region clear proxy at activation
                else if (clearProxyAtActivation_radioButton.Checked)
                {

                }
                #endregion

                #region only autodetect
                else if (autoProxyDetection_checkBox.Checked)
                {
                    bool done = ph.automaticProxyDetection(autoDetect);
                    if (done) MessageBox.Show("Succesfully applied the settings.!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("Automatic proxy detect enabling failed..!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                #endregion

            }
        }

        private void RemarksPage_SaveButton_Click(object sender, EventArgs e)
        {
            string remark = string.Empty;
            if (!string.IsNullOrEmpty(remarks_textBox.Text)) { remark = remarks_textBox.Text; }

            Database_Helper.deleteRemarkIfExist(ProfileSelection_ComboBox.Text);
            Database_Helper.saveRemarks(ProfileSelection_ComboBox.Text, remark);

            MessageBox.Show("successfully added..!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        #endregion

       
        private void AddProfile_Button_Click(object sender, EventArgs e)
        {
            #region collec Network page details
            NetworkPageDetails npd = new NetworkPageDetails();
            npd.profileName =ProfileSelection_ComboBox.Text;

            if (enableDHCP_radioButton.Checked)
            {
                npd.dhcp = 1;
                npd.manual = 0;
            }

            else if (enableManualIP_radioButton.Checked)
            {
                npd.dhcp = 0;
                npd.manual = 1;
                //set manual IPAddressing
                npd.nicName = NetworkCard_comboBox.Text;
                npd.ipAddress = IPAddress_textBox.Text;
                npd.subnetMask = subnetMask_textBox.Text;
                npd.gateway = gateway_textBox.Text;
                npd.dnsServer = DNSSever_textBox.Text;
            }
            #endregion

            #region collect Proxy page Details
            ProxyPageDetails ppd = new ProxyPageDetails();
            ppd.profileName = ProfileSelection_ComboBox.Text;
            ppd.enableProxy = enableProxy_checkBox.Checked ? 1 : 0;
            ppd.enableManual = manualProxy_radioButton.Checked ? 1 : 0;

            if (!(string.IsNullOrEmpty(HTTP_textBox.Text) || string.IsNullOrEmpty(HTTPport_textBox.Text))) 
                ppd.http = HTTP_textBox.Text + ":" + HTTPport_textBox.Text;
            if (!(string.IsNullOrEmpty(secure_textBox.Text) || string.IsNullOrEmpty(securePort_textBox.Text)))
                ppd.https = secure_textBox.Text + ":" + securePort_textBox.Text;
            if (!(string.IsNullOrEmpty(FTP_textBox.Text) || string.IsNullOrEmpty(FTPport_textBox.Text)))
                ppd.ftp = FTP_textBox.Text + ":" + FTPport_textBox.Text;
            if (!(string.IsNullOrEmpty(socks_textBox.Text) || string.IsNullOrEmpty(socksPort_textBox.Text)))
                ppd.socks = socks_textBox.Text + ":" + socksPort_textBox.Text;

            ppd.sameProxy = sameProxyForAllProtocols_checkBox.Checked ? 1 : 0;
            ppd.bypassProxy = byPassProxyForLocalAddrss_checkBox.Checked ? 1 : 0;
            ppd.enableScript = automaticProxyScript_radioButton.Checked ? 1 : 0;
            if (!string.IsNullOrEmpty(scriptURL_textBox.Text)) ppd.scriptURL = scriptURL_textBox.Text;

            ppd.clearAtActivation = clearProxyAtActivation_radioButton.Checked ? 1 : 0;
            ppd.autoDetect = autoProxyDetection_checkBox.Checked ? 1 : 0;
            ppd.enableException = enableProxyExceptions_checkBox.Checked ? 1 : 0;
            if (!string.IsNullOrEmpty(proxyExceptions_textBox.Text)) ppd.exception = proxyExceptions_textBox.Text;
            #endregion

            #region remarks
            string remark = string.Empty;
            if (!string.IsNullOrEmpty(remarks_textBox.Text)) { remark = remarks_textBox.Text; }
            #endregion

            if (string.IsNullOrEmpty(npd.profileName) || string.IsNullOrEmpty(ppd.profileName) || !ppd.profileName.Equals(npd.profileName))
            {
                MessageBox.Show("profile name null or empty..!!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (RemoveProfile_Button.Enabled) {

                DialogResult dialogResult = MessageBox.Show("are you sure, you want to change existing settings?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Warning); 
                if (dialogResult == DialogResult.No)
                {
                    return;
                }
                
            }

            Database_Helper.DeleteIfExist_NetworkPage(npd.profileName);
            Database_Helper.DeleteIfExist_ProxyPage(ppd.profileName);
            
            Database_Helper.Insert_to_networkTable(npd);
            Database_Helper.Insert_to_proxyTable(ppd);

            Database_Helper.deleteRemarkIfExist(ProfileSelection_ComboBox.Text);
            Database_Helper.saveRemarks(ProfileSelection_ComboBox.Text, remark);

            MessageBox.Show("successfully added..!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RemoveProfile_Button_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ProfileSelection_ComboBox.Text))
            {
                Database_Helper.DeleteIfExist_NetworkPage(ProfileSelection_ComboBox.Text);
                Database_Helper.DeleteIfExist_ProxyPage(ProfileSelection_ComboBox.Text);
                Database_Helper.deleteRemarkIfExist(ProfileSelection_ComboBox.Text);
                MessageBox.Show("successfully removed..!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else {
                MessageBox.Show("Removing failed or No profile selected..!!", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        
        private void minimizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

       
    }
}
