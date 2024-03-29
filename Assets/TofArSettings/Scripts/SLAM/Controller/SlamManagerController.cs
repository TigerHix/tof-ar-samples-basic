﻿/*
 * SPDX-License-Identifier: (Apache-2.0 OR GPL-2.0-only)
 *
 * Copyright 2022 Sony Semiconductor Solutions Corporation.
 *
 */

using TofAr.V0.Slam;
using System;
using System.Linq;


namespace TofArSettings.Slam
{
    public class SlamManagerController : ControllerBase
    {
        private void Awake()
        {
            isStarted = TofArSlamManager.Instance.autoStart;

            SetupPoseSourceLists();
        }

        private void OnEnable()
        {
            TofArSlamManager.OnStreamStarted += OnSlamStreamStarted;
            TofArSlamManager.OnStreamStopped += OnSlamStreamStopped;
        }

        private void OnDisable()
        {
            TofArSlamManager.OnStreamStopped -= OnSlamStreamStopped;
            TofArSlamManager.OnStreamStarted -= OnSlamStreamStarted;
        }

        private void OnSlamStreamStarted(object sender)
        {
            isStarted = true;
            OnStreamStartStatusChanged?.Invoke(isStarted);
        }

        private void OnSlamStreamStopped(object sender)
        {
            isStarted = false;
            OnStreamStartStatusChanged?.Invoke(isStarted);
        }

        public bool IsStreamActive()
        {
            return TofArSlamManager.Instance.IsStreamActive;
        }

        private bool isStarted = false;

        /// <summary>
        /// Start stream
        /// </summary>
        public void StartStream()
        {
            TofArSlamManager.Instance.StartStream();
        }

        /// <summary>
        /// Stop stream
        /// </summary>
        public void StopStream()
        {
            TofArSlamManager.Instance.StopStream();
        }

        public event ChangeToggleEvent OnStreamStartStatusChanged;

        public CameraPoseSource CameraPoseSource
        {
            get => TofArSlamManager.Instance.CameraPoseSource;
            set
            {
                if (value != TofArSlamManager.Instance.CameraPoseSource)
                {
                    TofArSlamManager.Instance.CameraPoseSource = value;
                    Index = Utils.Find(value, PoseSources);
                }
            }
        }

        private int index = 0;
        public int Index
        {
            get
            {
                return index;
            }

            set
            {
                if (value != Index &&
                    0 <= value && value < PoseSources.Length)
                {
                    index = value;
                    CameraPoseSource = PoseSources[index];
                    OnChangeIndex?.Invoke(Index);
                }
            }
        }

        public string[] PoseSourceNames { get; private set; }

        public CameraPoseSource[] PoseSources { get; private set; }

        public event ChangeIndexEvent OnChangeIndex;

        private void SetupPoseSourceLists()
        {
            PoseSources = (CameraPoseSource[])Enum.GetValues(typeof(CameraPoseSource));
            PoseSourceNames = PoseSources.Select((s) => s.ToString()).ToArray();
        }

    }
}
