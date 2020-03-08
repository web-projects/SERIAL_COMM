using StateMachine.State.Interfaces;
using StateMachine.State.Visitors;
using System;

namespace StateMachine.State.SubWorkflows.Management
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
