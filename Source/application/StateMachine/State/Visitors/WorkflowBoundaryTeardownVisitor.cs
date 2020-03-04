﻿using DEVICE_CORE.State.Interfaces;
using DEVICE_CORE.State.SubWorkflows;

namespace DEVICE_CORE.State.Visitors
{
    internal class WorkflowBoundaryTeardownVisitor : IStateControllerVisitor<ISubWorkflowHook, IDeviceSubStateController>
    {
        public void Visit(ISubWorkflowHook context, IDeviceSubStateController visitorAcceptor) => context.UnHook();
    }
}