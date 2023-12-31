﻿using ScottPlot.Palettes;
using ScottPlot.Plottable.AxisManagers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.PerformanceData;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Win32;
using winmdroot = global::Windows.Win32;

namespace PerfMonitor
{
    class QueryOnce
    {
        public static double Query(string query)
        {
            nint _hQuery;
            nint _hCounter;
            _ = PInvoke.PdhOpenQuery(null, 0, out _hQuery);
            nuint user = 0;
            _ = PInvoke.PdhAddCounter(_hQuery, query, user, out _hCounter);

            winmdroot.System.Performance.PDH_RAW_COUNTER rawData;

            _ = PInvoke.PdhCollectQueryData(_hQuery);

            uint type = 0;
            unsafe
            {
                _ = PInvoke.PdhGetRawCounterValue(_hCounter, &type, out rawData);
            }

            _ = PInvoke.PdhRemoveCounter(_hCounter);
            _ = PInvoke.PdhCloseQuery(_hQuery);

            return rawData.FirstValue;
        }
    }

    internal class PerfQuery : IDisposable
    {
        private nint _hQuery;
        private nint _hCounter;
        private winmdroot.System.Performance.PDH_RAW_COUNTER _lastData;
        private bool _first = true;

        public PerfQuery(string query)
        {
            PInvoke.PdhOpenQuery(null, 0, out _hQuery);
            nuint user = 0;
            PInvoke.PdhAddCounter(_hQuery, query, user, out _hCounter);
        }

        public double NextValue()
        {
            double cpu = 0.0f;
            winmdroot.System.Performance.PDH_RAW_COUNTER rawData;

            PInvoke.PdhCollectQueryData(_hQuery);

            uint type = 0;
            unsafe
            {
                PInvoke.PdhGetRawCounterValue(_hCounter, &type, out rawData);
            }

            if (!_first)
            {
                winmdroot.System.Performance.PDH_FMT_COUNTERVALUE fmtValue;
                winmdroot.System.Performance.PDH_FMT fmt = winmdroot.System.Performance.PDH_FMT.PDH_FMT_DOUBLE;
                PInvoke.PdhCalculateCounterFromRawValue(_hCounter, fmt, rawData, _lastData, out fmtValue);
                cpu = fmtValue.Anonymous.doubleValue;
                if (cpu > 100)
                    cpu = 100;
            }
            else { _first = false; }

            _lastData = rawData;

            return cpu;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                PInvoke.PdhRemoveCounter(_hCounter);
                PInvoke.PdhCloseQuery(_hQuery);
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
