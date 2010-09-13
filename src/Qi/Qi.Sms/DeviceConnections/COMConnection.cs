using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using log4net;

namespace Qi.Sms.Protocol.DeviceConnections
{
    public sealed class ComConnection : IDeviceConnection
    {
        private readonly ILog _log = LogManager.GetLogger(typeof(ComConnection));
        private readonly SerialPort _serialPort;
        private readonly object lockItem = "";
        private bool _hasReturnValue;
        private StringBuilder _returnValue;

        public ComConnection(string comName, int baudRate)
        {
            ComName = comName;
            BaudRate = baudRate;
            _serialPort = new SerialPort(ComName, BaudRate, Parity.None, 8, StopBits.One)
                              {
                                  DtrEnable = true,
                                  RtsEnable = true,
                                  Handshake = Handshake.None
                              };
            _serialPort.DataReceived += SerialPortDataReceived;
        }

        public string ComName { get; private set; }

        public int BaudRate { get; private set; }

        #region IDeviceConnection Members

        public event EventHandler<DeviceCommandEventHandlerArgs> SendingEvent;
        public event EventHandler<DeviceCommandEventHandlerArgs> ReceivedEvent;

        public void Dispose()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
        }

        public void Open()
        {
            try
            {
                _serialPort.Open();
                IsConnected = true;
            }
            catch (Exception ex)
            {
                throw new DeviceConnectionException(ex.Message, ex);
            }
        }

        public void Close()
        {
            try
            {
                _serialPort.Close();
                IsConnected = false;
            }
            catch (Exception ex)
            {
                throw new DeviceConnectionException(ex.Message, ex);
            }
        }

        public string Send(BaseCommand command)
        {
            return Send(command.ToString(), command.NoReturnValue);
        }

        public bool IsConnected { get; private set; }

        public string Send(string command, bool noReturnValue)
        {
            if (SendingEvent != null)
                SendingEvent(this, new DeviceCommandEventHandlerArgs(command));
            _log.InfoFormat("send command {0} with NoReturnValue={1}", command, noReturnValue);
            lock (lockItem)
            {
                ThreadPool.QueueUserWorkItem(delegate { _serialPort.WriteLine(command + "\r\n"); });
                Thread.Sleep(500);
                if (noReturnValue)
                    return "";

                _returnValue = new StringBuilder();
                DateTime now = DateTime.Now;

                while (!_hasReturnValue)
                {
                    Thread.Sleep(100);
                    TimeSpan span = DateTime.Now - now;
                    if (span.Seconds > 5)
                    {
                        _log.InfoFormat("Send command {0} timeout,and exit", command);
                        return ""; //throw new TimeoutException("Timeout {0} send fail.");
                    }
                }
                string result = _returnValue.ToString();
                _hasReturnValue = false;
                _returnValue = null;
                _log.InfoFormat("receive command {0}", result);
                return result;
            }
        }

        #endregion

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var serialPort = (SerialPort)sender;
            var result = new StringBuilder();
            while (serialPort.BytesToRead > 0)
            {
                string re = serialPort.ReadExisting();
                result.Append(re);
                if (_returnValue != null)
                {
                    _returnValue.Append(re);
                }
                Thread.Sleep(500);
            }
            if (_returnValue != null)
            {
                _hasReturnValue = true;
            }
            OnReceiveEvent(result.ToString());
        }

        public void OnReceiveEvent(string command)
        {
            if (ReceivedEvent != null)
            {
                ThreadPool.QueueUserWorkItem(e => ReceivedEvent(this, new DeviceCommandEventHandlerArgs(command)));
            }
        }
    }
}