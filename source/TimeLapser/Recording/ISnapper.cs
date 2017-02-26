﻿using System;
using System.Drawing;
using System.Threading.Tasks;

namespace kasthack.TimeLapser {
    interface ISnapper : IDisposable
    {
        void SetSource(Rectangle sourceRect);
        /*
         * _reusable_ bitmap
         * implementation must dispose it
         */
        Task<Bitmap> Snap(int timeout = 0);
        int MaxProcessingThreads { get; }
    }
}