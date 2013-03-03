using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using Qi.Sms.Protocol;
using Qi.Sms.Protocol.SendCommands;
using log4net;

namespace Qi.Sms.DeviceConnections
{
    public sealed class ComConnection : IDeviceConnection
    {
        private readonly StringBuilder _buffer = new StringBuilder();
        private readonly object _lockItem = "";
        private readonly ILog _log = LogManager.GetLogger(typeof(ComConnection));
        private readonly SerialPort _serialPort;
        public ComConnection(string portName, int baudRate)
        {
            if (portName == null)
                throw new ArgumentNullException("portName");
            PortName = portName;
            BaudRate = baudRate;
            _serialPort = new SerialPort(portName, BaudRate);
            //{
            //    DtrEnable = true,
            //    RtsEnable = true,
            //    Handshake = Handshake.None
            //};

            _serialPort.DataReceived += SerialPortDataReceived;
        }

        public string PortName { get; private set; }

        public int BaudRate { get; private set; }

        #region IDeviceConnection Members

        public event EventHandler<DeviceCommandEventHandlerArgs> ReceivedEvent;

        public void Dispose()
        {
            if (_serialPort != null)
            {
                _serialPort.Close();
                _serialPort.Dispose();
            }
            _log.Info("ComConnection is disposed.");
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

        public SerialPort SerialPort
        {
            get { return _serialPort; }
        }

        public bool IsConnected { get; private set; }

        public string Send(AbstractCommand command)
        {
            lock (_lockItem)
            {
                try
                {
                    _buffer.Remove(0, _buffer.Length);
                    ThreadPool.QueueUserWorkItem(delegate { _serialPort.WriteLine(command.CompleteCommand()); });
                    return WaitReturn(command);
                }
                finally
                {
                    _log.Info("Complete receive is: " + _buffer.ToString());
                    _buffer.Remove(0, _buffer.Length);
                }
            }
        }

        #endregion
        [Obsolete]
        public void InvokeSend(AbstractCommand command, int sleepMilSeconds)
        {
            lock (_lockItem)
            {
                _log.Debug("Invoke send");
                _buffer.Remove(0, _buffer.Length);
                _serialPort.WriteLine(command.CompleteCommand());
                _log.Debug("Invoke send success and wait " + sleepMilSeconds);
                Thread.Sleep(sleepMilSeconds);
                _buffer.Remove(0, _buffer.Length);
            }
        }

        private string WaitReturn(AbstractCommand command)
        {
            var now = DateTime.Now;
            while (true)
            {
                if ((DateTime.Now - now).TotalSeconds > 12)
                {
                    _log.Warn("waiting return timeout.");
                    break;
                }
                if (command.Init(_buffer.ToString()))
                {
                    _log.Debug("Return value,command is success or not?" + command.Success);
                    break;
                }
                Thread.Sleep(100);
            }
            string result = _buffer.ToString();
            //_log.InfoFormat("Wait Return  command \r\n {0}", result);
            return result;
        }

        public void Send(string command)
        {
            _serialPort.WriteLine(command + "\r\n");
        }

        private void SerialPortDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            _log.Debug("receiver something.");
            var serialPort = (SerialPort)sender;
            _buffer.Append(serialPort.ReadExisting());
            var result = CmtiCommand.GetSmsIndex(_buffer.ToString());
            _log.Debug(_buffer.ToString());
            //_log.Debug("sms receive :" + result.Length);
            if (result.Length != 0)
            {
                OnReceiveSms(result);
            }
        }

        private void OnReceiveSms(int[] command)
        {
            if (ReceivedEvent != null)
            {

                ThreadPool.QueueUserWorkItem(s => ReceivedEvent(this, new DeviceCommandEventHandlerArgs((int[])s)), command);

            }
        }
    }
}