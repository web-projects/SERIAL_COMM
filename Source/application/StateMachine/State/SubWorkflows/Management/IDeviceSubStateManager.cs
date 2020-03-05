using DEVICE_CORE.StateMachine.State.Interfaces;
using DEVICE_CORE.StateMachine.State.Visitors;
using System;

namespace DEVICE_CORE.StateMachine.State.SubWorkflows.Management
{
    public delegate void OnSubWorkflowCompleted();
    public delegate void OnSubWorkflowError();

    internal interface IDeviceSubStateManager : IActionReceiver, IStateControllerVisitable<ISubWorkflowHook, IDeviceSubStateController>, IDisposable
    {
        void LaunchWorkflow(WorkflowOptions launchOptions);
        event OnSubWorkflowCompleted SubWorkflowComplete;
        event OnSubWorkflowError SubWorkflowError;
    }
}
