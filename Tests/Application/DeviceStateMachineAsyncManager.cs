﻿using Moq;
using DEVICE_CORE.StateMachine.State.Actions;
using DEVICE_CORE.StateMachine.State.Interfaces;
using System.Threading;

namespace DEVICE_CORE.StateMachine.Tests
{
    class DeviceStateMachineAsyncManager
    {
        readonly ManualResetEvent resetEvent;

        public DeviceStateMachineAsyncManager()
            => resetEvent = new ManualResetEvent(false);

        public DeviceStateMachineAsyncManager(ref Mock<IDeviceStateController> mockController, IDeviceStateAction stateAction)
            : this()
        {
            mockController.Setup(e => e.Complete(stateAction)).Callback(() => resetEvent.Set());
            mockController.Setup(e => e.Error(stateAction)).Callback(() => resetEvent.Set());
        }

        public void Trigger() => resetEvent.Set();

        public bool WaitFor(int timeout = 2000) => resetEvent.WaitOne(timeout);

        public void Dispose() => resetEvent.Dispose();
    }
}
