using System.IO.Pipes;
using System.Text;
using System.Windows;
using static System.Net.Mime.MediaTypeNames;

namespace IPCCommonHelper
{
    public class ClientServerCommonHelper
    {
        #region Fields declaration
        public  AutoResetEvent autoProcessRequest;
        private bool isIPCServerRunning { get; set; }
        private NamedPipeServerStream _pipeServerStream = null;
        private int BytesCount = 10000;
        public string receivedString { get; set; }
        private object objReceivedDataLock = new object();
        #endregion

        #region Events
        public static event EventHandler<MessageReceived> TriggerReceivedMessage = null;
        // Method to trigger the event
        public static void ReceiveMessage(string message)
        {
            // Some message processing logic...

            // Raise the event
            if (!string.IsNullOrEmpty(message))
            {
                MessageReceived eventArgs = new MessageReceived { ReceivedData = message };
                OnTriggerReceivedMessage(eventArgs);
            }
        }

        // Method to raise the event
        private static void OnTriggerReceivedMessage(MessageReceived e)
        {
            // Verify if there are any attendees registered for the event.
            if (TriggerReceivedMessage != null)
            {
                // Raise the event
                TriggerReceivedMessage.Invoke(null, e);
            }
        }
        #endregion

        #region Singleton instance
        private static readonly object objLock = new object();
        private static ClientServerCommonHelper instance = null;
        public static ClientServerCommonHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (objLock)
                    {
                        if (instance == null)
                        {
                            instance = new ClientServerCommonHelper();
                        }
                    }
                }
                return instance;
            }
        }
        #endregion
        public ClientServerCommonHelper()
        {
            isIPCServerRunning = false;
            autoProcessRequest = new AutoResetEvent(false);
        }

        public void StartServer()
        {

            try
            {
                _pipeServerStream = new NamedPipeServerStream(CommonConstant.NamedPipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                _pipeServerStream.BeginWaitForConnection(new AsyncCallback(CallBackWaitingHandler), _pipeServerStream);
                isIPCServerRunning = true;
                Thread thread = new Thread(ReceiveDataProcessHelper);
                thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ReceiveDataProcessHelper(object? obj)
        {
            try
            {
                while (isIPCServerRunning)
                {
                    if (autoProcessRequest.WaitOne())
                    {
                        autoProcessRequest.Reset();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CallBackWaitingHandler(IAsyncResult ar)
        {
            try
            {

                NamedPipeServerStream pipeServerStream = (NamedPipeServerStream)ar.AsyncState;
                pipeServerStream.EndWaitForConnection(ar);
                byte[] byteArray = new byte[BytesCount];
                pipeServerStream.Read(byteArray, 0, BytesCount);
                // Transform byte array into a string.
                string text = Encoding.UTF8.GetString(byteArray);
                lock (objReceivedDataLock)
                {
                   // receivedString = text;
                    ReceiveMessage(text);
                    autoProcessRequest.Set();
                }
                // Terminate all running processes.
                pipeServerStream.Close();
                pipeServerStream = null;
                pipeServerStream = new NamedPipeServerStream(CommonConstant.NamedPipeName, PipeDirection.In, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);
                pipeServerStream.BeginWaitForConnection(new AsyncCallback(CallBackWaitingHandler), pipeServerStream);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Transfer information via Inter-Process Communication.
        public void TransmitDataToIPCServerExecutable(string data)
        {
            {
                try
                {
                    NamedPipeClientStream pipeStreamClient = new NamedPipeClientStream(".", CommonConstant.NamedPipeName, PipeDirection.Out, PipeOptions.Asynchronous);
                    SelfTestUnsuccessfulImplementationProcedure(10, TimeSpan.FromMilliseconds(400), () =>
                    {
                        pipeStreamClient.Connect(10000);
                    });

                    if (!pipeStreamClient.IsConnected)
                    {
                        return;
                    }

                    if (!pipeStreamClient.CanWrite)
                    {
                        return;
                    }
                    byte[] byteArray = Encoding.UTF8.GetBytes(data);
                    IAsyncResult writeResult = pipeStreamClient.BeginWrite(byteArray, 0, byteArray.Length, ResponseMessageSent, pipeStreamClient);
                    writeResult.AsyncWaitHandle.WaitOne();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);    
                }
            }
        }
        private void ResponseMessageSent(IAsyncResult iar)
        {

            try
            {
                NamedPipeClientStream clientPipeStream = (NamedPipeClientStream)iar.AsyncState;
                clientPipeStream.EndWrite(iar);
                clientPipeStream.Flush();
                clientPipeStream.Close();
                clientPipeStream.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        public void SelfTestUnsuccessfulImplementationProcedure(int maximumCounter, TimeSpan delay, Action actionProcess)
        {

            try
            {
                var counter = 0;
                do
                {
                    try
                    {
                        counter++;
                        actionProcess();
                        break; 
                    }
                    catch (Exception ex)
                    {
                        if (counter == maximumCounter)
                        {
                            return;
                        }
                        Task.Delay(delay).Wait();
                    }
                } while (true);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion
    }

    public class MessageReceived:EventArgs
    {
        public string ReceivedData { get; set; }
    }
}
