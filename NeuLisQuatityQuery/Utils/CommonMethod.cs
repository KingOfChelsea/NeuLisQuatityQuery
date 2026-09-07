using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace NeuLisQuatityQuery.Utils
{
    public class CommonMethod
    {
        // 获取有效的本机IP地址（排除虚拟网卡）
        public static string GetValidLocalIPAddress()
        {
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // 排除虚拟网卡和回环地址
                    if (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback ||
                        ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel ||
                        ni.Description.Contains("Virtual") ||
                        ni.Description.Contains("VMware") ||
                        ni.Description.Contains("VirtualBox"))
                    {
                        continue;
                    }

                    // 只处理已连接的网卡
                    if (ni.OperationalStatus == OperationalStatus.Up)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            // 只获取IPv4地址
                            if (ip.Address.AddressFamily == AddressFamily.InterNetwork)
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"获取IP地址失败: {ex.Message}");
            }

            return "127.0.0.1";
        }
    }
}
