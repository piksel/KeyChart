using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HidLibrary;
using KeyChart.GUI.Util;
using KeyChart.Keyboards.QMK;
using Microsoft.Win32.SafeHandles;

namespace KeyChart.Platforms.Windows
{
    public static class HidDeviceExtensions
    {
        static Encoding hidEncoding = new UnicodeEncoding(false, false, false);
        
        public static string StringFromBytes(IEnumerable<byte> bytes)
            => $"{hidEncoding.GetString(bytes.ToArray()).TrimEnd('\0')} (0x{string.Join("", bytes.Select(b => $"{b:x2}")).TrimEnd('0')})";

        public static string GetManufacturer(this HidDevice d)
            => d.ReadManufacturer(out var bytes) ? StringFromBytes(bytes) : "Unknown";
        
        public static string GetProduct(this HidDevice d)
            => d.ReadProduct(out var bytes) ? StringFromBytes(bytes) : "Unknown";

        public static string GetId(this HidDevice d)
            => $"{d.Attributes.VendorId:X4}:{d.Attributes.ProductId:X4}:{d.Attributes.Version:X4}";
    }

    public class HidComms
    {
        private List<HidDevice> _devices = new List<HidDevice>();
        public ushort UsagePage { get; }
        public int Usage { get; }
        
        public HidComms(ushort usagePage, int usage)
        {
            UsagePage = usagePage;
            Usage = usage;
        }

        public const ushort ConsoleUsagePage = 0xFF31;
        public const int ConsoleUsage = 0x0074;

        public static HidComms ConsoleHid => new(ConsoleUsagePage, ConsoleUsage);
        public static HidComms QmkRawHid => new(0xFF60 , 0x0061);
        
        public void UpdateHidDevices(bool disconnected)
        {
            var devices = GetListableDevices().ToList();

            var cts = new CancellationTokenSource();

            if (!disconnected)
            {
                foreach (var device in devices)
                {
                    var deviceExists = _devices.Aggregate(false, (current, dev) => current | dev.DevicePath.Equals(device.DevicePath));

                    if (device == null || deviceExists) continue;

                    _devices.Add(device);
                    device.OpenDevice(DeviceMode.Overlapped, DeviceMode.Overlapped, ShareMode.ShareRead | ShareMode.ShareWrite);

                    // TODO: Monitor events!!
                    device.MonitorDeviceEvents = false;
                    

                    Log($"HID console connected: {device.GetManufacturer()} {device.GetProduct()} ({device.GetId()}) from {device.DevicePath}");
                    DefaultDevice = device;

                    Task.Run<Task>(async () =>
                    {
                        while (!cts.Token.IsCancellationRequested)
                        {
                            try
                            {
                                await using var stream = OpenReadHidDevice(device.DevicePath);
                                var buffer = new byte[device.Capabilities.InputReportByteLength];

                                while (true)
                                {
                                    var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, cts.Token)
                                        .ConfigureAwait(false);
                                    var report = new HidReport(reportSize: buffer.Length,
                                        new HidDeviceData(buffer, HidDeviceData.ReadStatus.Success));
                                    // var report = device.ReadReport(1000);
                                    Debug.WriteLine($"Read {bytesRead} bytes (ReportID: {buffer[0]:x2}");
                                    HandleReportAsync(report);
                                    if (bytesRead == 0) break;
                                }

                                if (!device.IsConnected)
                                {
                                    Debug.WriteLine($"Device closed!");
                                    break;
                                }
                            }
                            catch (Exception x)
                            {
                                Debug.WriteLine($"ERROR: {x}");
                            }

                        }
                    }, cts.Token);
                    device.CloseDevice();
                }
            }
            else
            {
                foreach (var existingDevice in _devices)
                {
                    var deviceExists = devices.Any(device => existingDevice.DevicePath.Equals(device.DevicePath));

                    if (!deviceExists)
                    {
                        Log($"HID console disconnected ({existingDevice.GetId()})");
                    }
                }
            }

            _devices = devices;
            //UpdateHidList();
        }

        public HidDevice DefaultDevice { get; set; }

        public event EventHandler<string>? MessageLogged; 
        public event EventHandler<string>? RecievedLine; 
        protected void Log(string s)
        {
            MessageLogged?.Invoke(this, s);
        }

        private IEnumerable<HidDevice> GetListableDevices() =>
            HidDevices.Enumerate()
                .Where(d => d.IsConnected)
                .Where(device => device.Capabilities.InputReportByteLength > 0)
                .Where(device => (ushort)device.Capabilities.UsagePage == UsagePage)
                .Where(device => (ushort)device.Capabilities.Usage == Usage);
        
        
        string reportString = String.Empty;
        protected virtual void HandleReportAsync(HidReport report)
        {
            if (report.ReadStatus == HidDeviceData.ReadStatus.WaitTimedOut) return;
            if (report.ReadStatus != HidDeviceData.ReadStatus.Success)
            {
                Log($"Read failed: {report.ReadStatus:G}");
                return;
            }
            
            var data = report.Data;
            Log($"Got data: {string.Join(" ", data.Select(b => $"{b:x2}"))}");
            //Print(string.Format("* recv {0} bytes:", data.Length));
            
            for (var i = 0; i < data.Length; i++)
            {
                if(data[i] == 0) break;
                reportString += (char)data[i];
                //  if (i % 16 != 15 || i >= data.Length) continue;
                
            }
            if (reportString.LastOrDefault() == '\n')
            {
                OnRecievedLine($"{reportString.TrimEnd()}");
                reportString = string.Empty;
            }
            else
            {
                 // Log($"Buffer contains: {string.Join(" ", reportString.Select(c => $"{(byte)c:x2}"))}");
            }
            
            Debug.WriteLine($"HID Report ID: {report.ReportId}");
        }

        protected void OnRecievedLine(string line)
        {
            RecievedLine?.Invoke(this, line);
        }
        
        
        private static FileStream OpenReadHidDevice(string devicePath)
        {
            const int FlagOverlapped = 0x40000000;
            var handle = Winterop.CreateFile(
                devicePath,
                FileAccess.Read, //Kernel32.ACCESS_MASK.GENERIC_READ,
                FileShare.ReadWrite, //Kernel32.FILE_SHARE.READ | Kernel32.FILE_SHARE.WRITE,
                IntPtr.Zero,
                FileMode.Open, //Kernel32.CreationDisposition.OPEN_EXISTING,
                (FileAttributes)FlagOverlapped, // Kernel32.FILE_FLAG.OVERLAPPED,
                IntPtr.Zero);

            if (handle.IsInvalid) throw new Win32Exception();

            // Buffer size doesn't matter because the stream is only used for reading
            return new FileStream(handle, FileAccess.Read, bufferSize: 4096, isAsync: true);
        }
    }
    
    
    public class ViaHidComms: HidComms
    {
        public ViaHidComms() : base(0xFF60 , 0x0061)
        {
        }

        protected override void HandleReportAsync(HidReport report)
        {
            if (report.ReadStatus == HidDeviceData.ReadStatus.WaitTimedOut) return;
            try
            {
                if (report.ReadStatus != HidDeviceData.ReadStatus.Success)
                {
                    Log($"Read failed: {report.ReadStatus:G}");
                    return;
                }

                var data = report.Data;
                Log($"Got data: {string.Join(" ", data.Select(b => $"{b:x2}"))}");

                var command = (ViaCommand) data[0];
                var reportString = $"Cmd: {command:g} ({data[0]:x2}) Args: ";
                for (var i = 1; i < data.Length; i++)
                {
                    // if (data[i] == 0) break;
                    reportString += $"{data[i]:x2}";
                    //(char)data[i];
                    //  if (i % 16 != 15 || i >= data.Length) continue;
                }

                Debug.WriteLine(reportString);
                OnRecievedLine($"{reportString.TrimEnd('0')}");
            }
            catch (Exception x)
            {
                Debug.WriteLine($"{x}");
            }
        }
        
        public void SendCommand(ViaCommand cmd, params byte[] cmdData)
        {



            // var sendDevice = new HidDevice(DefaultDevice.DevicePath, DefaultDevice.Description);
            // var data = cmdData.Prepend((byte)cmd).ToArray();
            //var reportLen = DefaultDevice.Capabilities.OutputReportByteLength;
            var data = new byte[2];
            data[1] = (byte)cmd;
                
            Log($"Sending: {cmd:g} => {string.Join(" ", data.Select(b => $"{b:x2}"))}");

            DefaultDevice.Write(data);
            // DefaultDevice.WriteReport(new HidReport(1)
            // {
            //     Data = cmdData,
            //     ReportId = (byte)cmd,
            // });
            //DefaultDevice.CloseDevice();
        }
        

    }
}