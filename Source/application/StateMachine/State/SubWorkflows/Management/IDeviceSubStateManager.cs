using DEVICE_CORE.StateMachine.State.Interfaces;
using DEVICE_CORE.StateMachine.State.SubWorkflows;
using System;

namespace DEVICE_CORE.State.SubWorkflows.Management
{
    public delegate void OnSubWorkflowCompleted();
    public delegate void OnSubWorkflowError();

    internal interface IDeviceSubStateManager : IActionReceiver, IStateControllerVisitable<ISubWorkflowHook, IDALSubStateController>, IDisposable
    {
        void LaunchWorkflow(WorkflowOptions launchOptions);
        event OnSubWorkflowCompleted SubWorkflowComplete;
        event OnSubWorkflowError SubWorkflowError;
    }
}
