using XO.Requests;

namespace DEVICE_CORE.StateMachine.State.SubWorkflows
{
    internal class WorkflowOptions
    {
        public int? ExecutionTimeout;
        public LinkRequest StateObject;
    }
}
