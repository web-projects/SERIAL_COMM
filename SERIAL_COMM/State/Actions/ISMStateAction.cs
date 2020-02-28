using IPA5.DAL.Core.State;
using SERIAL_COMM.State.Enums;
using SERIAL_COMM.State.Interfaces;
using System;
using System.Threading.Tasks;

namespace SERIAL_COMM.State.Actions
{
    internal interface ISMStateAction : IActionReceiver, IDisposable
    {
        StateException LastException { get; }
        ISMStateController Controller { get; }
        SMWorkflowState WorkflowStateType { get; }
        void SetState(object stateObject);
        SMWorkflowStopReason StopReason { get; }
        bool DoDeviceDiscovery();
        Task DoWork();
    }
}
