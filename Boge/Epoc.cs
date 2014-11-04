using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace WS_STE
{
    /// <summary>
    /// Epoc packet data struct
    /// </summary>
    public struct EPOC_Data
    {
        public int GyroX, GyroY, F3Data, FC5Data, AF3Data, F7Data, T7Data, P7Data, O1Data, O2Data, P8Data, T8Data, F8Data, AF4Data, FC6Data, F4Data, packetC;
    }

    public delegate void EpocCallback(DateTime ts, EPOC_Data data);
    public delegate void EpocConnectionCallback(DateTime ts, bool connected);

    public class Epoc
    {
        #region Wrapper
        /// <summary>
        /// Connette all'headset e ritorna l'handle
        /// </summary>
        /// <returns></returns>
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        static extern IntPtr EPOC_Connect();

        /// <summary>
        /// Legge i dati e ritorna lo stato di connessione
        /// </summary>
        /// <param name="e"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        static extern bool EPOC_Read(IntPtr e, out EPOC_Data s);

        /// <summary>
        /// Ritorna la percentuale di batteria rimanente.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        static extern double EPOC_getBatteryLevel(IntPtr e);

        /// <summary>
        /// Ritorna la qualità media dei sensori
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        static extern double EPOC_getAVGContactQuality(IntPtr e);

        /// <summary>
        /// Ritorna la qualità del sensore numero <see cref="contact:"/>
        /// </summary>
        /// <param name="e"></param>
        /// <param name="contact"></param>
        /// <returns></returns>
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        static extern double EPOC_getContactQuality(IntPtr e, int contact);

        /// <summary>
        /// Libera le risorse
        /// </summary>
        /// <param name="e"></param>
        [DllImport(_dllName, CallingConvention = CallingConvention.Cdecl)]
        static extern void EPOC_Dispose(IntPtr e);
        #endregion

        const string _dllName = "Epoc.dll";

        static volatile int _askedQuality = 0;
        static volatile int _bat;
        static volatile bool _connected;
        static IntPtr _e = IntPtr.Zero;
        static Thread _scanner;

        private static void OnSensorDataAvailable(DateTime now, EPOC_Data data)
        {
            if (SensorsDataAvailable != null)
                SensorsDataAvailable.Invoke(now, data);
        }

        private static void OnQualityDataAvailable(DateTime now, EPOC_Data data)
        {
            if (QualityDataAvailable != null)
                QualityDataAvailable.Invoke(now, data);
        }

        private static void OnConnectedChanged(DateTime now, bool connected)
        {
            if (ConnectedChanged != null)
                ConnectedChanged.Invoke(now, connected);
        }

        protected Epoc() { }

        protected static void scan()
        {
            while (true)
            {
                try
                {
                    _bat = (int)(EPOC_getBatteryLevel(_e) * 100);
                    EPOC_Data a;
                    EPOC_Read(_e, out a);
                    DateTime n = DateTime.Now;
                    if (Connected = (EPOC_getAVGContactQuality(_e) >= 0))
                        OnSensorDataAvailable(n, a);
                    if (_askedQuality > 0)
                    {
                        a.GyroX = 100;
                        a.GyroY = 100;
                        object b = a;
                        for (int i = 0; i < a.GetType().GetFields().Length - 3/* 2 gyro + 1 pack c*/; i++)
                        {
                            double qq = EPOC_getContactQuality(_e, i);
                            int q = (int)(qq * 100);
                            a.GetType().GetFields()[i + 2/* 2 gyro*/].SetValue(b, q * 100);
                        }
                        _askedQuality--;
                        OnQualityDataAvailable(n, (EPOC_Data)b);
                    }
                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        /// <summary>
        /// Creates an Epoc Connection
        /// </summary>
        /// <returns></returns>
        public static void Create()
        {
            if (_e == IntPtr.Zero)
            {
                try
                {
                    _e = EPOC_Connect();
                    _scanner = null;
                    _scanner = new Thread(new ThreadStart(scan));
                    _scanner.IsBackground = true;
                    _scanner.Start();
                }
                catch (Exception e)
                {
                    MessageBox.Show("An error occurred while connecting to the headset, check your c++ runtime distributions and dlls.\n\nWARNING: The headset's data won't be available until this problem is not fixed.\n\n" + e.Message, "Runtime Dll Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                finally
                {
                }
            }
        }

        /// <summary>
        /// Scatenato all'arrivo di un pacchetto dati.
        /// </summary>
        public static event EpocCallback SensorsDataAvailable;

        /// <summary>
        /// Scatenato alla lettura dei valori di qualità.
        /// </summary>
        public static event EpocCallback QualityDataAvailable;

        /// <summary>
        /// Scatenato al cambiamento dello stato della connessione.
        /// </summary>
        public static event EpocConnectionCallback ConnectedChanged;

        /// <summary>
        /// Scatena il prima possibile un evento <see cref="QualityDataAvailable"/>
        /// </summary>
        public static void AskQuality()
        {
            _askedQuality = 1;
        }

        public static int BatteryCharge { get { return _bat; } set { _bat = value; } }

        public static bool Created { get { return _e != IntPtr.Zero; } }

        public static bool Connected
        {
            get
            {
                return _connected;
            }
            private set
            {
                if (_connected != value)
                    OnConnectedChanged(DateTime.Now, _connected = value);
            }
        }

        /// <summary>
        /// Dispone
        /// </summary>
        public static void Dispose()
        {
            try
            {
                _scanner.Abort();
            }
            catch (Exception)
            {

            }
            try
            {
                EPOC_Dispose(_e);
                _e = IntPtr.Zero;
            }
            catch (Exception
#if DEBUG 
                e)
            {
                MessageBox.Show("Error releasing EPOC: " + e.Message);
#else
                )
            {
#endif
            }
        }
    }
}
