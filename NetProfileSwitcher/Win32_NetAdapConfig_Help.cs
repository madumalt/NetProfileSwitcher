using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Management;
using System.Collections;
using System.Diagnostics;

using System.Net;

namespace NetProfileSwitcher
{
    public class Win32_NetAdapConfig_Help
    {

        #region NIC settings changing region
        ///<summary>
        ///returns a ArrayList of availabel(installed/connected) network cards 
        /// </summary>
        public static ArrayList getNetworkCards()
        {
            ArrayList Ncards = new ArrayList();

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                Ncards.Add(mo["Caption"]);

                /*//checking whether the device is IP Enabled device
                if ((bool)mo["IPEnabled"])
                {
                    //adds the network card name to the networkcard array list
                    Ncards.Add(mo["Caption"]);
                }*/
            }

            return Ncards;
        }

        /// <summary>
        /// provide the network card configuration of the specified Network Interface Card (NIC)
        /// </summary>
        /// <param name="nicName">Name of the NIC</param>
        /// <param name="ipAdresses">Array of IP</param>
        /// <param name="subnets">Array of subnet masks</param>
        /// <param name="gateways">Array of gateways</param>
        /// <param name="dnses">Array of DNS IP</param>
        public static void GetIP(string nicName, out string[] ipAdresses, out string[] subnets, out string[] gateways, out string[] dnses)
        {
            ipAdresses = null;
            subnets = null;
            gateways = null;
            dnses = null;

            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (mo["Caption"].Equals(nicName))
                {
                    ipAdresses = (string[])mo["IPAddress"];
                    subnets = (string[])mo["IPSubnet"];
                    gateways = (string[])mo["DefaultIPGateway"];
                    dnses = (string[])mo["DNSServerSearchOrder"];

                    break;
                }

               
               /* // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                if ((bool)mo["IPEnabled"])
                {
                    if (mo["Caption"].Equals(nicName))
                    {
                        ipAdresses = (string[])mo["IPAddress"];
                        subnets = (string[])mo["IPSubnet"];
                        gateways = (string[])mo["DefaultIPGateway"];
                        dnses = (string[])mo["DNSServerSearchOrder"];

                        break;
                    }
                }*/
                
            }
        }

        /// <summary>
        /// Set IP for the specified network card name
        /// </summary>
        /// <param name="nicName">Caption of the network card</param>
        /// <param name="IpAddresses">Comma delimited string containing one or more IP</param>
        /// <param name="SubnetMask">Subnet mask</param>
        /// <param name="Gateway">Gateway IP</param>
        /// <param name="DnsSearchOrder">Comma delimited DNS IP</param>
        /// <returns>int[3],[0]-setIp return, [1]-setGateway return, [2]-setDNSsearchOrder return</returns>
        public static int[] SetIP(string nicName, string IpAddresses, string SubnetMask, string Gateway, string DnsSearchOrder)
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (mo["Caption"].Equals(nicName))
                {

                    ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                    ManagementBaseObject newGate = mo.GetMethodParameters("SetGateways");
                    ManagementBaseObject newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");

                    newGate["DefaultIPGateway"] = Gateway.Split(';');
                    newGate["GatewayCostMetric"] = new int[] { 1 };

                    newIP["IPAddress"] = IpAddresses.Split(';');
                    newIP["SubnetMask"] = SubnetMask.Split(';');

                    newDNS["DNSServerSearchOrder"] = DnsSearchOrder.Split(';');

                    ManagementBaseObject setIP = mo.InvokeMethod("EnableStatic", newIP, null);
                    Console.WriteLine(setIP["RETURNVALUE"].GetType());
                    int IPRtn = (int)(UInt32)setIP["RETURNVALUE"];
                    
                    ManagementBaseObject setGateways = mo.InvokeMethod("SetGateways", newGate, null);
                    int GatewayRtn = (int)(UInt32)setGateways["RETURNVALUE"];

                    ManagementBaseObject setDNS = mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                    int DNSrtn = (int)(UInt32)setDNS["RETURNVALUE"];

                    int[] rtn = { IPRtn, GatewayRtn, DNSrtn };
                    return rtn;
                }

               /* // Make sure this is a IP enabled device. Not something like memory card or VM Ware
                if ((bool)mo["IPEnabled"])
                {
                    if (mo["Caption"].Equals(nicName))
                    {
                        
                        ManagementBaseObject newIP = mo.GetMethodParameters("EnableStatic");
                        ManagementBaseObject newGate = mo.GetMethodParameters("SetGateways");
                        ManagementBaseObject newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");

                        newGate["DefaultIPGateway"] = Gateway.Split(';');
                        newGate["GatewayCostMetric"] = new int[] { 1 };

                        newIP["IPAddress"] = IpAddresses.Split(';');
                        newIP["SubnetMask"] = SubnetMask.Split(';');

                        newDNS["DNSServerSearchOrder"] = DnsSearchOrder.Split(';');

                        ManagementBaseObject setIP = mo.InvokeMethod("EnableStatic", newIP, null);
                        Console.WriteLine(setIP["RETURNVALUE"]);
                        ManagementBaseObject setGateways = mo.InvokeMethod("SetGateways", newGate, null);
                        ManagementBaseObject setDNS = mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);

                        MessageBox.Show("IP Adrressing chnage successfully added..!!!");
                        break;
                    }
                }*/
               
            }
            int[] error = {-1, -1, -1};
            return error;
            
        }

        /// <summary>
        /// Enable DHCP on the NIC
        /// </summary>
        /// <param name="nicName">Name of the NIC</param>
        /// <returns>setDHCP return</returns>
        public static int SetDHCP(string nicName)
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();

            foreach (ManagementObject mo in moc)
            {
                if (mo["Caption"].Equals(nicName))
                {
                    ManagementBaseObject newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    newDNS["DNSServerSearchOrder"] = null;

                    ManagementBaseObject enableDHCP = mo.InvokeMethod("EnableDHCP", null, null);
                    ManagementBaseObject setDNS = mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                    int rtn = (int)(UInt32)setDNS["RETURNVALUE"];
                    return rtn;
                }

                /*// Make sure this is a IP enabled device. Not something like memory card or VM Ware
                if ((bool)mo["IPEnabled"])
                {
                    if (mo["Caption"].Equals(nicName))
                    {
                        ManagementBaseObject newDNS = mo.GetMethodParameters("SetDNSServerSearchOrder");
                        newDNS["DNSServerSearchOrder"] = null;

                        ManagementBaseObject enableDHCP = mo.InvokeMethod("EnableDHCP", null, null);
                        ManagementBaseObject setDNS = mo.InvokeMethod("SetDNSServerSearchOrder", newDNS, null);
                        MessageBox.Show("DHCP enabled on the chosen NIC..!!!");
                    }
                }*/
            }
            return -1;
        }
        #endregion

        #region show Message according to given integer
        /// <summary>
        /// shows a message box according to the integer specified in the Win32_NetworkAdapterConfiguration class
        /// </summary>
        /// <param name="output">value return by calling Win32_NetworkAdapterConfiguration class methods  </param>
        public static void showMessage(int output) {
            switch (output) {
                case 0:
                    MessageBox.Show("Successful completion, no reboot required.", "Information");
                    break;
                case 1:
                    MessageBox.Show("Successful completion, reboot required.", "Information");
                    break;
                case 64:
                    MessageBox.Show("Method not supported on this platform.", "Error");
                    break;
                case 65:
                    MessageBox.Show("Unknown failure.", "Error");
                    break;
                case 66:
                    MessageBox.Show("Invalid subnet mask.", "Error");
                    break;
                case 67:
                    MessageBox.Show("An error occurred while processing an instance that was returned.", "Error");
                    break;
                case 68:
                    MessageBox.Show("Invalid input parameter.", "Error");
                    break;
                case 69:
                    MessageBox.Show("More than five gateways specified.", "Error");
                    break;
                case 70:
                    MessageBox.Show("Invalid IP address.", "Error");
                    break;
                case 71:
                    MessageBox.Show("Invalid gateway IP address.", "Error");
                    break;
                case 72:
                    MessageBox.Show("An error occurred while accessing the registry for the requested information.", "Error");
                    break;
                case 73:
                    MessageBox.Show("Invalid domain name.", "Error");
                    break;
                case 74:
                    MessageBox.Show("Invalid host name.", "Error");
                    break;
                case 75:
                    MessageBox.Show("No primary or secondary WINS server defined.", "Error");
                    break;
                case 76:
                    MessageBox.Show("Invalid file.", "Error");
                    break;
                case 77:
                    MessageBox.Show("Invalid system path.", "Error");
                    break;
                case 78:
                    MessageBox.Show("File copy failed.", "Error");
                    break;
                case 79:
                    MessageBox.Show("Invalid security parameter.", "Error");
                    break;
                case 80:
                    MessageBox.Show("Unable to configure TCP/IP service.", "Error");
                    break;
                case 81:
                    MessageBox.Show("Unable to configure DHCP service.", "Error");
                    break;
                case 82:
                    MessageBox.Show("Unable to renew DHCP lease.", "Error");
                    break;
                case 83:
                    MessageBox.Show("Unable to release DHCP lease.", "Error");
                    break;
                case 84:
                    MessageBox.Show("IP not enabled on adapter.", "Error");
                    break;
                case 85:
                    MessageBox.Show("IPX not enabled on adapter.", "Error");
                    break;
                case 86:
                    MessageBox.Show("Frame or network number bounds error.", "Error");
                    break;
                case 87:
                    MessageBox.Show("Invalid frame type.", "Error");
                    break;
                case 88:
                    MessageBox.Show("Invalid network number.", "Error");
                    break;
                case 89:
                    MessageBox.Show("Duplicate network number.", "Error");
                    break;
                case 90:
                    MessageBox.Show("Parameter out of bounds.", "Error");
                    break;
                case 91:
                    MessageBox.Show("Access denied.", "Error");
                    break;
                case 92:
                    MessageBox.Show("Out of memory.", "Error");
                    break;
                case 93:
                    MessageBox.Show("Already exists.", "Error");
                    break;
                case 94:
                    MessageBox.Show("Path, file, or object not found.", "Error");
                    break;
                case 95:
                    MessageBox.Show("Unable to notify service.", "Error");
                    break;
                case 96:
                    MessageBox.Show("Unable to notify DNS service.", "Error");
                    break;
                case 97:
                    MessageBox.Show("Interface not configurable.", "Error");
                    break;
                case 98:
                    MessageBox.Show("Not all DHCP leases could be released or renewed.", "Error");
                    break;
                case 100:
                    MessageBox.Show("DHCP not enabled on the adapter.", "Error");
                    break;
                default:
                    MessageBox.Show(output+"\n"+"Unexpected Error Occured.", "Error");
                    break;
            }
        }

        #endregion
    }

}
